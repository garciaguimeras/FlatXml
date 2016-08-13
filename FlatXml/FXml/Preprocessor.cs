using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FlatXml.FXml
{
	public class Preprocessor
	{

		// Processes <tag name="Name">Value</tag> macros
		private string ProcessMacro(string line, string macro, string tag)
		{
			if (!line.StartsWith(macro))
				return line;

			string name = "";
			string value = "";
			string str = line;

			str = str.Substring(macro.Length);
			if (line.Length == 0)
				return line;

			if (!TokenParser.LETTERS.Contains(str[0].ToString()))
				return line;

			int pos = str.IndexOf(" ");
			if (pos == -1)
				name = str;
			else
			{
				name = str.Substring(0, pos);
				value = str.Substring(pos + 1).Trim();
				value = value.Replace("\"", "'");
			}
			return string.Format("{0} name=\"{1}\" value=\"{2}\"", tag, name, value);
		}

		// @macro-attr-name macro-attr-value
		private string ProcessMetaAttributeMacro(string line)
		{
			return ProcessMacro(line, "@", "meta-attr");
		}

		// @property-name property-value
		private string ProcessPropertyMacro(string line)
		{
			return ProcessMacro(line, "$", "property");
		}

		public string FixString(List<string> lines)
		{
			string code = "";

			foreach (string line in lines)
			{
				string str = line.Trim();
				str = ProcessMetaAttributeMacro(str);
				str = ProcessPropertyMacro(str);
				str = str + TokenParser.NEWLINE;
				code = code + str;
			}

			code = code + " " + TokenParser.STOP;
			return code;
		}

	}
}

