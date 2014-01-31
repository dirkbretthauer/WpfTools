#region Header
////////////////////////////////////////////////////////////////////////////// 
//    This file is part of $projectname$.
//
//    $projectname$ is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    $projectname$ is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with $projectname$.  If not, see <http://www.gnu.org/licenses/>.
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
