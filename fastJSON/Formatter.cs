using System.Collections.Generic;
using System.Text;

namespace fastJSON
{
	internal class Formatter
	{
		#region class members
		const string Space = " ";
		const int DefaultIndent = 0;
		const string Indent = Space + Space + Space + Space;
		static readonly string NewLine = "\r\n";
		#endregion

		private enum JsonContextType
		{
			Object, Array
		}

		static void BuildIndents(int indents, StringBuilder output)
		{
			indents += DefaultIndent;
			for (; indents > 0; indents--)
				output.Append(Indent);
		}


		bool inDoubleString = false;
		bool inSingleString = false;
		bool inVariableAssignment = false;
		char prevChar = '\0';

		Stack<JsonContextType> context = new Stack<JsonContextType>();

		bool InString()
		{
			return inDoubleString || inSingleString;
		}

		public string PrettyPrint(string input)
		{
			var output = new StringBuilder(input.Length * 2);
			char c;

			for (int i = 0; i < input.Length; i++)
			{
				c = input[i];

				switch (c)
				{
					case '{':
						if (!InString())
						{
							if (inVariableAssignment || (context.Count > 0 && context.Peek() != JsonContextType.Array))
							{
								output.Append(NewLine);
								BuildIndents(context.Count, output);
							}
							output.Append(c);
							context.Push(JsonContextType.Object);
							output.Append(NewLine);
							BuildIndents(context.Count, output);
						}
						else
							output.Append(c);

						break;

					case '}':
						if (!InString())
						{
							output.Append(NewLine);
							context.Pop();
							BuildIndents(context.Count, output);
							output.Append(c);
						}
						else
							output.Append(c);

						break;

					case '[':
						output.Append(c);

						if (!InString())
							context.Push(JsonContextType.Array);

						break;

					case ']':
						if (!InString())
						{
							output.Append(c);
							context.Pop();
						}
						else
							output.Append(c);

						break;

					case '=':
						output.Append(c);
						break;

					case ',':
						output.Append(c);

						if (!InString() && context.Peek() != JsonContextType.Array)
						{
							BuildIndents(context.Count, output);
							output.Append(NewLine);
							BuildIndents(context.Count, output);
							inVariableAssignment = false;
						}

						break;

					case '\'':
						if (!inDoubleString && prevChar != '\\')
							inSingleString = !inSingleString;

						output.Append(c);
						break;

					case ':':
						if (!InString())
						{
							inVariableAssignment = true;
							output.Append(Space);
							output.Append(c);
							output.Append(Space);
						}
						else
							output.Append(c);

						break;

					case '"':
						if (!inSingleString && prevChar != '\\')
							inDoubleString = !inDoubleString;

						output.Append(c);
						break;
					case ' ':
						if (InString())
							output.Append(c);
						break;

					default:
						output.Append(c);
						break;
				}
				prevChar = c;
			}

			return output.ToString();
		}
	}
}