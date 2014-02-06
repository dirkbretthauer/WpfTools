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
using System.Windows.Markup;
using System.Windows.Data;
using System.ComponentModel;
using System.Diagnostics;

namespace WpfTools.PersistentSettings
{
    /// <summary>
    /// This class is a markup extension implementation. Markup extension classes exist mainly to provide 
    /// infrastructure support for some aspect of the WPF XAML reader implementation, and the members exposed by 
    /// a markup extension are not typically called from user code. 
    /// This extension supports the x:UserSettings Markup Extension usage from XAML.
	/// 
	/// example usage: 
	///   Width="{app:UserSettings Default=Auto,Key=MainWindow.Grid.Column1}"
    /// </summary>
    public class UserSettingsExtension : MarkupExtension
    {
        private string _defaultValue;
        private string _key;
        private static DictionaryAppSettings _settings;

        /// <summary>
        /// Gets or sets the key that is used for persistent storage.
        /// make sure that this key is unique for the application.
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// the default used when the value cannot be retrieved from persistent storage.
        /// </summary>
        public string Default
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }

        public static StringDictionary Dictionary
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new DictionaryAppSettings("XamlPersist");
                    Application.Current.MainWindow.Closing += OnMainWindowClosing;
		        }

                return _settings.Dictionary;
            }
        }

        static void OnMainWindowClosing(object sender, CancelEventArgs e)
		{
			_settings.Save();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingsExtension"/> class.
        /// </summary>
        public UserSettingsExtension()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingsExtension"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public UserSettingsExtension(string defaultValue)
        {
            _defaultValue = defaultValue;
        }

        #region MarkupExtension overrides

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget provideValue = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (provideValue == null)
            {
                throw new NotSupportedException("The IProvideValueTarget is not supported");
            }

            DependencyObject targetObject = provideValue.TargetObject as DependencyObject;
			if (targetObject == null)
			{
				Debug.Fail(string.Format("can't persist type {0}, not a dependency object", provideValue.TargetObject));
				throw new NotSupportedException();
			}

			DependencyProperty targetProperty = provideValue.TargetProperty as DependencyProperty;
			if (targetProperty == null)
			{
				Debug.Fail(string.Format("can't persist type {0}, not a dependency property", provideValue.TargetProperty));
				throw new NotSupportedException();
			}

			if (_key == null)
			{
                IUriContext uriContext = (IUriContext)serviceProvider.GetService(typeof(IUriContext));

                // UIElements have a 'Uid' property that must be set!
                if (targetObject is UIElement)
                {
                    _key = string.Format("{0}.{1}[{2}].{3}", 
                        uriContext.BaseUri.PathAndQuery, 
                        targetObject.GetType().Name, ((UIElement)targetObject).Uid, 
                        targetProperty.Name);
                }
                // use parent-child relation to generate unique key
                else if (LogicalTreeHelper.GetParent(targetObject) is UIElement)
                {
                    UIElement parent = (UIElement)LogicalTreeHelper.GetParent(targetObject);
                    int i = 0;
                    foreach (object c in LogicalTreeHelper.GetChildren(parent))
                    {
                        if (c == targetObject)
                        {
                            _key = string.Format("{0}.{1}[{2}].{3}[{4}].{5}", 
                                uriContext.BaseUri.PathAndQuery, 
                                parent.GetType().Name, parent.Uid,
                                targetObject.GetType().Name, i, 
                                targetProperty.Name);
                            break;
                        }
                        i++;
                    }
                }
                //TODO:should do something clever here to get a good key for tags like GridViewColumn

                if (_key == null)
                {
                    Debug.Fail(string.Format("don't know how to automatically get a key for objects of type {0}\n use Key='...' option", targetObject.GetType()));

                    // fallback to default value if no key available
                    DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(targetProperty, targetObject.GetType());
                    return descriptor.GetValue(targetObject);
                }
                else
                {
                    Debug.WriteLine(string.Format("key={0}", _key));
                }
			}

            if (!Dictionary.ContainsKey(_key))
            {
                Dictionary[_key] = _defaultValue;
            }

            object value = ConvertFromString(targetObject, targetProperty, Dictionary[_key]);
   
            SetBinding(targetObject, targetProperty, _key);

            return value;
        }

        #endregion

        #region static functions

        private void SetBinding(DependencyObject targetObject, DependencyProperty targetProperty, string key)
        {
            Binding binding = new Binding();
            binding.Mode = BindingMode.OneWayToSource;
            binding.Path = new PropertyPath(string.Format("Dictionary[{0}]", key));
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this;
            BindingOperations.SetBinding(targetObject, targetProperty, binding);
        }

		private static object ConvertFromString(DependencyObject targetObject, DependencyProperty targetProperty, string stringValue)
        {
            DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(targetProperty, targetObject.GetType());
            return string.IsNullOrEmpty(stringValue) ? descriptor.GetValue(targetObject) : descriptor.Converter.ConvertFromInvariantString(stringValue);
        }

        #endregion
    }
}
