using System;
using System.Windows;
using System.Windows.Input;

namespace PointBuilder
{
    internal class DragSupport
    {
        static bool movedDragDistance(Vector diff) => Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                                                      || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance;


        Func<Action<MouseButtonEventArgs>, Action> MouseLeftButtonDownEventHandler<U>(U element)
            where U : UIElement
        {
            RoutedEvent routedEvent = UIElement.MouseLeftButtonDownEvent;

            return (Action<MouseButtonEventArgs> a) =>
            {
                var asd = (object sender, MouseButtonEventArgs e) =>
                {
                    a(e);
                };
                MouseButtonEventHandler handler = new MouseButtonEventHandler(asd);
                element.AddHandler(routedEvent, handler, true);
                return () =>
                {
                    element.RemoveHandler(routedEvent, handler);
                };
            };

        }

        static MouseEventHandler MouseEventListener(Action<MouseEventArgs> action) =>
                    new((object sender, MouseEventArgs e) => { action(e); });

        static QueryContinueDragEventHandler QueryContinueDragEventListener(Action<QueryContinueDragEventArgs> action) =>
            new((object sender, QueryContinueDragEventArgs e) => { action(e); });

        static MouseButtonEventHandler MouseButtonEventListener(Action<MouseButtonEventArgs> action) =>
            new((object sender, MouseButtonEventArgs e) => { action(e); });

        static MouseButtonEventHandler MouseButtonEventListener(Action action) =>
            new((object sender, MouseButtonEventArgs e) => { action(); });

        public static Action Subscribe<T, U>(U element, DragDropEffects allowedEffects, Func<U, T> mapper)
            where U : UIElement
        {

            Point startPos;
            Action endDrag = () => { /* temporary, replaced after definitions */ };

            var onMouseMove = MouseEventListener(e =>
            {
                if (!movedDragDistance(startPos - e.GetPosition(null))) { return; }

                T data = mapper(element);
                DataObject dragData = new DataObject(typeof(T), data);
                DragDrop.DoDragDrop(element, dragData, allowedEffects);
            });

            var onQueryContinueDrag = QueryContinueDragEventListener(e =>
            {
                if ((e.KeyStates & DragDropKeyStates.LeftMouseButton) == DragDropKeyStates.LeftMouseButton) { return; }

                e.Action = DragAction.Cancel;
                endDrag();
            });

            var onMouseUp = MouseButtonEventListener(endDrag);

            Func<MouseEventArgs, Point> MouseEventPosition = e => e.GetPosition(null);
            Func<Action<Point>, Action<MouseEventArgs>> MapToPostion = (pf) => (MouseEventArgs e) => pf(MouseEventPosition(e));

            var mouseDown = MouseButtonEventListener(MapToPostion(pos =>
            {
                startPos = pos;
                element.MouseMove += onMouseMove;
                element.QueryContinueDrag += onQueryContinueDrag;
                element.AddHandler(UIElement.MouseLeftButtonUpEvent, onMouseUp, true);
            }));

            endDrag = () =>
            {
                element.MouseMove -= onMouseMove;
                element.QueryContinueDrag -= onQueryContinueDrag;
                element.RemoveHandler(UIElement.MouseLeftButtonUpEvent, onMouseUp);
            };

            element.AddHandler(UIElement.MouseLeftButtonDownEvent, mouseDown, true);

            return () =>
            {
                element.RemoveHandler(UIElement.MouseLeftButtonDownEvent, mouseDown);
            };
        }

        public static Action Enable<T, U>(U element, DragDropEffects allowedEffects, Func<T> dataFunc)
            where U : UIElement
        {
            Point startPos;
            Action endDrag = () => { /* temporary, replaced after definitions */ };


            var onMouseMove = MouseEventListener(e =>
            {
                if (!movedDragDistance(startPos - e.GetPosition(null))) { return; }

                DataObject dragData = new DataObject(typeof(T), dataFunc());
                DragDrop.DoDragDrop(element, dragData, allowedEffects);
            });

            var onQueryContinueDrag = QueryContinueDragEventListener(e =>
            {
                if ((e.KeyStates & DragDropKeyStates.LeftMouseButton) == DragDropKeyStates.LeftMouseButton) { return; }

                e.Action = DragAction.Cancel;
                endDrag();
            });

            var onMouseUp = MouseButtonEventListener(endDrag);

            var mouseDown = MouseButtonEventListener(e =>
            {
                startPos = e.GetPosition(null);
                element.MouseMove += onMouseMove;
                element.QueryContinueDrag += onQueryContinueDrag;
                element.AddHandler(UIElement.MouseLeftButtonUpEvent, onMouseUp, true);
            });

            endDrag = () =>
            {
                element.MouseMove -= onMouseMove;
                element.QueryContinueDrag -= onQueryContinueDrag;
                element.RemoveHandler(UIElement.MouseLeftButtonUpEvent, onMouseUp);
            };

            element.AddHandler(UIElement.MouseLeftButtonDownEvent, mouseDown, true);

            return () =>
            {
                element.RemoveHandler(UIElement.MouseLeftButtonDownEvent, mouseDown);
            };

        }
    }
}
