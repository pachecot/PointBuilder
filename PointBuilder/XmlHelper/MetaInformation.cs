using System;
using System.Xml.Linq;

namespace PointBuilder.XmlHelper
{
    public class MetaInformation
    {
        const string TagName = "MetaInformation";

        /* sample:

            <MetaInformation>
                <ExportMode Value="Special" />
                <RuntimeVersion Value="2.0.4.83"/>
                <SourceVersion Value="2.0.4.83" />
                <ServerFullPath Value="/LockeDr251_AS01"/>
            </MetaInformation>
        */

        public string ExportMode { get; set; } = string.Empty;
        public string RuntimeVersion { get; set; } = string.Empty;
        public string SourceVersion { get; set; } = string.Empty;
        public string ServerFullPath { get; set; } = string.Empty;

        const string TagExportMode = "ExportMode";
        const string TagRuntimeVersion = "RuntimeVersion";
        const string TagSourceVersion = "SourceVersion";
        const string TagServerFullPath = "ServerFullPath";

        public MetaInformation()
        {
        }

        public MetaInformation(XElement element)
        {
            if (element.Name != TagName) { 
                throw new ArgumentException("expected TagName MetaInformation element"); 
            }

            foreach (var e in element.Elements())
            {
                var v = e.Attribute("Value")?.Value;
                if (v == null) { continue; }
                switch (e.Name.LocalName)
                {
                    case "ExportMode":
                        ExportMode = v;
                        break;
                    case "RuntimeVersion":
                        RuntimeVersion = v;
                        break;
                    case "SourceVersion":
                        SourceVersion = v;
                        break;
                    case "ServerFullPath":
                        ServerFullPath = v;
                        break;
                }
            }
        }

        static public MetaInformation From(XElement parent)
        {
            if (parent.Name == TagName)
            {
                return new MetaInformation(parent);
            }

            var e = parent.Element(TagName) ?? throw new ArgumentException("");
            return new MetaInformation(e);
        }

        public XElement XElement()
        {
            return new XElement(

                TagName,
                new XElement(TagExportMode, new XAttribute("Value", ExportMode)),
                new XElement(TagRuntimeVersion, new XAttribute("Value", RuntimeVersion)),
                new XElement(TagSourceVersion, new XAttribute("Value", SourceVersion)),
                new XElement(TagServerFullPath, new XAttribute("Value", ServerFullPath)));
        }
    }
}
