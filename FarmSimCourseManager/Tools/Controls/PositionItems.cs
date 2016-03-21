using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FarmSimCourseManager.Tools.Controls
{
    public class PositionItems : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyPropertyHelper<PositionItems>.Register(o => o.ItemsSource);
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyPropertyHelper<PositionItems>.Register(o => o.ItemTemplate);

        public PositionItems()
        {
            DefaultStyleKey = typeof (PositionItems);
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(PositionItems), new FrameworkPropertyMetadata(typeof(PositionItems)));
        }

        public IEnumerable<IPositionItem> ItemsSource
        {
            get { return (IEnumerable<IPositionItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
    }
}
