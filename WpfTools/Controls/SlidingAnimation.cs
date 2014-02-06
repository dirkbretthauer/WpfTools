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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WpfTools.Controls
{
    /// <summary>
    /// Defines the direction of the slide animation.
    /// </summary>
    internal enum Direction
    {
        /// <summary>
        /// Go backward, in less detail direction
        /// </summary>
        LeftToRight,

        /// <summary>
        /// Go forward, in more detail direction.
        /// </summary>
        RightToLeft
    }

    /// <summary>
    /// Given two VisualBrush'es, this class creates a sliding animation
    /// from one to the other one.
    /// </summary>
    internal class SlidingAnimation
    {
        #region attributes
        private Direction _direction;
        private Rectangle _nextRect;
        private Rectangle _prevRect;
        private Grid _rectContainer;
        private Duration _duration = new Duration(TimeSpan.FromSeconds(0.5));
        #endregion

        #region properties

        #region  Direction
        /// <summary>
        /// Direction of the sliding animation
        /// </summary>
        internal Direction Direction
        {
            get
            {
                return this._direction;
            }
            set
            {
                this._direction = value;
            }
        }
        #endregion

        #region Owner
        /// <summary>
        /// The owner holds the FrameworkElement on which the
        /// sliding animation is executed.
        /// </summary>
        internal ContentSwitchPresenter Owner { get; set; }
        #endregion

        #endregion

        /// <summary>
        /// Initializes a SlidingAnimation with a direcetion
        /// LeftToRight (GoForward)
        /// </summary>
        internal SlidingAnimation()
            : this(Direction.RightToLeft)
        {
        }

        /// <summary>
        /// Initializes a SlidingAnimation
        /// </summary>
        internal SlidingAnimation(Direction direction)
        {
            this.Direction = direction;
        }

        /// <summary>
        /// Creates a Storyboard which contains two animations, one animation
        /// to slide-out the current content, and one to slide-in the
        /// new content.
        /// </summary>
        internal Storyboard CreateStoryboard()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation element = new DoubleAnimation();
            DoubleAnimation animation2 = new DoubleAnimation();

            Storyboard.SetTargetName(element, "PrevElement");
            Storyboard.SetTargetProperty(element, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            Storyboard.SetTargetName(animation2, "NextElement");
            Storyboard.SetTargetProperty(animation2, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            element.Duration = _duration;
            animation2.Duration = _duration;
            if (Direction == Direction.RightToLeft)
            {
                element.From = 0.0;
                element.To = -1.0 * Owner.ActualWidth;
                animation2.From = Owner.ActualWidth;
                animation2.To = 0.0;
            }
            else
            {
                element.From = 0.0;
                element.To = Owner.ActualWidth;
                animation2.From = -1.0 * Owner.ActualWidth;
                animation2.To = 0.0;
            }

            storyboard.Children.Add(element);
            storyboard.Children.Add(animation2);
            return storyboard;
        }

        /// <summary>
        /// Creates the FrameworkElements which will be
        /// animated.
        /// </summary>
        internal void Setup(Brush prevBrush, Brush nextBrush)
        {
            SetupElements();
            _prevRect.Fill = prevBrush;
            _nextRect.Fill = nextBrush;
            Owner.AddAnimationElement(_rectContainer);
        }

        /// <summary>
        /// Called when the animation has finished.
        /// </summary>
        internal void Cleanup()
        {
            _nextRect = null;
            _prevRect = null;
            _rectContainer = null;
        }

        private void SetupElements()
        {
            _rectContainer = new Grid();
            _rectContainer.ClipToBounds = true;
            _prevRect = new Rectangle();
            _prevRect.RenderTransform = new TranslateTransform();
            _nextRect = new Rectangle();
            _nextRect.RenderTransform = new TranslateTransform();
            _rectContainer.Children.Add(this._nextRect);
            _rectContainer.Children.Add(this._prevRect);
            NameScope nameScope = GetNameScope();
            nameScope.RegisterName("PrevElement", this._prevRect);
            nameScope.RegisterName("NextElement", this._nextRect);
        }

        private NameScope GetNameScope()
        {
            NameScope scope = new NameScope();
            NameScope.SetNameScope(this.Owner.AnimationContainer, scope);
            return scope;
        }
    }
}
