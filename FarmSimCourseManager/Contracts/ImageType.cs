using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FarmSimCourseManager.Contracts
{
    /// <summary>
    /// Тип картики
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// Картинка
        /// </summary>
        [Description("Картинка")]
        Image = 1,
        /// <summary>
        /// Вектор
        /// </summary>
        [Description("Вектор")]
        Vector = 2,
        /// <summary>
        /// Xaml нотация
        /// </summary>
        [Description("Xaml нотация")]
        Xaml = 3
    }

    /// <summary>
    /// Хранение картинки
    /// </summary>
    [Serializable]
    public class ImageBinary
    {
        /// <summary>
        /// Тип картинки.
        /// </summary>
        [XmlAttribute]
        public ImageType ImageType { get; set; }

        /// <summary>
        /// Значение картинки.
        /// </summary>
        [XmlElement]
        public byte[] Image { get; set; }

        /// <summary>
        /// Ширина.
        /// </summary>
        [XmlAttribute]
        public int Width { get; set; }

        /// <summary>
        /// Высота.
        /// </summary>
        [XmlAttribute]
        public int Height { get; set; }
    }
}
