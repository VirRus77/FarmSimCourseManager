using System.Collections.Generic;

namespace FarmSimCourseManager.Model
{
    /// <summary>
    /// Нода дерева
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Имя
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Дети
        /// </summary>
        IList<INode> Childs { get; }

        /// <summary>
        /// Закрыта нода
        /// </summary>
        bool IsCollapsed { get; set; }

        /// <summary>
        /// Выбрана или нет
        /// </summary>
        bool? IsChecked { get; set; }
    }

    public interface INode<T> : INode
    {
        T Value { get; }
    }
}
