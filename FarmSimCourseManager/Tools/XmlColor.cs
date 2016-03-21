using System.Drawing;
using System.Globalization;

namespace FarmSimCourseManager.Tools
{
    public class XmlColor
    {
        private Color _color;

        public XmlColor()
        {
            _color = Color.Black;
        }

        public XmlColor(string colorValue)
        {
            _color = Convert(colorValue);
        }

        public XmlColor(Color color)
        {
            _color = color;
        }


        public Color ToColor()
        {
            return _color;
        }

        public void FromColor(Color color)
        {
            _color = color;
        }

        public static implicit operator Color(XmlColor xmlColor)
        {
            return xmlColor.ToColor();
        }

        public static implicit operator XmlColor(Color color)
        {
            return new XmlColor(color);
        }

        public static Color Convert(string value)
        {
            if (!value.StartsWith("#"))
                return Color.Black;

            var color = value.Substring(1);

            if (color.Length != 6 && color.Length != 8)
                return Color.Black;

            var intColor = int.Parse(color, NumberStyles.HexNumber);

            return color.Length == 6
                ? Color.FromArgb((intColor >> 16) & 0xFF, (intColor >> 8) & 0xFF, intColor & 0xFF)
                : Color.FromArgb((intColor >> 24) & 0xFF, (intColor >> 16) & 0xFF, (intColor >> 8) & 0xFF, intColor & 0xFF);
        }

        public static string Convert(Color color)
        {
            var colorValue = color.A == byte.MaxValue
                ? string.Format("#{0:X6}", (((color.R << 8) + color.G) << 8) + color.B)
                : string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
            return colorValue;
        }

        #region Overrides of Object

        public override string ToString()
        {
            return Convert(_color);
        }

        #endregion
    }
}
