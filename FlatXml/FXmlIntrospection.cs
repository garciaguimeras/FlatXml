using System;
using System.Reflection;
using System.Collections.Generic;

namespace FlatXml
{
	class FXmlIntrospectionException : Exception
	{
		public FXmlIntrospectionException(Exception e) : base(e.Message, e)
		{}
	}
		

	class FXmlElementMemberDescriptor
	{
		public MemberInfo Member { get; set; }
		public Type Type { get; set; }
		public string Name { get; set; }
	}

	class FXmlElementDescriptor
	{
		
	}

	class FXmlIntrospection
	{

		private static FXmlElementMemberDescriptor GetMemberDescriptor(MemberInfo info)
		{
			IEnumerable<CustomAttributeData> attrs = info.CustomAttributes;
			foreach (CustomAttributeData attrData in attrs)
			{
				if (attrData.AttributeType == typeof(FXmlNodeAttribute) ||
					attrData.AttributeType == typeof(FXmlAttributeAttribute))
				{
					foreach (CustomAttributeNamedArgument arg in attrData.NamedArguments)
					{
						if (arg.MemberName.Equals("Name"))
							return new FXmlElementMemberDescriptor 
							{
								Member = info,
								Type = attrData.AttributeType,
								Name = arg.TypedValue.Value.ToString(),
							};
					}
				}
			}
			return null;
		}

		public static FXmlElementDescriptor GetDescriptor(Type type)
		{
			try
			{
				object instance = type.GetConstructor(new Type[] { }).Invoke(new object[] { });

				MemberInfo[] members = type.GetMembers();
				foreach (MemberInfo info in members)
				{
					FXmlElementMemberDescriptor memberDesc = GetMemberDescriptor(info);
				}

				return null;
			}
			catch (Exception e)
			{
				throw new FXmlIntrospectionException(e);
			}
		}

	}
}

