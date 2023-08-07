using System.Collections.ObjectModel;
using System.Linq;

namespace PointBuilderApp.Export
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

}
