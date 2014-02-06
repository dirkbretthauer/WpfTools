#region Header
////////////////////////////////////////////////////////////////////////////// 
//The MIT License (MIT)

//Copyright (c) 2013 Dirk Bretthauer

//Permission is hereby granted, free of charge, to any person obtaining a copy of
//this software and associated documentation files (the "Software"), to deal in
//the Software without restriction, including without limitation the rights to
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//the Software, and to permit persons to whom the Software is furnished to do so,
//subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using WpfTools.MouseNStuff;

namespace WpfTools.DragDrop
{
    public static class DragDropManager
    {
        private const string Offsetpoint = "OffsetPoint";
        private static UIElement _draggedElt;
        private static bool _isMouseDown = false;
        private static Point _dragStartPoint;
        private static Point _offsetPoint;
        private static DropPreviewAdorner _overlayElt;

        #region Dependency Properties
        public static readonly DependencyProperty DragSourceAdvisorProperty =
                DependencyProperty.RegisterAttached("DragSourceAdvisor", typeof(IDragSourceAdvisor), typeof(DragDropManager),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDragSourceAdvisorChanged)));

        public static IDragSourceAdvisor GetDragSourceAdvisor(DependencyObject depObj)
        {
            return depObj.GetValue(DragSourceAdvisorProperty) as IDragSourceAdvisor;
        }

        public static void SetDragSourceAdvisor(DependencyObject depObj, bool isSet)
        {
            depObj.SetValue(DragSourceAdvisorProperty, isSet);
        }

        public static readonly DependencyProperty DropTargetAdvisorProperty =
            DependencyProperty.RegisterAttached("DropTargetAdvisor", typeof(IDropTargetAdvisor), typeof(DragDropManager),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDropTargetAdvisorChanged)));

        public static void SetDropTargetAdvisor(DependencyObject depObj, bool isSet)
        {
            depObj.SetValue(DropTargetAdvisorProperty, isSet);
        }

        public static IDropTargetAdvisor GetDropTargetAdvisor(DependencyObject depObj)
        {
            return depObj.GetValue(DropTargetAdvisorProperty) as IDropTargetAdvisor;
        }

        public static readonly DependencyProperty DragDropTemplateProperty =
            DependencyProperty.RegisterAttached("DragDropTemplate", typeof(DataTemplate), typeof(DragDropManager),
            new UIPropertyMetadata(null));


        public static DataTemplate GetDragDropTemplate(DependencyObject depObj)
        {
            return (DataTemplate)depObj.GetValue(DragDropTemplateProperty);
        }

        public static void SetDragDropTemplate(DependencyObject depObj, DataTemplate value)
        {
            depObj.SetValue(DragDropTemplateProperty, value);
        }
        #endregion

        #region Property Change handlers
        private static void OnDragSourceAdvisorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            UIElement sourceElt = depObj as UIElement;
            if (args.NewValue != null && args.OldValue == null)
            {
                sourceElt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
                sourceElt.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                sourceElt.PreviewMouseUp += new MouseButtonEventHandler(DragSource_PreviewMouseUp);

                // Set the Drag source UI
                IDragSourceAdvisor advisor = args.NewValue as IDragSourceAdvisor;
                advisor.SourceUI = sourceElt;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                sourceElt.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
                sourceElt.PreviewMouseMove -= DragSource_PreviewMouseMove;
                sourceElt.PreviewMouseUp -= DragSource_PreviewMouseUp;
            }
        }

        static void DragSource_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            Mouse.Capture(null);
        }

        private static void OnDropTargetAdvisorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            UIElement targetElt = depObj as UIElement;
            if (args.NewValue != null && args.OldValue == null)
            {
                targetElt.PreviewDragEnter += new DragEventHandler(DropTarget_PreviewDragEnter);
                targetElt.PreviewDragOver += new DragEventHandler(DropTarget_PreviewDragOver);
                targetElt.PreviewDragLeave += new DragEventHandler(DropTarget_PreviewDragLeave);
                targetElt.PreviewDrop += new DragEventHandler(DropTarget_PreviewDrop);

                targetElt.AllowDrop = true;

                // Set the Drag source UI
                IDropTargetAdvisor advisor = args.NewValue as IDropTargetAdvisor;
                advisor.TargetUI = targetElt;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                targetElt.PreviewDragEnter -= DropTarget_PreviewDragEnter;
                targetElt.PreviewDragOver -= DropTarget_PreviewDragOver;
                targetElt.PreviewDragLeave -= DropTarget_PreviewDragLeave;
                targetElt.PreviewDrop -= DropTarget_PreviewDrop;


                targetElt.AllowDrop = false;
            }
        }


        #endregion

        #region Drop Target events 
        static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            if (UpdateEffects(sender, e) == false)
            {
                return;
            }

            IDropTargetAdvisor advisor = GetDropTargetAdvisor(sender as DependencyObject);
            Point dropPoint = e.GetPosition(advisor.TargetUI);

            advisor.OnDropCompleted(e.Data, dropPoint);
            RemovePreviewAdorner();
            _offsetPoint = new Point(0, 0);
        }

        static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            if (UpdateEffects(sender, e) == false)
            {
                return;
            }

            RemovePreviewAdorner();
            e.Handled = true;
        }

        static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            IDropTargetAdvisor advisor = GetDropTargetAdvisor(sender as DependencyObject);

            if (UpdateEffects(sender, e) == false)
            {
                return;
            }
            // Update position of the preview Adorner
            Point position = e.GetPosition(advisor.TargetUI);
            _overlayElt.Position = new Point(position.X - _offsetPoint.X, position.Y - _offsetPoint.Y);

            e.Handled = true;
        }

        static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {

            if (UpdateEffects(sender, e) == false)
            {
                return;
            }

            IDropTargetAdvisor advisor = GetDropTargetAdvisor(sender as DependencyObject);
            object uiContent = advisor.GetUiContent(e.Data);
            _offsetPoint = GetOffsetPoint(e.Data);

            CreatePreviewAdorner(advisor.TargetUI, uiContent);
            
            e.Handled = true;
        }

        static bool UpdateEffects(object uiObject, DragEventArgs e)
        {
            IDropTargetAdvisor advisor = GetDropTargetAdvisor(uiObject as DependencyObject);
            if (advisor.IsValidDataObject(e.Data) == false) return false;

            if ((e.AllowedEffects & DragDropEffects.Move) == 0 &&
                (e.AllowedEffects & DragDropEffects.Copy) == 0)
            {
                e.Effects = DragDropEffects.None;
                return true;
            }

            if ((e.AllowedEffects & DragDropEffects.Move) != 0 &&
                (e.AllowedEffects & DragDropEffects.Copy) != 0)
            {
                if ((e.KeyStates & DragDropKeyStates.ControlKey) != 0)
                {
                }
                e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ?
                    DragDropEffects.Copy : DragDropEffects.Move;
            }

            return true;
        }
        #endregion

        #region Drag Source events
        static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Make this the new drag source
            IDragSourceAdvisor advisor = GetDragSourceAdvisor(sender as DependencyObject);

            if (advisor.IsDraggable(e.Source as UIElement) == false) return;

            _draggedElt = e.Source as UIElement;
            _dragStartPoint = e.GetPosition(advisor.SourceUI);
            
            _offsetPoint = e.GetPosition(_draggedElt);
            _isMouseDown = true;
        }

        static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            IDragSourceAdvisor advisor = GetDragSourceAdvisor(sender as DependencyObject);

            if (_isMouseDown && IsDragGesture(e.GetPosition(advisor.SourceUI)))
            {
                DragStarted(sender as UIElement);
            }
        }

        static void DragStarted(UIElement uiElt)
        {
            _isMouseDown = false;
            Mouse.Capture(uiElt);

            IDragSourceAdvisor advisor = GetDragSourceAdvisor(uiElt as DependencyObject);
            DataObject data = advisor.GetDataObject(_draggedElt);

            SetOffsetPoint(data, _offsetPoint);

            DragDropEffects supportedEffects = advisor.SupportedEffects;

            // Perform DragDrop

            DragDropEffects effects = System.Windows.DragDrop.DoDragDrop(_draggedElt, data, supportedEffects);
            advisor.FinishDrag(_draggedElt, effects);

            // Clean up
            RemovePreviewAdorner();
            Mouse.Capture(null);
            _draggedElt = null;
        }

        static bool IsDragGesture(Point point)
        {
            bool hGesture = Math.Abs(point.X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance;
            bool vGesture = Math.Abs(point.Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance;

            return (hGesture | vGesture);
        }
        #endregion

        #region utility functions
        static UIElement GetTopContainer(UIElement adornedElt)
        {
            //return  LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "canvas") as UIElement;

            return Window.GetWindow(adornedElt);
        }

        private static void CreatePreviewAdorner(UIElement adornedElt, object feedbackUI)
        {
            // Clear if there is an existing preview adorner
            RemovePreviewAdorner();

            _overlayElt = new DropPreviewAdorner(feedbackUI, adornedElt, GetDragDropTemplate(adornedElt),
                                                 AdornerLayer.GetAdornerLayer(adornedElt));
        }

        private static void RemovePreviewAdorner()
        {
            if (_overlayElt != null)
            {
                _overlayElt.Detach();
                _overlayElt = null;
            }
        }

        static Point GetOffsetPoint(IDataObject obj)
        {
            return (Point)obj.GetData(Offsetpoint);
        }

        static void SetOffsetPoint(IDataObject obj, Point point)
        {
            obj.SetData(Offsetpoint, point);
        }
        #endregion
    }
}
