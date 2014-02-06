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

using System.Windows;

namespace WpfTools.DragDrop
{
    /// <summary>
    /// Represents a drop target for UIElements.
    /// </summary>
    public interface IDropTargetAdvisor
    {
        /// <summary>
        /// Gets or sets the target UI.
        /// </summary>
        /// <value>
        /// The target UI.
        /// </value>
        UIElement TargetUI { get; set; }

        /// <summary>
        /// Determines whether obj is a valid data object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        ///   <c>true</c> if obj is a valid data object; otherwise, <c>false</c>.
        /// </returns>
        bool IsValidDataObject(IDataObject obj);

        /// <summary>
        /// Called when a drop is completed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="dropPoint">The drop point.</param>
        void OnDropCompleted(IDataObject obj, Point dropPoint);

        /// <summary>
        /// Gets the ui content shown during the drag operation.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        object GetUiContent(IDataObject obj);
    }
}
