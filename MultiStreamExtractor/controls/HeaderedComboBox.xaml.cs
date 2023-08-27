using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PhoneSpecsParser.controls
{
    public partial class HeaderedComboBox : UserControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(HeaderedComboBox), new PropertyMetadata(string.Empty));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(HeaderedComboBox), new PropertyMetadata(null));

        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(HeaderedComboBox), new PropertyMetadata(null, OnSelectedItemChanged));

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(HeaderedComboBox), new PropertyMetadata(-1, OnSelectedIndexChanged));

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        public HeaderedComboBox()
        {
            InitializeComponent();
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedComboBox headeredComboBox = (HeaderedComboBox)d;
            headeredComboBox.RaiseSelectionChangedEvent(e);
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedComboBox headeredComboBox = (HeaderedComboBox)d;
            int selectedIndex = (int)e.NewValue;
            headeredComboBox.comboBox.SelectedIndex = selectedIndex;
        }

        protected virtual void RaiseSelectionChangedEvent(DependencyPropertyChangedEventArgs e)
        {
            SelectionChangedEventArgs args = new SelectionChangedEventArgs(Selector.SelectionChangedEvent, new[] { e.OldValue }, new[] { e.NewValue });
            SelectionChanged?.Invoke(this, args);
        }
    }
}
