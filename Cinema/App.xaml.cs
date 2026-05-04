using System.Configuration;
using System.Data;
using System.Windows;

namespace Cinema
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private bool _isDark = true;

		public void ThemeToggle()
		{
			_isDark = !_isDark;

			// ── The three lines that power the entire toggle ──
			var dict = new ResourceDictionary
			{
				Source = new Uri(
					_isDark
						? "Themes/Dark.xaml"
						: "Themes/Light.xaml",
					UriKind.Relative)
			};

			// MergedDictionaries[0] = the active theme slot.
			// Clear + Add swaps the entire colour palette in one step.
			Resources.MergedDictionaries.Clear();
			Resources.MergedDictionaries.Add(dict);
		}
	}

}
