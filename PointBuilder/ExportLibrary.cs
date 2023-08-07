using PointBuilder.Export;
using PointBuilder.XmlHelper;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace PointBuilder
{
    internal class ExportLibrary
    {
        public readonly string LibPath;

        private ObjectSet objectSet;

        private XDocument emptyDoc;

        public ImmutableList<ObjectInstance> Points { get; }

        public ExportLibrary()
        {
            LibPath = "";
            objectSet = new ObjectSet();
            emptyDoc = new XDocument();
            Points = ImmutableList<ObjectInstance>.Empty;
        }

        public ExportLibrary(string uri)
        {
            if (!File.Exists(uri))
            {
                throw new ArgumentException("invalid file");
            }
            XDocument doc = XDocument.Load(uri);
            if (doc.Root == null)
            {
                throw new ArgumentException("invalid file");
            }
            LibPath = uri;
            XElement root = doc.Root;
            objectSet = new ObjectSet(root);
            emptyDoc = new XDocument(doc);
            (emptyDoc.Root?.Element("Types"))?.RemoveNodes();
            emptyDoc.Root?.Element("ExportedObjects")?.RemoveNodes();
            IEnumerable<ObjectInstance> names = objectSet.ExportedObjects.Select((XElement e) => new ObjectInstance(e));
            Points = names.ToImmutableList();
        }

        public Dictionary<string, XElement> GetTypesMap()
        {
            IEnumerable<(string, XElement)> nts = from t in objectSet.Types
                                                  let n = t.Attribute("Name")?.Value
                                                  where n != null
                                                  select (n, t);
            return nts.ToDictionary(((string n, XElement t) nt) => nt.n, ((string n, XElement t) nt) => nt.t);
        }

        public XDocument EmptyExport()
        {
            return new XDocument(emptyDoc);
        }
    }
}

