using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace Cinema.Controls
{
    public partial class NavItem : UserControl
    {
        // ══════════════════════════════════════════════════════
        //  DEPENDENCY PROPERTIES
        //  WPF requires DependencyProperty (not plain C# props)
        //  for XAML binding, triggers, and animations to work.
        //  The Register call links the property to the WPF
        //  property system — enabling {Binding}, {DynamicResource},
        //  and PropertyChanged callbacks.
        // ══════════════════════════════════════════════════════

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string),
                typeof(NavItem), new PropertyMetadata(""));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(string),
                typeof(NavItem), new PropertyMetadata(""));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool),
                typeof(NavItem), new PropertyMetadata(false, OnIsActiveChanged));

        public static readonly DependencyProperty BadgeCountProperty =
            DependencyProperty.Register(nameof(BadgeCount), typeof(int),
                typeof(NavItem), new PropertyMetadata(0, OnBadgeCountChanged));
		public static readonly RoutedEvent ClickEvent =
	EventManager.RegisterRoutedEvent(
		"Click",
		RoutingStrategy.Bubble,
		typeof(RoutedEventHandler),
		typeof(NavItem));

		public event RoutedEventHandler Click
		{
			add { AddHandler(ClickEvent, value); }
			remove { RemoveHandler(ClickEvent, value); }
		}

		// ── CLR property wrappers ──────────────────────────────
		public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }
        public int BadgeCount
        {
            get => (int)GetValue(BadgeCountProperty);
            set => SetValue(BadgeCountProperty, value);
        }

        public NavItem()
        {
            InitializeComponent();

            // Hover effect — change background on mouse enter/leave
            MouseEnter += (s, e) =>
            {
                if (!IsActive)
                    rootBorder.Background = new SolidColorBrush(
                        System.Windows.Media.Color.FromArgb(15, 255, 255, 255));
            };
            MouseLeave += (s, e) =>
            {
                if (!IsActive)
                    rootBorder.Background = System.Windows.Media.Brushes.Transparent;
            };
			MouseLeftButtonDown += (s, e) =>
			{
				RaiseEvent(new RoutedEventArgs(ClickEvent));
			};
		}

        // ── Property changed callbacks ─────────────────────────
        private static void OnIsActiveChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var nav = (NavItem)d;
            bool active = (bool)e.NewValue;

            if (active)
            {
                // Accent left border + accent text colour
                nav.rootBorder.BorderBrush =
                    (Brush)Application.Current.Resources["AccentBrush"];
                nav.labelText.SetResourceReference(
                    TextBlock.ForegroundProperty, "AccentBrush");
                nav.iconText.SetResourceReference(
                    TextBlock.ForegroundProperty, "AccentBrush");
                nav.rootBorder.Background =
                    new SolidColorBrush(
                        System.Windows.Media.Color.FromArgb(20, 74, 142, 245));
            }
            else
            {
                nav.rootBorder.BorderBrush = System.Windows.Media.Brushes.Transparent;
                nav.rootBorder.Background  = System.Windows.Media.Brushes.Transparent;
                nav.labelText.SetResourceReference(
                    TextBlock.ForegroundProperty, "TextSecondaryBrush");
                nav.iconText.SetResourceReference(
                    TextBlock.ForegroundProperty, "TextSecondaryBrush");
            }
        }

        private static void OnBadgeCountChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var nav   = (NavItem)d;
            int count = (int)e.NewValue;
            nav.badge.Visibility = count > 0 ? Visibility.Visible : Visibility.Collapsed;
            nav.badgeText.Text   = count.ToString();
        }
    }
}
