using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace PointBuilder.Dialogs
{
    internal class CommonDialogs
    {
        const string FileExtension = "xml";
        const string FileFilter = "Export Files|*.xml";

        public static void FileSaveDialog(Window owner, string? currentFile, Action<string> fileAction)
        {
            var dialog = new SaveFileDialog();
            dialog.DefaultExt = FileExtension;
            dialog.Filter = FileFilter;
            if (currentFile != null)
            {
                dialog.InitialDirectory = Path.GetDirectoryName(currentFile);
                dialog.FileName = currentFile;
            }
            var result = dialog.ShowDialog(owner);
            if (!result.HasValue || !result.Value)
            {
                return;
            };
            fileAction(dialog.FileName);
        }

        public static void FileOpenDialog(Window owner, string? currentFile, Action<string> fileAction)
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = FileExtension;
            dialog.Filter = FileFilter;
            if (currentFile != null)
            {
                dialog.InitialDirectory = Path.GetDirectoryName(currentFile);
                dialog.FileName = currentFile;
            }
            var result = dialog.ShowDialog(owner);
            if (!result.HasValue || !result.Value)
            {
                return;
            };
            fileAction(dialog.FileName);
        }
    }
}
