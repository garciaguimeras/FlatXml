using System;
using System.Linq.Expressions;
using System.IO;
using System.Collections.Generic;
using System.Text;

using FlatXml.FXml;


namespace FlatXml
{
	public class Serializer
	{
		public Serializer()
		{}

		private void SerializeElement(FXmlNode element, StreamWriter writer, int level)
		{
			string indent = "";
			for (int i = 0; i < level; i++)
				indent += "    ";

			writer.Write("{0}{1} ", indent, element.Name);
			if (element.FXmlAttributes != null)
			{
				foreach (string key in element.FXmlAttributes.Keys)
				{
					writer.Write("{0}=\"{1}\" ", key, element.FXmlAttributes[key]);
				}
			}

			writer.WriteLine();

			if (element.Nodes != null && element.Nodes.Count > 0)
			{
				writer.Write(indent);
				writer.WriteLine("{");
				foreach (FXmlNode child in element.Nodes)
				{
					SerializeElement(child, writer, level + 1);
				}
				writer.Write(indent);
				writer.WriteLine("}");
			}
		}

		public Stream Serialize(IEnumerable<FXmlNode> elements)
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			foreach (FXmlNode element in elements)
			{
				SerializeElement(element, writer, 0);
			}
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		public Stream Serialize(Object baseNode)
		{
			// TODO: Convert baseNode to element list and use previous method
			return null;
		}

	}
}

