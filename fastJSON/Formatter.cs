using System.Text;

namespace fastJSON
{
    internal static class Formatter
    {
        private const string Indent = "   ";

        private static void AppendIndent(StringBuilder sb, int count)
        {
            for (; count > 0; --count) sb.Append(Indent);
        }

        public static string PrettyPrint(string input)
        {
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
                        switch (ch)
                        {
                            case '\\':
                                output.Append(ch);
                                ch = chars[++i];
                                break;
                            case '\"':
                                str = false;
                                break;
                        }
                    }
                }

                switch (ch)
                {
                    case '{':
                    case '[':
                        output.Append(ch);
                        output.AppendLine();
                        AppendIndent(output, ++depth);
                        break;
                    case '}':
                    case ']':
                        output.AppendLine();
                        AppendIndent(output, --depth);
                        output.Append(ch);
                        break;
                    case ',':
                        output.Append(ch);
                        output.AppendLine();
                        AppendIndent(output, depth);
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