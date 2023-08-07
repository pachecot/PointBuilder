using System;
using System.Xml.Linq;

namespace PointBuilder.XmlHelper
{
    class ValueElement
    {
        public string Name { get; private set; }
        public string Value { get; set; }
        public ValueElement(string name, string value = "")
        {
            Name = name;
            Value = value;
        }
        public ValueElement(XElement element)
        {
            Name = element.Name.LocalName;
            Value = element.Attribute("Value")?.Value ?? throw new ArgumentException("element has no value attribute.");
        }

        public XElement GetXElement()
        {
            return new XElement(Name, new XAttribute("Value", Value));
        }

    }
}
