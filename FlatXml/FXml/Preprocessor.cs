using System;
using System.Collections.Generic;

namespace FlatXml.FXml
{
	public class Preprocessor
	{

		private string ProcessMetaAttributeMacro(string line)
		{
			if (!line.StartsWith("@"))
				return line;

			string name = "";
			string value = "";

			line = line.Substring(1);
			int pos = line.IndexOf(" ");
			if (pos == -1)
				name = line;
			else
			{
				name = line.Substring(0, pos);
				value = line.Substring(pos + 1).Trim();
				value = value.Replace("\"", "'");
			}
			return string.Format("meta-attr name=\"{0}\" value=\"{1}\"", name, value);
		}

		public string FixString(List<string> lines)
		{
			string code = "";

			foreach (string line in lines)
			{
				string str = line.Trim();
				str = ProcessMetaAttributeMacro(str);
				str = str + TokenParser.NEWLINE;
				code = code + str;
			}

			code = code + " " + TokenParser.STOP;
			return code;
		}

	}
}

