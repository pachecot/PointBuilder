using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using PointBuilder.XmlHelper;

namespace PointBuilder.Export
{

    public static class ObservableCollectionOfPointExtensions
    {
        public static int FindPosition(this ObservableCollection<ObjectInstance> list, ObjectInstance name)
        {
            int pos = 0;
            while (pos < list.Count && list[pos].Name.CompareTo(name.Name) < 0) { pos++; }
            return pos;
        }

        public static ObjectInstance GetName(this ObservableCollection<ObjectInstance> list, ObjectInstance point)
        {
            var count = 1;
            var newName = point.Name;
            while (list.Any(p => p.Name == newName))
            {
                count++;
                newName = $"{point.Name}_{count}";
            }
            point.Name = newName;
            return point;
        }
    }


    class ExportFile
    {
        string? currentFile;
        ExportLibrary library;

        ObjectSet s;

        ObservableCollection<ObjectInstance> _points = new ObservableCollection<ObjectInstance>();
        public ObservableCollection<ObjectInstance> Points { get => _points; }

        public ExportFile(ExportLibrary library)
        {
            this.library = library;
        }

        public void AddItem(ObjectInstance point)
        {
            point = _points.GetName(point);
            var pos = _points.FindPosition(point);
            _points.Insert(pos, point);
        }
        public void AddItems(IEnumerable<ObjectInstance> points)
        {
            foreach (var item in points)
            {
                AddItem(item);
            }
        }

        public void UpdateItem(ObjectInstance point, ObjectInstance newPoint)
        {
            _points.Remove(point);
            AddItem(newPoint);
        }

        public void RemoveItem(ObjectInstance item)
        {
            _points.Remove(item);
        }

        public void Reset()
        {
            _points.Clear();
        }

        public void Save(string fileName)
        {
            if (currentFile == null || !currentFile.Equals(fileName))
            {
                currentFile = fileName;
            }

            //var tm = _points.Templates()
            //                .ToDictionary(k => k, library.Elements);

            // generate the file
            // save the file
        }

        internal void Load(string file)
        {
            var d = XDocument.Load(file);
            if (d.Root == null) { return; }
            s = new ObjectSet(d.Root);

            _points.Clear();
        }
    }
}
