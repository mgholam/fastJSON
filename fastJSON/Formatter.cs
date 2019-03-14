using System.Text;

namespace fastJSON
{
    internal static class Formatter
    {
        //private static string _indent = "   ";

        private static void AppendIndent(StringBuilder sb, int count, string indent)
        {
            for (; count > 0; --count) sb.Append(indent);
        }

        public static string PrettyPrint(string input)
        {
            return PrettyPrint(input, new string(' ', JSON.Parameters.FormatterIndentSpaces));// "   ");
        }

        public static string PrettyPrint(string input, string spaces)
        {
            //_indent = spaces;
            var output = new StringBuilder();
            int depth = 0;
            int len = input.Length;
            char[] chars = input.ToCharArray();
            for (int i = 0; i < len; ++i)
            {
                char ch = chars[i];

                if (ch == '\"') // found string span
                {
                    bool str = true;
                    while (str)
                    {
                        output.Append(ch);
                        ch = chars[++i];
                        if (ch == '\\')
                        {
                            output.Append(ch);
                            ch = chars[++i];
                        }
                        else if (ch == '\"')
                            str = false;
                    }
                }

                switch (ch)
                {
                    case '{':
                    case '[':
                        output.Append(ch);
                        output.AppendLine();
                        AppendIndent(output, ++depth, spaces);
                        break;
                    case '}':
                    case ']':
                        output.AppendLine();
                        AppendIndent(output, --depth, spaces);
                        output.Append(ch);
                        break;
                    case ',':
                        output.Append(ch);
                        output.AppendLine();
                        AppendIndent(output, depth, spaces);
                        break;
                    case ':':
                        output.Append(" : ");
                        break;
                    default:
                        if (!char.IsWhiteSpace(ch))
                            output.Append(ch);
                        break;
                }
            }

            return output.ToString();
        }
    }
}