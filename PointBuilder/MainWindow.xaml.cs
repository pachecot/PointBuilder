using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PointBuilder.Export;
using PointBuilder.Dialogs;
using System.Collections.Generic;
using System.Windows.Data;
using System;
using static System.Environment;

namespace PointBuilder
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Builder builder;
        ListCollectionView inputView;

        public MainWindow()
        {
            InitializeComponent();
            builder = new();
            inputView = new(builder.LibraryPoints);

            InputList.ItemsSource = inputView;
            OutputList.ItemsSource = builder.Points;

            DragSupport.Subscribe(InputList, DragDropEffects.Copy,
                (ListView lv) =>
                {
                    return lv.SelectedItems
                             .Cast<ObjectInstance>()
                             .ToList();
                });

            DropSupport.Subscribe<List<ObjectInstance>>(OutputList, DragDropEffects.Copy, AddItems);


            OutputList.KeyDown += (s, e) =>
            {
                var oi = OutputList.SelectedItem as ObjectInstance;
                if (oi == null)
                {
                    return;
                }
                switch (e.Key)
                {
                    case Key.Delete:
                        builder.Delete(oi);
                        break;
                }
            };

            OutputListCtxCopyItem.Click += (s, e) =>
            {
                var oi = OutputList.SelectedItem as ObjectInstance;
                if (oi == null) { return; }
                AddItems(new List<ObjectInstance>() { new ObjectInstance(oi) });
            };

            OutputListCtxDeleteItem.Click += (s, e) =>
            {
                var oi = OutputList.SelectedItem as ObjectInstance;
                if (oi == null) { return; }
                builder.Delete(oi);
            };

            OutputListCtxEditItem.Click += (s, e) =>
            {
                var oi = OutputList.SelectedItem as ObjectInstance;
                if (oi == null) { return; }

                PointDialog.ShowDialog(this, oi, newOI => builder.Update(oi, newOI));
            };


            var cc = Environment.GetFolderPath(SpecialFolder.LocalApplicationData);





            UpdateFilter();
        }

        private void UpdateFilter()
        {
            inputView.Filter = (o) =>
            {
                var oi = o as ObjectInstance;
                if (oi == null || InputFilterText.Text.Length == 0)
                {
                    return true;
                }
                return oi.Name.Contains(InputFilterText.Text, StringComparison.InvariantCultureIgnoreCase);
            };

            InputFilterText.TextChanged += (_, _) =>
            {
                inputView.Refresh();
            };

            inputView.Refresh();
        }

        private void AddItems(IEnumerable<ObjectInstance> data)
        {
            AddDialog.Open(this, data, builder.Add);
        }

        private void InputList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var c = e.OriginalSource as FrameworkElement;
            if (c?.DataContext is null) { return; }

            var point = c.DataContext as ObjectInstance;
            if (point != null)
            {
                AddItems(new List<ObjectInstance>() { point });
                return;
            }

            var points = c.DataContext as IEnumerable<ObjectInstance>;
            if (points != null)
            {
                AddItems(points);
                return;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var m = sender as MenuItem;
            if (m == null) { return; }
            switch (m.Name)
            {
                case "OpenLibrary":
                    CommonDialogs.FileOpenDialog(this, builder.LibraryFile(), file =>
                    {
                        builder.LoadLibrary(file);
                        inputView = new ListCollectionView(builder.LibraryPoints);
                        InputList.ItemsSource = inputView;
                        UpdateFilter();
                    });
                    break;
                case "Open":
                    CommonDialogs.FileOpenDialog(this, builder.File, builder.Load);
                    break;
                case "Save" when builder.File != null:
                    builder.Save();
                    break;
                case "Save":
                case "SaveAs":
                    CommonDialogs.FileSaveDialog(this, builder.File, builder.SaveAs);
                    break;
                case "Exit":
                    Close();
                    break;
                case "Close":
                    builder.Reset();
                    break;
            }
        }

        private void OutList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var c = e.OriginalSource as FrameworkElement;
            if (c?.DataContext is null) { return; }

            var oi = c.DataContext as ObjectInstance;
            if (oi == null) { return; }

            PointDialog.ShowDialog(this, oi, newOI => builder.Update(oi, newOI));
        }
    }
}
