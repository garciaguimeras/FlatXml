using System;

namespace FlatXml
{
	
	[AttributeUsage(AttributeTargets.Property)]
	public class FXmlNodeAttribute : Attribute
	{
		public string Name { get; set; }
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class FXmlAttributeAttribute : Attribute
	{
		public string Name { get; set; }
	}

}

