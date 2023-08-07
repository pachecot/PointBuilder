using PointBuilder.Export;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PointBuilder
{
    /// <summary>
    /// Interaction logic for PointDialog.xaml
    /// </summary>
    public partial class PointDialog : Window
    {
        ObjectInstance oi;

        public PointDialog(Window owner, ObjectInstance oi)
        {
            this.Owner = owner;
            this.oi = oi;
            InitializeComponent();
            DataContext = this.oi;
        }

        public static bool ShowDialog(Window owner, ObjectInstance oi, Action<ObjectInstance> f)
        {
            var newOI = new ObjectInstance(oi);
            var dialog = new PointDialog(owner, newOI);
            if (!dialog.ShowDialog().GetValueOrDefault()) { return false; }
            f(newOI);
            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            if (b == null) { return; }
            this.DialogResult = !b.IsCancel;
            this.Close();
        }
    }
}
