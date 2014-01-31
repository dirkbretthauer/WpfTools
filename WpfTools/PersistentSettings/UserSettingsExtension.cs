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
