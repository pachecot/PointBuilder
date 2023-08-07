using System;
using System.Windows;

namespace PointBuilder
{
    internal class DropSupport

    {

        static DragEventHandler DragEventListener(Action<DragEventArgs> action) =>
            new((object sender, DragEventArgs e) => { action(e); });


        static Action<DragEventArgs> DragEventHandler(Action<DragEventArgs> action) =>
            new((DragEventArgs e) => { action(e); });

        static Action<DragEventArgs> DragEventComposer
            (params Action<DragEventArgs>[] actions) =>
            new((DragEventArgs e) => { foreach (var action in actions) { action(e); if (e.Handled) { break; } } });

        public static Action Subscribe<T>(UIElement element, DragDropEffects effects, Action<T> dropAction)
            where T : class
        {

            var onDrag = DragEventListener(e =>
            {
                if (!e.Data.GetDataPresent(typeof(T)))
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                    return;
                }

                e.Effects = e.AllowedEffects & effects;
                e.Handled = true;
            });

            var onDrop = DragEventListener(e =>
            {
                if (!e.Data.GetDataPresent(typeof(T)))
                {
                    e.Effects = DragDropEffects.None;
                    return;
                }

                if ((e.AllowedEffects & effects) == DragDropEffects.None)
                {
                    e.Effects = DragDropEffects.None;
                    return;
                }

                T? _data = e.Data.GetData(typeof(T)) as T;
                if (_data == null)
                {
                    e.Effects = DragDropEffects.None;
                    return;
                }

                e.Effects = e.AllowedEffects & effects;
                dropAction(_data);
            });

            element.DragEnter += onDrag;
            element.DragOver += onDrag;
            element.Drop += onDrop;

            return () =>
            {
                element.DragEnter -= onDrag;
                element.DragOver -= onDrag;
                element.Drop -= onDrop;
            };
        }

    }
}
