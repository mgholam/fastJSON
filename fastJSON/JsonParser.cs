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
            ,
            PosInfinity,
            NegInfinity,
            NaN
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
        //bool AllowJson5String = false;
        int _len = 0;
        SafeDictionary<string, bool> _lookup;
        SafeDictionary<Type, bool> _seen;
        bool _parseJsonType = false;

        internal JsonParser(string json, bool AllowNonQuotedKeys)//, bool AllowJson5String)
        {
            allownonquotedkey = AllowNonQuotedKeys;
            //this.AllowJson5String = AllowJson5String;
            this.json = json.ToCharArray();
            _len = json.Length;
        }

        private void SetupLookup()
        {
            _lookup = new SafeDictionary<string, bool>();
            _seen = new SafeDictionary<Type, bool>();
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
                        if (_parseJsonType == false || _lookup.Count() == 7)
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

        private void BuildGenericTypeLookup(Type t)
        {
            if (_seen.TryGetValue(t, out bool _))
                return;

            foreach (var e in t.GetGenericArguments())
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

        private void BuildArrayTypeLookup(Type t)
        {
            if (_seen.TryGetValue(t, out bool _))
                return;

            bool isstruct = t.IsValueType && !t.IsEnum;

            if ((t.IsClass || isstruct) && t != typeof(string) && t != typeof(DateTime) && t != typeof(Guid))
            {
                BuildLookup(t.GetElementType());
            }
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

            if (_seen.TryGetValue(objtype, out bool _))
                return;

            if (objtype.IsGenericType)
                BuildGenericTypeLookup(objtype);

            else if (objtype.IsArray)
            {
                Type t = objtype;
                BuildArrayTypeLookup(objtype);
            }
            else
            {
                _seen.Add(objtype, true);

                foreach (var m in Reflection.Instance.Getproperties(objtype, objtype.FullName, true))
                {
                    Type t = m.Value.pt;

                    _lookup.Add(m.Key, true);

                    if (t.IsArray)
                        BuildArrayTypeLookup(t);

                    if (t.IsGenericType)
                    {
                        // skip if dictionary
                        if (typeof(IDictionary).IsAssignableFrom(t))
                        {
                            _parseJsonType = false;
                            return;
                        }
                        BuildGenericTypeLookup(t);
                    }
                    if (t.FullName.IndexOf("System.") == -1)
                        BuildLookup(t);
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
                                // performance hit here
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
                case Token.False:
                case Token.Null:
                case Token.PosInfinity:
                case Token.NegInfinity:
                case Token.NaN:
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

                case Token.PosInfinity:
                    ConsumeToken();
                    return double.PositiveInfinity;

                case Token.NegInfinity:
                    ConsumeToken();
                    return double.NegativeInfinity;

                case Token.NaN:
                    ConsumeToken();
                    return double.NaN;
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
            char quote = p[index - 1];
            ConsumeToken();

            if (s.Length > 0)
                s.Length = 0;

            //if (AllowJson5String)
            //    return ParseJson5String(p);

            int len = _len;
            int run = 0;

            // non escaped string
            while (index + run < len)
            {
                var c = p[index + run++];
                if (c == '\\')
                    break;
                if (c == quote)//'\"')
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
                if (c == quote)//'"')
                    return s.ToString();

                if (c != '\\')
                    s.Append(c);
                else
                {
                    c = p[index++];
                    switch (c)
                    {
                        //case '\'':
                        //    s.Append('\'');
                        //    break;
                        //case '"':
                        //    s.Append('"');
                        //    break;

                        //case '\\':
                        //    s.Append('\\');
                        //    break;

                        //case '/':
                        //    s.Append('/');
                        //    break;

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
                        default:
                            if (c == '\r' || c == '\n' || c == ' ' || c == '\t')
                            {
                                // json5 skip ending whitespace till new line
                                while (c == '\r' || c == '\n' || c == ' ' || c == '\t')
                                {
                                    index++;
                                    c = p[index];
                                    if (c == '\r' || c == '\n')
                                    {
                                        c = p[index + 1];
                                        if (c == '\r' || c == '\n')
                                        {
                                            index += 2;
                                            c = p[index];
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                                s.Append(c);
                            break;
                    }
                }
            }


            return s.ToString();
        }

        private unsafe string ParseJson5String(char* p)
        {
            throw new NotImplementedException();
        }

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
            if (p[startIndex] == '.') dec = true;
            do
            {
                if (index == _len)
                    break;
                var c = p[index];

                if (c == 'x' || c == 'X')
                {
                    index++;
                    return ReadHexNumber(p);
                }

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
                    case '+':
                    case '-':
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
                    case 'n':
                    case 'N':
                        index += 3;
                        return double.NaN;
                    //break;
                    default:
                        run = false;
                        break;
                }

                if (index == _len)
                    run = false;

            } while (run);

            if (skip)
                return 0;
            var len = index - startIndex;
            if (dob || len > 31)
            {
                string s = UnsafeSubstring(p, startIndex, len);
                return double.Parse(s, NumberFormatInfo.InvariantInfo);
            }
            if (dec == false && len < 20)
                return Helper.CreateLong(json, startIndex, len);
            else
            {
                string s = UnsafeSubstring(p, startIndex, len);
                return decimal.Parse(s, NumberFormatInfo.InvariantInfo);
            }
        }

        private unsafe object ReadHexNumber(char* p)
        {
            long num = 0L;
            bool run = true;
            while (run && index < _len)
            {
                char c = p[index];
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
                        index++;
                        num = (num << 4) + (c - '0');
                        break;
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                        index++;
                        num = (num << 4) + (c - 'a') + 10;
                        break;
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                        index++;
                        num = (num << 4) + (c - 'A') + 10;
                        break;
                    default:
                        run = false;
                        break;
                }
            }

            return num;
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

                if (c == '/' && p[index + 1] == '*') // c style multi line comments
                {
                    index++;
                    index++;
                    do
                    {
                        c = p[index];
                        if (c == '*' && p[index + 1] == '/')
                        {
                            index += 2;
                            c = p[index];
                            break; // read till end of comment
                        }
                    }
                    while (++index < _len);
                }

                if (c != ' ' && c != '\t' && c != '\n' && c != '\r')
                    break;
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

                case '\'':
                case '"':
                    return Token.String;

                case '-':
                    if (p[index] == 'i' || p[index] == 'I') // TODO : check all chars ??
                    {
                        index += 8;
                        return Token.NegInfinity;
                    }
                    return Token.Number;
                case '+':
                    if (p[index] == 'i' || p[index] == 'I') // TODO : check all chars ??
                    {
                        index += 8;
                        return Token.PosInfinity;
                    }
                    return Token.Number;

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
                case '.':
                    return Token.Number;

                case ':':
                    return Token.Colon;
                case 'I':
                case 'i': // TODO : check complete infinity chars
                    index += 7;
                    return Token.PosInfinity;

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
                case 'N':
                    if (len - index >= 3 &&
                        p[index + 0] == 'u' &&
                        p[index + 1] == 'l' &&
                        p[index + 2] == 'l')
                    {
                        index += 3;
                        return Token.Null;
                    }
                    else if (len - index >= 2 &&
                         p[index] == 'a' &&
                         (p[index + 1] == 'n' || p[index + 1] == 'N'))
                    {
                        index += 2;
                        return Token.NaN;
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
                throw new Exception("Could not find token at index " + --index + " got '" + p[index] + "'");
        }

        private static unsafe string UnsafeSubstring(char* p, int startIndex, int length)
        {
            return new string(p, startIndex, length);
        }
    }
}
