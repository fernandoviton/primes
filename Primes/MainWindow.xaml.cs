using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Primes
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			const int totalRows = 20;
			const int totalColumns = 20;
			const int totalNumbers = totalRows * (totalColumns - 1) + 2;
			var _lastMultiple = 1;

			var uiMap = new Dictionary<int, Button>();

			for (var row = 0; row < totalRows; row++)
			{
				var rowContainer = new StackPanel
				{
					Orientation = Orientation.Horizontal,
				};
				rowContainer.SetValue(Grid.RowProperty, row);

				for (var column = 0; column < totalColumns; column++)
				{
					var number = row * totalColumns + column + 2;
					var numberUi = MakeNumberUi(number);
					numberUi.Click += (_, __) =>
					{
						DeemphizeMultiple(totalNumbers, uiMap, number);
					};
					rowContainer.Children.Add(numberUi);
					uiMap[number] = numberUi;
					EmphisizeNumberUi(numberUi);
				}

				NumberContainer.RowDefinitions.Add(new RowDefinition());
				NumberContainer.Children.Add(rowContainer);
			}

			var timer = new DispatcherTimer();
			timer.Tick += (_, __) => _lastMultiple = DeemphisizeNextMultiple(uiMap, _lastMultiple, totalNumbers);
			timer.Interval = TimeSpan.FromSeconds(2);
			timer.Start();
		}

		private static void DeemphizeMultiple(int totalNumbers, Dictionary<int, Button> uiMap, int number)
		{
			var numberUi = uiMap[number];
			if (!numberUi.GetValue(FontWeightProperty).Equals(FontWeights.UltraLight))
				ReallyEmphisizeNumberUi(numberUi);
			DeemphisizeMultiples(uiMap, number, totalNumbers);
		}

		private static int DeemphisizeNextMultiple(Dictionary<int, Button> uiMap, int lastMultiple, int totalNumbers)
		{
			while (++lastMultiple < totalNumbers)
			{
				if (!IsDeemphisizedNumberUi(uiMap[lastMultiple]))
				{
					DeemphizeMultiple(totalNumbers, uiMap, lastMultiple);
					return lastMultiple;
				}
			}

			return lastMultiple-1;
		}

		private static void DeemphisizeMultiples(Dictionary<int, Button> uiMap, int multiple, int maxNumber)
		{
			var numbers = Enumerable.Range(1, maxNumber / multiple + 1).Select(n => n * multiple);
			foreach (var number in numbers.Skip(1))
				if (uiMap.ContainsKey(number))
					DeemphisizeNumberUi(uiMap[number]);
		}

		private static Button MakeNumberUi(int number) => new Button
		{
			Content = number.ToString(),
			Height = 30,
			Width = 30,
		};

		private static void DeemphisizeNumberUi(DependencyObject element)
			=> element.SetValue(FontWeightProperty, FontWeights.UltraLight);

		private static void EmphisizeNumberUi(DependencyObject element)
			=> element.SetValue(FontWeightProperty, FontWeights.Bold);

		private static void ReallyEmphisizeNumberUi(DependencyObject element)
			=> element.SetValue(FontSizeProperty, 16.0);

		private static bool IsDeemphisizedNumberUi(DependencyObject element)
			=> element.GetValue(FontWeightProperty).Equals(FontWeights.UltraLight);
	}
}
