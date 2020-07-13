using System;
using System.Xml;

namespace Engine.Shared
{
    public static class ExtensionMethods
    {
        public static int AttributeAsInt(this XmlNode node, string AttributeName) =>
            Convert.ToInt32(node.AttributeAsString(AttributeName));

        public static string AttributeAsString(this XmlNode node, string attributeName)
        {
            var attribute = node.Attributes?[attributeName];

            if (attribute == null)
            {
                throw new ArgumentException($"The attribute '{attributeName}' is missing.");
            }

            return attribute.Value;
        }
    }
}