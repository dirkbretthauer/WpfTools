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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfTools.DragDrop
{
    /// <summary>
    /// Adorner for the dragging process
    /// </summary>
    public class DropPreviewAdorner : Adorner
    {
        private readonly ContentPresenter _presenter;
        private readonly AdornerLayer _adornerLayer;
        private Point _position;

        /// <summary>
        /// Gets or sets the left-top position.
        /// </summary>
        /// <value>
        /// The left-top position.
        /// </value>
        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropPreviewAdorner"/> class.
        /// </summary>
        /// <param name="feedbackUI">The feedback UI.</param>
        /// <param name="adornedElt">The adorned elt.</param>
        /// <param name="dragDropTemplate">The drag drop template.</param>
        /// <param name="adornerLayer">The adorner layer.</param>
        public DropPreviewAdorner(object feedbackUI, UIElement adornedElt,
                                  DataTemplate dragDropTemplate, AdornerLayer adornerLayer)
            : base(adornedElt)
        {
            _adornerLayer = adornerLayer;
            _presenter = new ContentPresenter();
            _presenter.Content = feedbackUI;
            _presenter.ContentTemplate = dragDropTemplate;
            _presenter.IsHitTestVisible = false;
            this.Visibility = Visibility.Collapsed;

            _adornerLayer.Add(this);
        }

        private void UpdatePosition()
        {
            AdornerLayer layer = this.Parent as AdornerLayer;
            if (layer != null)
            {
                layer.Update(AdornedElement);
            }
        }

        /// <summary>
        /// Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> object representing the
        /// amount of layout space needed by the adorner.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            _presenter.Measure(constraint);
            return _presenter.DesiredSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and
        /// determines a size for a <see cref="FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this
        /// element should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            _presenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Overrides <see cref="Visual.GetVisualChild(System.Int32)"/>, and returns a child
        /// at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null;
        /// if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return _presenter;
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Returns a <see cref="Transform"/> for the adorner, based on
        /// the transform that is currently applied to the adorned element.
        /// </summary>
        /// <param name="transform">The transform that is currently applied to the adorned element.</param>
        /// <returns>
        /// A transform to apply to the adorner.
        /// </returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(new TranslateTransform(_position.X, _position.Y));
            result.Children.Add(base.GetDesiredTransform(transform));

            if (this.Position.Y > 0)
            {
                this.Visibility = Visibility.Visible;
            }

            result.Children.Add(base.GetDesiredTransform(transform));

            return result;
        }

        public void Detach()
        {
            _adornerLayer.Remove(this);
        }
    }
}
