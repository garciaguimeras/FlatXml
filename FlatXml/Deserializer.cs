using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

using FlatXml.FXml;
using System.CodeDom;

namespace FlatXml
{
	public class Deserializer
	{
		List<Error> errors;

		public IEnumerable<Error> Errors { get { return errors; } }

		public Deserializer()
		{
			errors = null;
		}

		public FXmlDocument Deserialize(Stream stream)
		{
			errors = null;

			List<string> lines = new List<string>();
			using (StreamReader reader = new StreamReader(stream))
			{
				while (!reader.EndOfStream)
					lines.Add(reader.ReadLine());
			}

			Preprocessor preprocessor = new Preprocessor();
			string code = preprocessor.FixString(lines);

			TokenParser tokenParser = new TokenParser();
			bool result = tokenParser.Parse(code);
			if (!result)
			{
				errors = new List<Error>();
				errors.Add(tokenParser.Error);
				return null;
			}

			LLParser llParser = new LLParser();
			result = llParser.Parse(tokenParser.Tokens);
			if (!result)
			{
				errors = new List<Error>();
				errors.AddRange(llParser.Errors);
				return null;
			}

			FXmlDocument document = new FXmlDocument();
			document.Nodes.AddRange(llParser.FXmlNodes);
			return document;
		}

		public object Deserialize(Stream stream, Type type)
		{
			FXmlDocument document = Deserialize(stream);
			// TODO: convert from document to type instance
			return null;
		}
	}
}