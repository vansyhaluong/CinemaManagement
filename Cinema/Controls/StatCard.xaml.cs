using System.Windows;
using System.Windows.Controls;

namespace Cinema.Controls
{
    public partial class StatCard : UserControl
    {
        // ══════════════════════════════════════════════════════
        //  DEPENDENCY PROPERTIES
        //  ValueBrushKey is a string (e.g. "SuccessBrush") rather
        //  than a Brush. This lets us call SetResourceReference on
        //  lblValue so it automatically updates when the theme
        //  dictionary swaps — participating in the toggle for free.
        // ══════════════════════════════════════════════════════

        public static readonly DependencyProperty CardLabelProperty =
            DependencyProperty.Register(nameof(CardLabel), typeof(string),
                typeof(StatCard), new PropertyMetadata("", OnCardLabelChanged));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string),
                typeof(StatCard), new PropertyMetadata("", OnValueChanged));

        public static readonly DependencyProperty SubLabelProperty =
            DependencyProperty.Register(nameof(SubLabel), typeof(string),
                typeof(StatCard), new PropertyMetadata("", OnSubLabelChanged));

        public static readonly DependencyProperty ValueBrushKeyProperty =
            DependencyProperty.Register(nameof(ValueBrushKey), typeof(string),
                typeof(StatCard), new PropertyMetadata("TextPrimaryBrush", OnValueBrushKeyChanged));

        // ── CLR wrappers ───────────────────────────────────────
        public string CardLabel
        {
            get => (string)GetValue(CardLabelProperty);
            set => SetValue(CardLabelProperty, value);
        }
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public string SubLabel
        {
            get => (string)GetValue(SubLabelProperty);
            set => SetValue(SubLabelProperty, value);
        }
        public string ValueBrushKey
        {
            get => (string)GetValue(ValueBrushKeyProperty);
            set => SetValue(ValueBrushKeyProperty, value);
        }

        public StatCard()
        {
            InitializeComponent();
        }

        // ── Property changed callbacks ─────────────────────────
        private static void OnCardLabelChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
            => ((StatCard)d).lblLabel.Text = (string)e.NewValue;

        private static void OnValueChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
            => ((StatCard)d).lblValue.Text = (string)e.NewValue;

        private static void OnSubLabelChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
            => ((StatCard)d).lblSub.Text = (string)e.NewValue;

        private static void OnValueBrushKeyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var card = (StatCard)d;
            // SetResourceReference binds the TextBlock's Foreground to the
            // named brush key — so it updates automatically on theme toggle.
            card.lblValue.SetResourceReference(
                TextBlock.ForegroundProperty, (string)e.NewValue);
        }
    }
}
