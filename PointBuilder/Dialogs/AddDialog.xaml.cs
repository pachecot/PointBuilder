using PointBuilder.XmlHelper;
using PointBuilder.Export;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace PointBuilder
{

    /// <summary>
    /// Interaction logic for AddDialog.xaml
    /// </summary>
    public partial class AddDialog : Window
    {
        public ObservableCollection<ObjectInstance> Items { get; set; }

        ObjectInstance? _item;
        IList<ObjectInstance>? _items;

        static public bool Open(Window owner, IEnumerable<ObjectInstance> data, Action<IEnumerable<ObjectInstance>> f)
        {
            var dialog = new AddDialog(owner, data);
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                f(dialog.Items);
                return true;
            }
            return false;
        }

        public AddDialog(Window owner, IEnumerable<ObjectInstance> items)
        {
            InitializeComponent();

            Owner = owner;
            _items = items.ToList();
            if (_items.Count == 1)
            {
                _item = _items[0];
            }
            else
            {
                NumberOfItems.IsEnabled = false;
            }

            OkButton.Click += (_, _) => { DialogResult = true; };
            CancelButton.Click += (_, _) => { DialogResult = false; };

            Items = new ObservableCollection<ObjectInstance>(items.Select(x => new ObjectInstance(x)));
            NewItems.ItemsSource = Items;
        }


        private void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            if (!Clipboard.ContainsText())
            {
                return;
            }

            e.Handled = true;

            var txt = Clipboard.GetText();
            var lines = txt.ReplaceLineEndings().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            if (Items.Count == 1)
            {
                while (lines.Length > Items.Count)
                {
                    var p = new ObjectInstance(Items[0]);
                    p.Name = $"{_item}_{Items.Count}";
                    Items.Add(p);
                }
                for (int i = 0; i < lines.Length; i++)
                {
                    Items[i].Name = lines[i];
                }
                return;
            }

            var j = NewItems.SelectedIndex == -1 ? 0 : NewItems.SelectedIndex;
            for (int i = 0; i < lines.Length && j < Items.Count; i++, j++)
            {
                Items[j].Name = lines[i];
            }
        }

        private void NumberOfItems_TextChanged(object sender, TextChangedEventArgs e)
        {
            int newCount;
        
            if (!int.TryParse(NumberOfItems.Text, out newCount))
            {
                return;
            }
            
            if (Items == null || newCount == Items.Count)
            {
                return;
            }

            if (newCount > Items.Count)
            {
                if (_item != null)
                {
                    while (newCount > Items.Count)
                    {
                        var p = new ObjectInstance(_item);
                        p.Name = $"{_item}_{Items.Count}";
                        Items.Add(p);
                    }
                }
                return;
            }
            
            for (var i = Items.Count - 1; i >= newCount; i--)
            {
                Items.RemoveAt(i);
            }
        }
    }
}
