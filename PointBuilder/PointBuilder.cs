using PointBuilder.Export;
using PointBuilder.XmlHelper;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace PointBuilder
{
    internal class Builder
    {
        ExportLibrary library = new();
        Dictionary<string, XElement> typeMap = new();

        ObservableCollection<ObjectInstance> _points = new ObservableCollection<ObjectInstance>();

        string? currrentFile;
        public string? File
        {
            get => currrentFile;
        }
        public Builder()
        {
            LoadLibraryTypes();
        }

        public string LibraryFile() { return library.LibPath; }

        public void LoadLibrary(string file)
        {
            library = new ExportLibrary(file);
            LoadLibraryTypes();
        }

        private void LoadLibraryTypes()
        {
            var tm = library.GetTypesMap();
            if (typeMap.Count == 0)
            {
                typeMap = tm;
                return;
            }
            foreach (var item in tm)
            {
                typeMap.TryAdd(item.Key, item.Value);
            }
        }

        public ImmutableList<ObjectInstance> LibraryPoints
        {
            get { return library.Points; }
        }
        public ObservableCollection<ObjectInstance> Points
        {
            get { return _points; }
        }

        public void Add(ObjectInstance oi)
        {
            oi = _points.GetName(oi);
            var pos = _points.FindPosition(oi);
            _points.Insert(pos, oi);
        }
        public void Add(IEnumerable<ObjectInstance> points)
        {
            foreach (var item in points)
            {
                Add(item);
            }
        }

        public void Load(string filename)
        {
            currrentFile = filename;
            var d = XDocument.Load(currrentFile);
            if (d.Root == null) { return; }
            var s = new ObjectSet(d.Root);

            _points.Clear();

            foreach (var item in s.ExportedObjects)
            {
                _points.Add(new ObjectInstance(item));
            }
        }

        public void SaveAs(string filename)
        {
            currrentFile = filename;
            Save();
        }

        public void Save()
        {
            if (currrentFile == null) { throw new InvalidOperationException("no curernt file, can not save file"); }

            var doc = library.EmptyExport();


            var ois = _points.Select(oi => oi.Element).ToArray();

            var ns = from oi in ois
                     from e in oi.DescendantsAndSelf()
                     from a in e.Attributes()
                     let n = a.Name.LocalName.ToLower()
                     where n == "type"
                     select a.Value;

            var ts = from t in ns.Distinct()
                     where typeMap.ContainsKey(t)
                     select typeMap[t];

            var types = doc.Root?.Element(ObjectSet.Tag_Types);
            if (types != null)
            {
                types.Add(ts.ToArray());
            }
            var objs = doc.Root?.Element(ObjectSet.Tag_ExportedObjects);
            if (objs != null)
            {
                objs.Add(ois);
            }
            doc.Save(currrentFile);
        }

        public void Update(ObjectInstance oldOI, ObjectInstance newOI)
        {
            _points.Remove(oldOI);
            Add(newOI);
        }

        public void Delete(ObjectInstance oi)
        {
            _points.Remove(oi);
        }

        public void Reset()
        {
            currrentFile = null;
            _points.Clear();
        }
    }
}

