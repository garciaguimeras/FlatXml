using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatXml.FXml
{
	public abstract class FXmlElement
	{
		
		public List<FXmlNode> Nodes { get; protected set; }

		public IEnumerable<string> Names()
		{
			List<string> names = new List<string>();
			foreach (FXmlNode node in Nodes)
				if (!names.Contains(node.Name))
					names.Add(node.Name);
			return names;
		}

		public IEnumerable<FXmlNode> NodesByName(string name)
		{
			List<FXmlNode> nodes = new List<FXmlNode>();
			foreach (FXmlNode node in Nodes)
				if (node.Name.Equals(name))
					nodes.Add(node);
			return nodes;
		}

		public Dictionary<string, IEnumerable<FXmlNode>> NodesMap()
		{
			Dictionary<string, IEnumerable<FXmlNode>> map = new Dictionary<string, IEnumerable<FXmlNode>>();
			IEnumerable<string> names = Names();
			foreach (string name in names)
			{
				IEnumerable<FXmlNode> nodes = NodesByName(name);
				map.Add(name, nodes);
			}
			return map;
		}

	}

	public class FXmlNode : FXmlElement
	{
		public string Name { get; set; }
		public Dictionary<string, string> FXmlAttributes { get; protected set; }

		public FXmlNode()
		{
			Name = "";
			FXmlAttributes = new Dictionary<string, string>();
			Nodes = new List<FXmlNode>();
		}

	}

	public class FXmlDocument : FXmlElement
	{

		public string DocumentName { get; set; }

		public FXmlDocument()
		{
			DocumentName = "";
			Nodes = new List<FXmlNode>();
		}

	}
}

