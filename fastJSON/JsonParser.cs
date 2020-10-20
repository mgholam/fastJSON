using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace fastJSON
{

    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    /// </summary>
    internal sealed class JsonParser
    {
        enum Token
        {
            None = -1,           // Used to denote no Lookahead available
            Curly_Open,
            Curly_Close,
            Squared_Open,
            Squared_Close,
            Colon,
            Comma,
            String,
            Number,
            True,
            False,
            Null//, 
            //Key
        }

        // slower than StringBuilder
        //class myStringBuilder
        //{

        //    int _index = 0;
        //    char[] _str = new char[32*1024];

        //    public void Append(char c)
        //    {
        //        _str[_index++] = c;
        //    }

        //    public void Clear()
        //    {
        //        _index = 0;
        //    }

        //    public override string ToString()
        //    {
        //        return new string(_str, 0, _index - 1);
        //    }
        //}

        readonly char[] json;
        readonly StringBuilder s = new StringBuilder(); // used for inner string parsing " \"\r\n\u1234\'\t " 
        //readonly myStringBuilder s = new myStringBuilder(); 
        Token lookAheadToken = Token.None;
        int index;
        bool allownonquotedkey = false;
        int _len = 0;
        SafeDictionary<string, bool> _lookup;
        bool _parseJsonType = false;

        internal JsonParser(string json, bool AllowNonQuotedKeys)
        {
            this.allownonquotedkey = AllowNonQuotedKeys;
            this.json = json.ToCharArray();
            _len = json.Length;
        }

        private void SetupLookup()
        {
            _lookup = new SafeDictionary<string, bool>();
            _lookup.Add("$types", true);
            _lookup.Add("$type", true);
            _lookup.Add("$i", true);
            _lookup.Add("$map", true);
            _lookup.Add("$schema", true);
            _lookup.Add("k", true);
            _lookup.Add("v", true);
        }

        public unsafe object Decode(Type objtype)
        {
            fixed (char* p = json)
            {
                if (objtype != null)
                {
                    if (CheckForTypeInJson(p) == false)
                    {
                        _parseJsonType = true;
                        SetupLookup();

                        BuildLookup(objtype);

                        // reset if no properties found
                        if (_lookup.Count() == 7)
                            _lookup = null;
                    }
                }
                return ParseValue(p);
            }
        }

        private unsafe bool CheckForTypeInJson(char* p)
        {
            int idx = 0;
            int len = _len > 1000 ? 1000 : _len;
            while (idx < len)
            {
                if (p[idx + 0] == '$' &&
                    p[idx + 1] == 't' &&
                    p[idx + 2] == 'y' &&
                    p[idx + 3] == 'p' &&
                    p[idx + 4] == 'e' &&
                    p[idx + 5] == 's'
                    )
                    return true;
                idx++;
            }

            return false;
        }

        private void BuildLookup(Type objtype)
        {
            // build lookup
            if (objtype == null)
                return;

            if (objtype == typeof(NameValueCollection) || objtype == typeof(StringDictionary))
                return;

            //if (objtype == typeof(DataSet) || objtype == typeof(DataTable)) 
            //    return;

            if (typeof(IDictionary).IsAssignableFrom(objtype))
                return;

            if (objtype.IsGenericType)
            {
                foreach (var e in objtype.GetGenericArguments())
                {
                    if (e.IsPrimitive)
                        continue;

                    bool isstruct = e.IsValueType && !e.IsEnum;

                    if ((e.IsClass || isstruct || e.IsAbstract) && e != typeof(string) && e != typeof(DateTime) && e != typeof(Guid))
                    {
                        BuildLookup(e);
                    }
                }
            }

            else if (objtype.IsArray)
            {
                Type t = objtype;
                bool isstruct = t.IsValueType && !t.IsEnum;

                if ((t.IsClass || isstruct) && t != typeof(string) && t != typeof(DateTime) && t != typeof(Guid))
                {
                    BuildLookup(t.GetElementType());
                }
            }
            else
            {
                foreach (var m in Reflection.Instance.Getproperties(objtype, objtype.FullName, true))
                {
                    Type t = m.Value.pt;

                    _lookup.Add(m.Key, true);

                    // t = class

                    bool isstruct = t.IsValueType && !t.IsEnum;

                    if (t.IsArray)
                    {
                        if ((t.IsClass || isstruct) && t != typeof(string) && t != typeof(DateTime) && t != typeof(Guid))
                        {
                            BuildLookup(t.GetElementType());
                        }
                    }


                    if (t.IsGenericType)
                    {
                        // t = list
                        foreach (var e in t.GetGenericArguments())
                        {
                            if (e.IsPrimitive)
                                continue;

                            isstruct = e.IsValueType && !e.IsEnum;

                            if ((e.IsClass || isstruct || e.IsAbstract) && e != typeof(string) && e != typeof(DateTime) && e != typeof(Guid))
                            {
                                BuildLookup(e);
                            }
                        }

                        // t = dictionary
                    }
                }
            }
        }

        private bool InLookup(string name)
        {
            if (_lookup == null)
                return true;

            return _lookup.TryGetValue(name.ToLowerInvariant(), out bool v);
        }

        bool _parseType = false;
        private unsafe Dictionary<string, object> ParseObject(char* p)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();

            ConsumeToken(); // {

            while (true)
            {
                switch (LookAhead(p))
                {

                    case Token.Comma:
                        ConsumeToken();
                        break;

                    case Token.Curly_Close:
                        ConsumeToken();
                        return obj;

                    default:
                        // name
                        string name = ParseKey(p);

                        var n = NextToken(p);
                        // :
                        if (n != Token.Colon)
                        {
                            throw new Exception("Expected colon at index " + index);
                        }

                        if (_parseJsonType)
                        {
                            if (name == "$types")
                            {
                                _parseType = true;
                                Dictionary<string, object> types = (Dictionary<string, object>)ParseValue(p);
                                _parseType = false;
                                // parse $types 
                                // FIX : performance hit here
                                if (_lookup == null)
                                    SetupLookup();

                                foreach (var v in types.Keys)
                                    BuildLookup(Reflection.Instance.GetTypeFromCache(v, true));

                                obj[name] = types;

                                break;
                            }

                            if (name == "$schema")
                            {
                                _parseType = true;
                                var value = ParseValue(p);
                                _parseType = false;
                                obj[name] = value;
                                break;
                            }

                            if (_parseType || InLookup(name))
                                obj[name] = ParseValue(p);
                            else
                                SkipValue(p);
                        }
                        else
                        {
                            obj[name] = ParseValue(p);
                        }
                        break;
                        //}
                }
            }
        }

        private unsafe void SkipValue(char* p)
        {
            // optimize skipping
            switch (LookAhead(p))
            {
                case Token.Number:
                    ParseNumber(p, true);
                    break;

                case Token.String:
                    SkipString(p);
                    break;

                case Token.Curly_Open:
                    SkipObject(p);
                    break;

                case Token.Squared_Open:
                    SkipArray(p);
                    break;

                case Token.True:
                    ConsumeToken();
                    break;

                case Token.False:
                    ConsumeToken();
                    break;

                case Token.Null:
                    ConsumeToken();
                    break;
            }
        }

        private unsafe void SkipObject(char* p)
        {
            ConsumeToken(); // {

            while (true)
            {
                switch (LookAhead(p))
                {

                    case Token.Comma:
                        ConsumeToken();
                        break;

                    case Token.Curly_Close:
                        ConsumeToken();
                        return;

                    default:
                        // name
                        SkipString(p);

                        var n = NextToken(p);
                        // :
                        if (n != Token.Colon)
                        {
                            throw new Exception("Expected colon at index " + index);
                        }
                        SkipValue(p);
                        break;
                }
            }
        }

        private unsafe void SkipArray(char* p)
        {
            ConsumeToken(); // [

            while (true)
            {
                switch (LookAhead(p))
                {
                    case Token.Comma:
                        ConsumeToken();
                        break;

                    case Token.Squared_Close:
                        ConsumeToken();
                        return;

                    default:
                        SkipValue(p);
                        break;
                }
            }
        }

        private unsafe void SkipString(char* p)
        {
            ConsumeToken();

            int len = _len;

            // escaped string
            while (index < len)
            {
                var c = p[index++];
                if (c == '"')
                    return;

                if (c == '\\')
                {
                    c = p[index++];

                    if (c == 'u')
                        index += 4;
                }
            }
        }

        private unsafe List<object> ParseArray(char* p)
        {
            List<object> array = new List<object>();
            ConsumeToken(); // [

            while (true)
            {
                switch (LookAhead(p))
                {
                    case Token.Comma:
                        ConsumeToken();
                        break;

                    case Token.Squared_Close:
                        ConsumeToken();
                        return array;

                    default:
                        array.Add(ParseValue(p));
                        break;
                }
            }
        }

        private unsafe object ParseValue(char* p)//, bool val)
        {
            switch (LookAhead(p))
            {
                case Token.Number:
                    return ParseNumber(p, false);

                case Token.String:
                    return ParseString(p);

                case Token.Curly_Open:
                    return ParseObject(p);

                case Token.Squared_Open:
                    return ParseArray(p);

                case Token.True:
                    ConsumeToken();
                    return true;

                case Token.False:
                    ConsumeToken();
                    return false;

                case Token.Null:
                    ConsumeToken();
                    return null;
            }

            throw new Exception("Unrecognized token at index " + index);
        }

        private unsafe string ParseKey(char* p)
        {
            if (allownonquotedkey == false || p[index - 1] == '"')
                return ParseString(p);

            ConsumeToken();

            int len = _len;
            int run = 0;
            while (index + run < len)
            {
                var c = p[index + run++];

                if (c == ':')
                {
                    var str = UnsafeSubstring(p, index, run - 1).Trim();
                    index += run - 1;
                    return str;
                }
            }
            throw new Exception("Unable to read key");
        }

        private unsafe string ParseString(char* p)
        {
            ConsumeToken();

            if (s.Length > 0)
                s.Length = 0;

            int len = _len;
            int run = 0;

            // non escaped string
            while (index + run < len)
            {
                var c = p[index + run++];
                if (c == '\\')
                    break;
                if (c == '\"')
                {
                    var str = UnsafeSubstring(p, index, run - 1);
                    index += run;
                    return str;
                }
            }

            // escaped string
            while (index < len)
            {
                var c = p[index++];
                if (c == '"')
                    return s.ToString();

                if (c != '\\')
                    s.Append(c);
                else
                    switch (p[index++])
                    {
                        case '"':
                            s.Append('"');
                            break;

                        case '\\':
                            s.Append('\\');
                            break;

                        case '/':
                            s.Append('/');
                            break;

                        case 'b':
                            s.Append('\b');
                            break;

                        case 'f':
                            s.Append('\f');
                            break;

                        case 'n':
                            s.Append('\n');
                            break;

                        case 'r':
                            s.Append('\r');
                            break;

                        case 't':
                            s.Append('\t');
                            break;

                        case 'u':
                            {
                                //int remainingLength = l - index;
                                //if (remainingLength < 4) break;

                                // parse the 32 bit hex into an integer codepoint
                                uint codePoint = ParseUnicode(p[index], p[index + 1], p[index + 2], p[index + 3]);
                                s.Append((char)codePoint);

                                // skip 4 chars
                                index += 4;
                            }
                            break;
                    }
            }


            return s.ToString();
        }

        //private unsafe string ParseKey(char* p)
        //{
        //    var c = p[index];
        //    if (c == '"')
        //        return ParseString(p);

        //    else if (allownonquotedkey == false)
        //        throw new Exception("Expecting a double quoted key and AllowNonQuotedKey is disabled");

        //    ConsumeToken();

        //    int run = 0;
        //    int l = _len;

        //    while (index + run < l)
        //    {
        //        c = p[index + run++];

        //        if (c == ':' || c == ' ' || c == '\t')
        //        {
        //            var s = UnsafeSubstring(p, index, (run - 1));
        //            index += run - 1;
        //            return s;
        //        }
        //    }

        //    throw new Exception("Unexpectedly reached end of string");
        //}

        private uint ParseSingleChar(char c1, uint multipliyer)
        {
            uint p1 = 0;
            if (c1 >= '0' && c1 <= '9')
                p1 = (uint)(c1 - '0') * multipliyer;
            else if (c1 >= 'A' && c1 <= 'F')
                p1 = (uint)((c1 - 'A') + 10) * multipliyer;
            else if (c1 >= 'a' && c1 <= 'f')
                p1 = (uint)((c1 - 'a') + 10) * multipliyer;
            return p1;
        }

        private uint ParseUnicode(char c1, char c2, char c3, char c4)
        {
            uint p1 = ParseSingleChar(c1, 0x1000);
            uint p2 = ParseSingleChar(c2, 0x100);
            uint p3 = ParseSingleChar(c3, 0x10);
            uint p4 = ParseSingleChar(c4, 1);

            return p1 + p2 + p3 + p4;
        }

        private unsafe object ParseNumber(char* p, bool skip)
        {
            ConsumeToken();

            // Need to start back one place because the first digit is also a token and would have been consumed
            var startIndex = index - 1;
            bool dec = false;
            bool dob = false;
            bool run = true;
            do
            {
                if (index == _len)
                    break;
                var c = p[index];

                //if ((c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E')
                //{
                //    if (/*c == '.' ||*/ c == 'e' || c == 'E')
                //        dob = true;
                //    if (c == '.')
                //        dec = true;
                //    if (++index == _len)
                //        break;//throw new Exception("Unexpected end of string whilst parsing number");
                //    continue;
                //}
                //run = false;// break;

                switch (c)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                    case '+':
                        index++;
                        break;
                    case 'e':
                    case 'E':
                        dob = true;
                        index++;
                        break;
                    case '.':
                        index++;
                        dec = true;
                        break;
                    default:
                        run = false;
                        break;
                }

                if (index == _len)
                    run = false;

            } while (run);

            if (skip)
                return 0;

            if (dob)
            {
                string s = UnsafeSubstring(p, startIndex, index - startIndex);// json.Substring(startIndex, index - startIndex);
                return double.Parse(s, NumberFormatInfo.InvariantInfo);
            }
            if (dec == false && index - startIndex < 20)// && json[startIndex] != '-')
                return Helper.CreateLong(json, startIndex, index - startIndex);
            else
            {
                string s = UnsafeSubstring(p, startIndex, index - startIndex);//json.Substring(startIndex, index - startIndex);
                return //Helper.ParseDecimal(s);
                   decimal.Parse(s, NumberFormatInfo.InvariantInfo);
            }
        }

        private unsafe Token LookAhead(char* p)
        {
            if (lookAheadToken != Token.None) return lookAheadToken;

            return lookAheadToken = NextTokenCore(p);
        }

        private void ConsumeToken()
        {
            lookAheadToken = Token.None;
        }

        private unsafe Token NextToken(char* p)
        {
            var result = lookAheadToken != Token.None ? lookAheadToken : NextTokenCore(p);

            lookAheadToken = Token.None;

            return result;
        }

        private unsafe void SkipWhitespace(char* p)
        {
            // Skip past whitespace
            do
            {
                var c = p[index];

                if (c == '/' && p[index + 1] == '/') // c++ style single line comments
                {
                    index++;
                    index++;
                    do
                    {
                        c = p[index];
                        if (c == '\r' || c == '\n') break; // read till end of line
                    }
                    while (++index < _len);
                }
                if (c != ' ' && c != '\t' && c != '\n' && c != '\r')
                    break;
                //switch (c)
                //{
                //    case ' ':
                //    case '\t':
                //    case '\r':
                //    case '\n':
                //        break;
                //    default:
                //        return;
                //}
            } while (++index < _len);
        }

        private unsafe Token NextTokenCore(char* p)
        {
            char c;
            int len = _len;

            SkipWhitespace(p);

            if (index == len)
            {
                throw new Exception("Reached end of string unexpectedly");
            }

            c = p[index];

            index++;

            switch (c)
            {
                case '{':
                    return Token.Curly_Open;

                case '}':
                    return Token.Curly_Close;

                case '[':
                    return Token.Squared_Open;

                case ']':
                    return Token.Squared_Close;

                case ',':
                    return Token.Comma;

                case '"':
                    return Token.String;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                case '+':
                case '.':
                    return Token.Number;

                case ':':
                    return Token.Colon;

                case 'f':
                    if (len - index >= 4 &&
                        p[index + 0] == 'a' &&
                        p[index + 1] == 'l' &&
                        p[index + 2] == 's' &&
                        p[index + 3] == 'e')
                    {
                        index += 4;
                        return Token.False;
                    }
                    break;

                case 't':
                    if (len - index >= 3 &&
                        p[index + 0] == 'r' &&
                        p[index + 1] == 'u' &&
                        p[index + 2] == 'e')
                    {
                        index += 3;
                        return Token.True;
                    }
                    break;

                case 'n':
                    if (len - index >= 3 &&
                        p[index + 0] == 'u' &&
                        p[index + 1] == 'l' &&
                        p[index + 2] == 'l')
                    {
                        index += 3;
                        return Token.Null;
                    }
                    break;
            }

            if (allownonquotedkey)//&& tok == Token.String)
            {
                index--;
                return Token.String;
            }

            //return tok;
            else
                throw new Exception("Could not find token at index " + --index);
        }

        private static unsafe string UnsafeSubstring(char* p, int startIndex, int length)
        {
            return new string(p, startIndex, length);
        }
    }
}
