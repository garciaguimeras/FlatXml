using System;
using System.Collections.Generic;

namespace FlatXml.FXml
{
	public class Preprocessor
	{

		public string FixString(List<string> lines)
		{
			string code = "";

			foreach (string line in lines)
			{
				code = code + line.Trim() + TokenParser.NEWLINE;
			}
			code = code + " " + TokenParser.STOP;
			return code;
		}

	}
}

