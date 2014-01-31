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
