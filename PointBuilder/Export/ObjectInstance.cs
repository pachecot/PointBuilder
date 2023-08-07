using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace PointBuilder.Export
{
    public class ObjectInstance : INotifyPropertyChanged
    {
        const string ATT_NAME = "NAME";
        const string ATT_TYPE = "TYPE";
        const string ATT_DESCR = "DESCR";
        const string TAG_OI = "OI";

        XElement _element;
        string _name;
        string _desc;
        string _type;

        public static string NameSelector(ObjectInstance p) => p.Name;

        public static string DescriptionSelector(ObjectInstance p) => p.Description;

        public XElement Element
        {
            get { return _element; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) { return; }
                _name = value;
                _element.SetAttributeValue(ATT_NAME, value);
                NotifyPropertyChanged();
            }
        }
        public string Description
        {
            get { return _desc; }
            set
            {
                if (_desc == value) { return; }
                _desc = value;
                _element.SetAttributeValue(ATT_DESCR, value);
                NotifyPropertyChanged();
            }
        }

        public ObjectInstance(XElement oi)
        {
            if (oi.Name != TAG_OI) { throw new ArgumentException("expected OI element."); }
            _type = oi.Attribute(ATT_TYPE)?.Value ?? throw new ArgumentException("OI has no TYPE Attribute.");
            _name = oi.Attribute(ATT_NAME)?.Value ?? throw new ArgumentException("OI has no NAME Attribute.");
            _desc = oi.Attribute(ATT_DESCR)?.Value ?? "";
            _element = new XElement(oi);
        }

        public ObjectInstance(ObjectInstance other)
        {
            _type = other._type;
            _name = other._name;
            _desc = other._desc;
            _element = new XElement(other._element);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public override string ToString()
        {
            return _name;
        }
    }
}
