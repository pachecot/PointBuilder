using PointBuilder.XmlHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace PointBuilder.XmlHelper
{
    public class ObjectSet
    {
        string ExportMode = "Standard"; // Standard, Library, Special
        string Note = "TypesFirst";
        string SemanticsFilter = "Standard"; // None, Direct, All
        string Version = "4.0.3.176";

        Dictionary<string, string> attr = new();
        Dictionary<string, XElement> _tm = new();
        Dictionary<int, List<string>> _tr = new();

        const string Tag_ObjectSet = "ObjectSet";
        public const string Tag_Types = "Types";
        public const string Tag_ExportedObjects = "ExportedObjects";

        List<XElement> _types;
        List<XElement> _exportedObjects;

        public MetaInformation MetaInformation { get; set; }
        public IEnumerable<XElement> Types { get => _types; }
        public IEnumerable<XElement> ExportedObjects { get => _exportedObjects; }

        public ObjectSet()
        {
            MetaInformation = new MetaInformation();
            _types = new List<XElement>();
            _exportedObjects = new List<XElement>();
        }

        public ObjectSet(XElement objectSet)
        {
            if (objectSet == null)
            {
                throw new ArgumentNullException("objectSet");
            }

            if (objectSet.Name != Tag_ObjectSet)
            {
                throw new ArgumentException($"received wrong element {objectSet.Name} expected {Tag_ObjectSet}.");
            }

            attr = objectSet.Attributes().ToDictionary(a => a.Name.LocalName, a => a.Value);

            MetaInformation = MetaInformation.From(objectSet);

            _types = GetElements(objectSet, Tag_Types);
            _tm = MapToName(_types);

            _exportedObjects = GetElements(objectSet, Tag_ExportedObjects);
            Dictionary<int, List<string>> tr = new();
            for (var i = 0; i < _exportedObjects.Count; i++)
            {
                XElement oi = _exportedObjects[i];
                var ns = GetTypeNames(oi).ToList();
                if (ns.Count > 0)
                {
                    tr.Add(i, ns);
                }
            }
        }


        static Dictionary<string, XElement> MapToName(IEnumerable<XElement> types)
        {
            var i = 0;
            var tm = types.ToDictionary(t => t.Attribute("Name")?.Value ?? ("id_" + ++i));
            return tm;
        }

        static IEnumerable<string> GetTypeNames(IEnumerable<XElement> elements)
        {
            var types = from e in elements
                        from n in GetTypeNames(e)
                        select n;

            return types.Distinct();
        }

        static IEnumerable<string> GetTypeNames(XElement element)
        {
            var types = from e in element.DescendantsAndSelf()
                        let a = e.Attribute("TYPE")
                        where a != null
                        select a.Value;

            return types.Distinct();
        }

        public void Append(ObjectSet set)
        {
            // merge distinct types
            _types = _types.Concat(set.Types)
                           .GroupBy(t => t.Name + (t.Attribute("Name")?.ToString() ?? string.Empty))
                           .Select(x => x.First())
                           .ToList();

            _exportedObjects = _exportedObjects.Concat(set.ExportedObjects.Elements())
                                               .OrderBy(x => x.Name)
                                               .ToList();
        }


        static XElement GetElementName(XElement objectSet, string name) => objectSet.Element(name) switch
        {
            XElement e => new XElement(e),
            _ => new XElement(name)
        };

        static List<XElement> GetElements(XElement objectSet, string name) => objectSet.Element(name) switch
        {
            XElement e => new XElement(e).Elements().ToList(),
            _ => new List<XElement>()
        };



    }
}
