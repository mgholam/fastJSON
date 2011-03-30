using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace fastJSON
{
    internal class JSONSerializer
    {
        private readonly StringBuilder _output = new StringBuilder();
        readonly bool useMinimalDataSetSchema;
        readonly bool fastguid = true;
        readonly bool useExtension = true;
        readonly bool serializeNulls = true;
        readonly int _MAX_DEPTH = 10;
        int _current_depth = 0;

        internal JSONSerializer(bool UseMinimalDataSetSchema, bool UseFastGuid, bool UseExtensions, bool SerializeNulls)
        {
            this.useMinimalDataSetSchema = UseMinimalDataSetSchema;
            this.fastguid = UseFastGuid;
            this.useExtension = UseExtensions;
            this.serializeNulls = SerializeNulls;
        }

        internal string ConvertToJSON(object obj)
        {
            WriteObject(obj);

            return _output.ToString();
        }

        private void WriteValue(object obj)
        {
        	if (serializeNulls && (obj == null || obj is DBNull))
                _output.Append("null");

            else if (obj is string || obj is char)
                WriteString((string)obj);

            else if (obj is Guid)
                WriteGuid((Guid)obj);

            else if (obj is bool)
                _output.Append(((bool)obj) ? "true" : "false"); // conform to standard

            else if (
                obj is int || obj is long || obj is double ||
                obj is decimal || obj is float ||
                obj is byte || obj is short ||
                obj is sbyte || obj is ushort ||
                obj is uint || obj is ulong
            )
                _output.Append(((IConvertible)obj).ToString(NumberFormatInfo.InvariantInfo));

            else if (obj is DateTime)
                WriteDateTime((DateTime)obj);

            else if (obj is IDictionary<string, string>)
                WriteStringDictionary((IDictionary)obj);

            else if (obj is IDictionary)
                WriteDictionary((IDictionary)obj);

            else if (obj is DataSet)
                WriteDataset((DataSet)obj);

            else if (obj is byte[])
            	WriteBytes((byte[])obj);

            else if (obj is Array || obj is IList || obj is ICollection)
                WriteArray((IEnumerable)obj);

            else if (obj is Enum)
                WriteEnum((Enum)obj);

            else
                WriteObject(obj);
        }

        private void WriteEnum(Enum e)
        {
            // TODO : optimize enum write
            WriteStringFast(e.ToString());
        }

        private void WriteGuid(Guid g)
        {
            if (fastguid == false)
                WriteStringFast(g.ToString());
            else
                WriteBytes(g.ToByteArray());
        }

        private void WriteBytes(byte[] bytes)
        {
            WriteStringFast(Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None));
        }

        private void WriteDateTime(DateTime dateTime)
        {
            // datetime format standard : yyyy-MM-dd HH:mm:ss

            _output.Append("\"");
            _output.Append(dateTime.Year.ToString("0000", NumberFormatInfo.InvariantInfo));
            _output.Append("-");
            _output.Append(dateTime.Month.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append("-");
            _output.Append(dateTime.Day.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(" ");
            _output.Append(dateTime.Hour.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(":");
            _output.Append(dateTime.Minute.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(":");
            _output.Append(dateTime.Second.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append("\"");
        }

        private DatasetSchema GetSchema(DataSet ds)
        {
            if (ds == null) return null;

            DatasetSchema m = new DatasetSchema();
            m.Info = new List<string>();
            m.Name = ds.DataSetName;

            foreach (DataTable t in ds.Tables)
            {
                foreach (DataColumn c in t.Columns)
                {
                    m.Info.Add(t.TableName);
                    m.Info.Add(c.ColumnName);
                    m.Info.Add(c.DataType.ToString());
                }
            }
            // TODO : serialize relations and constraints here

            return m;
        }

        private void WriteDataset(DataSet ds)
        {
            _output.Append('{');
            if (useExtension)
            {
                WritePair("$schema", useMinimalDataSetSchema ? (object)GetSchema(ds) : ds.GetXmlSchema());
                _output.Append(',');
            }
            foreach (DataTable table in ds.Tables)
            {
                _output.Append('\"');
                _output.Append(table.TableName);
                _output.Append("\":[");
                DataColumnCollection cols = table.Columns;
                foreach (DataRow row in table.Rows)
                {
                    _output.Append('[');

                    bool pendingSeperator = false;
                    foreach (DataColumn column in cols)
                    {
                        if (pendingSeperator) _output.Append(',');
                        WriteValue(row[column]);
                        pendingSeperator = true;
                    }
                    _output.Append(']');
                }

                _output.Append(']');
            }
            // end dataset
            _output.Append('}');
        }

        private void WriteObject(object obj)
        {
            _current_depth++;
            if (_current_depth > _MAX_DEPTH)
                throw new Exception("Serializer encountered maximum depth of " + _MAX_DEPTH);

            _output.Append('{');
            Dictionary<string, string> map = new Dictionary<string, string>();
            Type t = obj.GetType();
            bool append = false;
            if (useExtension)
            {
                WritePairFast("$type", JSON.Instance.GetTypeAssemblyName(t));
                append = true;
            }

            List<Getters> g = JSON.Instance.GetGetters(t);
            foreach (var p in g)
            {
                if (append)
                    _output.Append(',');
                object o = p.Getter(obj);
                WritePair(p.Name, o);
                if (o != null && useExtension)
                {
                	Type tt = o.GetType();
                	if (tt == typeof(System.Object))
                        map.Add(p.Name, tt.ToString());
                }
                append = true;
            }
            if (map.Count > 0 && useExtension)
            {
                _output.Append(",\"$map\":");
                WriteStringDictionary(map);
            }
            _output.Append('}');
            _current_depth--;
        }

        private void WritePairFast(string name, string value)
        {
            WriteStringFast(name);

            _output.Append(':');

            WriteStringFast(value);
        }

        private void WritePair(string name, object value)
        {
            WriteStringFast(name);

            _output.Append(':');

            WriteValue(value);
        }

        private void WriteArray(IEnumerable array)
        {
            _output.Append('[');

            bool pendingSeperator = false;

            foreach (object obj in array)
            {
                if (pendingSeperator) _output.Append(',');

                WriteValue(obj);

                pendingSeperator = true;
            }

            _output.Append(']');
        }

        private void WriteStringDictionary(IDictionary dic)
        {
            _output.Append('{');

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) _output.Append(',');

                WritePairFast((string)entry.Key, (string)entry.Value);

                pendingSeparator = true;
            }

            _output.Append('}');
        }

        private void WriteDictionary(IDictionary dic)
        {
            _output.Append('[');

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) _output.Append(',');

                _output.Append('{');
                WritePair("k", entry.Key);
                _output.Append(",");
                WritePair("v", entry.Value);
                _output.Append('}');

                pendingSeparator = true;
            }
            _output.Append(']');
        }

        private void WriteStringFast(string s)
        {
            _output.Append('\"');
            _output.Append(s);
            _output.Append('\"');
        }

        private void WriteString(string s)
        {
            _output.Append('\"');

            int runIndex = -1;

            for (var index = 0; index < s.Length; ++index)
            {
                var c = s[index];

                if (c >= ' ' && c < 128 && c != '\"' && c != '\\')
                {
                    if (runIndex == -1)
                    {
                        runIndex = index;
                    }

                    continue;
                }

                if (runIndex != -1)
                {
                    _output.Append(s, runIndex, index - runIndex);
                    runIndex = -1;
                }

                switch (c)
                {
                    case '\t': _output.Append("\\t"); break;
                    case '\r': _output.Append("\\r"); break;
                    case '\n': _output.Append("\\n"); break;
                    case '"':
                    case '\\': _output.Append('\\'); _output.Append(c); break;
                    default:
                        _output.Append("\\u");
                        _output.Append(((int)c).ToString("X4", NumberFormatInfo.InvariantInfo));
                        break;
                }
            }

            if (runIndex != -1)
            {
                _output.Append(s, runIndex, s.Length - runIndex);
            }

            _output.Append('\"');
        }
    }
}
