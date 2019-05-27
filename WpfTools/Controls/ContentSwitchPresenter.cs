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
using System.Windows.Input;
using Prism.Commands;

namespace WpfTools.Controls
{
        /// <summary>
    /// This control implements a sliding animation between two of its children.
    /// It can have multiple children but only one is visible at a time.
    /// The GoForward and GoBackward properties can be used to slide through
    /// the content of the children in the respective direction.
    /// </summary>
    [TemplatePart(Name = ContentSwitchPresenter.ElementChildren, Type = typeof(Grid))]
    [TemplatePart(Name = ContentSwitchPresenter.ElementAnimationContainer, Type = typeof(Grid))]
    public class ContentSwitchPresenter : ItemsControl
    {
        #region attributes
        private bool _isPlaying;
        private FrameworkElement _nextChild;
        private FrameworkElement _prevChild;
        internal Grid ChildContainer { get; set; }
        internal Grid AnimationContainer { get; set; }
        private SlidingAnimation _slidingAnimation;
        private int _currentIndex;

        private Point _mouseDownPosition;

        private const string ElementChildren = "PART_ChildrenContainer";
        private const string ElementAnimationContainer = "PART_TransitionElementsContainer";
        #endregion

        #region properties

        #region GoForwardCommand
        /// <summary>
        /// Executes the animation from right to left
        /// </summary>
        public ICommand GoForwardCommand
        {
            get { return (ICommand)GetValue(GoForwardCommandProperty); }
            set { SetValue(GoForwardCommandProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GoForwardCommand.
        /// </summary>
        public static readonly DependencyProperty GoForwardCommandProperty =
            DependencyProperty.Register("GoForwardCommand", typeof(ICommand), typeof(ContentSwitchPresenter), new UIPropertyMetadata(null));
        #endregion

        #region GoBackwardCommand
        /// <summary>
        /// Executes the animation from left to right.
        /// </summary>
        public ICommand GoBackwardCommand
        {
            get { return (ICommand)GetValue(GoBackwardCommandProperty); }
            set { SetValue(GoBackwardCommandProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GoBackwardCommand.
        /// </summary>
        public static readonly DependencyProperty GoBackwardCommandProperty =
            DependencyProperty.Register("GoBackwardCommand", typeof(ICommand), typeof(ContentSwitchPresenter), new UIPropertyMetadata(null));
        #endregion

        #region CanGoForward
        /// <summary>
        /// Enables/Disables the GoForwardCommand.
        /// </summary>
        public bool CanGoForward
        {
            get { return (bool)GetValue(CanGoForwardProperty); }
            set { SetValue(CanGoForwardProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanGoForward.
        /// </summary>
        public static readonly DependencyProperty CanGoForwardProperty =
            DependencyProperty.Register("CanGoForward", typeof(bool), typeof(ContentSwitchPresenter), new UIPropertyMetadata(false, OnCanGoForwardChanged));

        private static void OnCanGoForwardChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ContentSwitchPresenter presenter = obj as ContentSwitchPresenter;
            presenter.ForwardCanExecuteChanged();
        }
        #endregion

        #region CanGoBackward
        /// <summary>
        /// Enables/Disables the GoBackwardCommand.
        /// </summary>
        public bool CanGoBackward
        {
            get { return (bool)GetValue(CanGoBackwardProperty); }
            set { SetValue(CanGoBackwardProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanGoBackward.
        /// </summary>
        public static readonly DependencyProperty CanGoBackwardProperty =
            DependencyProperty.Register("CanGoBackward", typeof(bool), typeof(ContentSwitchPresenter), new UIPropertyMetadata(false, OnCanGoBackwardChanged));

        private static void OnCanGoBackwardChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ContentSwitchPresenter presenter = obj as ContentSwitchPresenter;
            presenter.BackwardCanExecuteChanged();
        }
        #endregion

        #region CurrentIndexChangedCommand
        /// <summary>
        /// Is executed when a content switch took place and has the new
        /// visible child index as parameter
        /// </summary>
        public ICommand CurrentIndexChangedCommand
        {
            get { return (ICommand)GetValue(CurrentIndexChangedCommandProperty); }
            set { SetValue(CurrentIndexChangedCommandProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GoBackwardCommand.
        /// </summary>
        public static readonly DependencyProperty CurrentIndexChangedCommandProperty =
            DependencyProperty.Register("CurrentIndexChangedCommand", typeof(ICommand), typeof(ContentSwitchPresenter), new UIPropertyMetadata(null));
        #endregion
        #endregion

        /// <summary>
        /// Initializes a ContentSwitchPresenter.
        /// </summary>
        public ContentSwitchPresenter()
            : base()
        {
            _slidingAnimation = new SlidingAnimation();

            AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPreviewMouseLeftButtonDownEvent), true);
            AddHandler(PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(OnPreviewMouseLeftButtonUpEvent), true);

            Loaded += OnContentSwitchLoaded;
            IsVisibleChanged += OnVisibilityChanged;
        }

        private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (ChildContainer != null)
            {
                if (Visibility != Visibility.Visible)
                {
                    foreach (UIElement element in this.ChildContainer.Children)
                    {
                        element.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    foreach (UIElement element in this.ChildContainer.Children)
                    {
                        if (Panel.GetZIndex(element) == 1)
                        {
                            element.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        private void OnGoForwardExecuted(object parameter)
        {
            DoGoForward();
        }

        private void OnGoBackwardExecuted(object parameter)
        {
            DoGoBackward();
        }
        
        private bool IsGoForwardEnabled(object parameter)
        {
            return CanGoForward;
        }

        private bool IsGoBackwardEnabled(object parameter)
        {
            return CanGoBackward;
        }

        private void ForwardCanExecuteChanged()
        {
            if (GoForwardCommand != null)
            {
                ((DelegateCommand<object>)GoForwardCommand).RaiseCanExecuteChanged();
            }
        }

        private void BackwardCanExecuteChanged()
        {
            if (GoBackwardCommand != null)
            {
                ((DelegateCommand<object>)GoBackwardCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes
        /// call System.Windows.FrameworkElement.ApplyTemplate().
        /// Gets the ChildContainer template element which is the ItemsHost
        /// of this control and the animation container.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.ChildContainer = Template.FindName(ElementChildren, this) as Grid;
            this.AnimationContainer = Template.FindName(ElementAnimationContainer, this) as Grid;
        }

        /// <summary>
        /// Adds the FrameworkElement on which the animation is done.
        /// Called by the sliding animation.
        /// </summary>
        internal void AddAnimationElement(FrameworkElement elt)
        {
            AnimationContainer.Children.Add(elt);
        }

        private void OnContentSwitchLoaded(object sender, RoutedEventArgs e)
        {
            if (ChildContainer != null && ChildContainer.Children.Count > 0)
            {
                _nextChild = (FrameworkElement)ChildContainer.Children[0];
                ChangeChildrenStackOrder();

                ChildContainer.InvalidateMeasure();
            }

            SetValue(GoForwardCommandProperty, new DelegateCommand<object>(OnGoForwardExecuted, IsGoForwardEnabled));
            SetValue(GoBackwardCommandProperty, new DelegateCommand<object>(OnGoBackwardExecuted, IsGoBackwardEnabled));
        }

        private bool DoGoForward()
        {
            if (_currentIndex < ChildContainer.Children.Count - 1)
            {
                _slidingAnimation.Direction = Direction.RightToLeft;
                if (ApplyAnimation(GetItem(_currentIndex),
                                GetItem(_currentIndex + 1)))
                {
                    _currentIndex++;
                    FireCurrentIndexChanged();

                    return true;
                }
            }

            return false;
        }

        private bool DoGoBackward()
        {
            if (_currentIndex > 0)
            {
                _slidingAnimation.Direction = Direction.LeftToRight;
                if (ApplyAnimation(GetItem(_currentIndex),
                                GetItem(_currentIndex - 1)))
                {
                    _currentIndex--;
                    FireCurrentIndexChanged();

                    return true;
                }
            }
            return false;
        }

        private void FireCurrentIndexChanged()
        {
            ICommand command = CurrentIndexChangedCommand;
            if (command != null)
            {
                command.Execute(_currentIndex);
            }
        }

        

        private void OnPreviewMouseLeftButtonDownEvent(object sender, MouseButtonEventArgs e)
        {
            _mouseDownPosition = e.GetPosition(this);
        }

        private void OnPreviewMouseLeftButtonUpEvent(object sender, MouseButtonEventArgs e)
        {
            var verticalMove = _mouseDownPosition.X - e.GetPosition(this).X;

            if(verticalMove < -60)
            {
                if (CanGoBackward && DoGoBackward())
                {
                    e.Handled = true;
                }

            }
            else if(verticalMove > 60)
            {
                if (CanGoForward && DoGoForward())
                {
                    e.Handled = true;
                }
            }
        }

        private bool ApplyAnimation(FrameworkElement prevChild, FrameworkElement nextChild)
        {
            bool animationPlayed = false;

            if (!_isPlaying)
            {
                _prevChild = prevChild;
                _nextChild = nextChild;
                _prevChild.Visibility = Visibility.Visible;
                _nextChild.Visibility = Visibility.Visible;
                SwitchToAnimationMode(true);
                _slidingAnimation.Owner = this;
                _slidingAnimation.Setup(CreateBrush(_prevChild), CreateBrush(_nextChild));
                Storyboard sb = _slidingAnimation.CreateStoryboard();
                PlayStoryboard(sb);
                animationPlayed = true;
            }

            return animationPlayed;
        }

        private void ChangeChildrenStackOrder()
        {
            Panel.SetZIndex(this._nextChild, 1);
            foreach (UIElement element in this.ChildContainer.Children)
            {
                if (element != this._nextChild)
                {
                    Panel.SetZIndex(element, 0);
                    element.Visibility = Visibility.Hidden;
                }
            }
        }

        private static Brush CreateBrush(FrameworkElement element)
        {
            return new VisualBrush(element) { Viewbox = new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight), ViewboxUnits = BrushMappingMode.Absolute };
        }

        private void FinishTransition()
        {
            _slidingAnimation.Cleanup();
            AnimationContainer.Children.Clear();
            ChangeChildrenStackOrder();
            SwitchToAnimationMode(false);
        }

        private FrameworkElement GetItem(int currentIndex)
        {
            return (ChildContainer.Children[currentIndex] as FrameworkElement);
        }

        private void PlayStoryboard(Storyboard sb)
        {
            EventHandler handler = null;
            handler = delegate
            {
                sb.Completed -= handler;
                FinishTransition();
            };
            sb.Completed += handler;
            sb.Begin(this.AnimationContainer);
        }

        private void SwitchToAnimationMode(bool isTransition)
        {
            if (isTransition)
            {
                this.AnimationContainer.Visibility = Visibility.Visible;
                this.ChildContainer.Visibility = Visibility.Hidden;
            }
            else
            {
                this.AnimationContainer.Visibility = Visibility.Hidden;
                this.ChildContainer.Visibility = Visibility.Visible;
                this.ChildContainer.InvalidateMeasure();
            }
            this._isPlaying = isTransition;
        }
    }
}
