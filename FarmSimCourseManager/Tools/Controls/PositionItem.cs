using System.Windows;

namespace FarmSimCourseManager.Tools.Controls
{
    public abstract class PositionItem : ObservableObject, IPositionItem
    {
        private Point _position;

        public Point Position
        {
            get { return _position; }
            set
            {
                if (value.Equals(_position))
                    return;
                _position = value;
                NotifyPropertyChanged(() => Position);
            }
        }
    }

    public abstract class PositionItem<T> : PositionItem
    {
        private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                if (Equals(value, _value)) return;
                _value = value;
                NotifyPropertyChanged(() => Value);
            }
        }
    }
}
