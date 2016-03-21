using System;
using System.Collections.Generic;
using System.Windows;

namespace FarmSimCourseManager.Tools
{
    public static class WpfTools
    {
        /// <summary>
        /// Получить максимальный размер из набора элементов.
        /// </summary>
        /// <param name="elements">набор элементов</param>
        /// <returns>максимальный размер</returns>
        public static Size GetMaxActualSize(this IEnumerable<FrameworkElement> elements)
        {
            var size = Size.Empty;
            foreach (var element in elements)
            {
                var actualWidth = element.ActualWidth;
                var actualHeight = element.ActualHeight;
                if (actualWidth.IsNaN() || actualHeight.IsNaN() || actualWidth < double.Epsilon || actualHeight < double.Epsilon)
                    continue;
                if (size.IsEmpty)
                {
                    size = new Size(actualWidth, actualHeight);
                }
                else
                {
                    size.Width = Math.Max(size.Width, actualWidth);
                    size.Height = Math.Max(size.Height, actualHeight);
                }
            }
            return size;
        }
    }
}