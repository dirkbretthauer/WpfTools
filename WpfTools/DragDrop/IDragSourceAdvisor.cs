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
    /// Represent a source of draggable UIElements.
    /// </summary>
    public interface IDragSourceAdvisor
    {
        /// <summary>
        /// Gets or sets the source UI.
        /// </summary>
        /// <value>
        /// The source UI.
        /// </value>
        UIElement SourceUI { get; set; }

        /// <summary>
        /// Gets the supported effects.
        /// </summary>
        DragDropEffects SupportedEffects { get; }

        /// <summary>
        /// Gets the data object.
        /// </summary>
        /// <param name="draggedElt">The dragged elt.</param>
        /// <returns></returns>
        DataObject GetDataObject(UIElement draggedElt);

        /// <summary>
        /// Finishes the drag.
        /// </summary>
        /// <param name="draggedElt">The dragged elt.</param>
        /// <param name="finalEffects">The final effects.</param>
        void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects);

        /// <summary>
        /// Determines whether the specified <paramref name="dragElt"/> is draggable.
        /// </summary>
        /// <param name="dragElt">The drag elt.</param>
        /// <returns>
        ///   <c>true</c> if the specified <paramref name="dragElt"/> is draggable; otherwise, <c>false</c>.
        /// </returns>
        bool IsDraggable(UIElement dragElt);
    }
}
