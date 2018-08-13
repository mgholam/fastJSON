using System;
using System.Collections.Generic;
using System.Globalization;
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

        readonly char[] json;
        readonly StringBuilder s = new StringBuilder(); // used for inner string parsing " \"\r\n\u1234\'\t " 
        Token lookAheadToken = Token.None;
        int index;
        bool allownonquotedkey = false;
        int _len = 0;

        internal JsonParser(string json, bool AllowNonQuotedKeys)
        {
            this.allownonquotedkey = AllowNonQuotedKeys;
            this.json = json.ToCharArray();
            _len = json.Length;
        }

        public unsafe object Decode()
        {
            fixed(char* p = json)
                return ParseValue(p, false);
        }

        private unsafe Dictionary<string, object> ParseObject(char* p)
        {
            Dictionary<string, object> table = new Dictionary<string, object>(10);

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
                        return table;

                    default:
                        {
                            // name
                            string name = ParseString(p, false);

                            var n = NextToken(p);
                            // :
                            if (n != Token.Colon)
                            {
                                throw new Exception("Expected colon at index " + index);
                            }

                            // value
                            object value = ParseValue(p, true);

                            //table.Add(name, value);
                            table[name] = value;
                        }
                        break;
                }
            }
        }

        private unsafe List<object> ParseArray(char* p)
        {
            List<object> array = new List<object>(10);
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
                        array.Add(ParseValue(p, false));
                        break;
                }
            }
        }

        private unsafe object ParseValue(char* p, bool val)
        {
            switch (LookAhead(p))
            {
                case Token.Number:
                    return ParseNumber(p);

                case Token.String:
                    return ParseString(p, val);

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

            throw new Exception("Unrecognized token at index" + index);
        }

        private unsafe string ParseString(char* p, bool val)
        {
            ConsumeToken(); // "

            if (s.Length > 0)
                s.Length = 0;
            //s.Clear();
            bool instr = val;
            int runIndex = -1;
            int l = _len;
            //fixed (char* p = json)
            //char[] p = json;
            {
                while (index < l)
                {
                    var c = p[index++];
                    if (c == '"')
                        instr = true;

                    if (c == '"' || (allownonquotedkey && instr == false && (c == ':' || c == ' ' || c == '\t')))
                    {
                        int len = 1;
                        if (allownonquotedkey && c != '"' && instr == false)
                        {
                            index--;
                            len = 0;
                        }

                        if (runIndex != -1)
                        {
                            if (s.Length == 0)
                                return UnsafeSubstring(p, runIndex, index - runIndex - len);

                            s.Append(json, runIndex, index - runIndex - 1);
                        }
                        return s.ToString();
                    }

                    if (c != '\\')
                    {
                        if (runIndex == -1)
                            runIndex = index - 1;

                        continue;
                    }

                    if (index == l) break;

                    if (runIndex != -1)
                    {
                        s.Append(json, runIndex, index - runIndex - 1);
                        runIndex = -1;
                    }

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
                                int remainingLength = l - index;
                                if (remainingLength < 4) break;

                                // parse the 32 bit hex into an integer codepoint
                                uint codePoint = ParseUnicode(p[index], p[index + 1], p[index + 2], p[index + 3]);
                                s.Append((char)codePoint);

                                // skip 4 chars
                                index += 4;
                            }
                            break;
                    }
                }
            }

            throw new Exception("Unexpectedly reached end of string");
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

        private unsafe object ParseNumber(char* p)
        {
            ConsumeToken();

            // Need to start back one place because the first digit is also a token and would have been consumed
            var startIndex = index - 1;
            bool dec = false;
            bool dob = false;
            do
            {
                if (index == _len)
                    break;
                var c = json[index];

                if ((c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E')
                {
                    if (/*c == '.' ||*/ c == 'e' || c == 'E')
                        dob = true;
                    if (c == '.')
                        dec = true;
                    if (++index == _len)
                        break;//throw new Exception("Unexpected end of string whilst parsing number");
                    continue;
                }
                break;
            } while (true);

            if (dob)
            {
                string s = UnsafeSubstring(p, startIndex, index - startIndex);// json.Substring(startIndex, index - startIndex);
                return double.Parse(s, NumberFormatInfo.InvariantInfo);
            }
            if (dec == false && index - startIndex < 20 && json[startIndex] != '-')
                return Helper.CreateLong(json, startIndex, index - startIndex);
            else
            {
                string s = UnsafeSubstring(p, startIndex, index - startIndex);//json.Substring(startIndex, index - startIndex);
                return decimal.Parse(s, NumberFormatInfo.InvariantInfo);
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

        private unsafe Token NextTokenCore(char* p)
        {
            char c;
            int len = _len;

            // Skip past whitespace
            do
            {
                //fixed (char* p = json)
                {
                    c = p[index];

                    if (c == '/' && p[index + 1] == '/') // c++ style single line comments
                    {
                        index++;
                        index++;
                        do
                        {
                            c = p[index];
                            if (c == '\r' || c == '\n') break; // read till end of line
                        }
                        while (++index < len);
                    }
                    if (c > ' ') break;
                    if (c != ' ' && c != '\t' && c != '\n' && c != '\r') break;
                }
            } while (++index < len);

            if (index == len)
            {
                throw new Exception("Reached end of string unexpectedly");
            }

            //fixed (char* p = json)
            {
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
            }
            if (allownonquotedkey)
            {
                index--;
                return Token.String;
            }
            else
                throw new Exception("Could not find token at index " + --index);
        }

        private static unsafe string UnsafeSubstring(//char[] source, 
            char* p, int startIndex, int length)
        {
            //fixed (char* c = source)
                return new string(p, startIndex, length);
        }
    }
}
