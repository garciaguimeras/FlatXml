using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatXml.FXml
{
	public class FXmlElement
	{
		public string Name { get; set; }
		public Dictionary<string, string> FXmlAttributes { get; protected set; }
		public List<FXmlElement> Children { get; protected set; }

		public FXmlElement()
		{
			Name = "";
			FXmlAttributes = new Dictionary<string, string>();
			Children = new List<FXmlElement>();
		}
	}
}

