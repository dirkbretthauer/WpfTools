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
