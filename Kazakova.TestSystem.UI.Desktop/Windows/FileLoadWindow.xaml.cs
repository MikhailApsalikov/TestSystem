namespace Kazakova.TestSystem.UI.Desktop.Windows
{
	using System;
	using System.ComponentModel;
	using System.Windows;
	using Content;
	using Logic;
	using Logic.Enums;
	using Logic.Fake;
	using Logic.Services;
	using Microsoft.Win32;

	public partial class FileLoadWindow : Window
	{
		private ITester logic;
		private readonly GreetingsWindow greetingsWindow;

		public FileLoadWindow(GreetingsWindow greetingsWindow)
		{
			this.greetingsWindow = greetingsWindow;
			InitializeComponent();
			InitializeFields();
		}

		private void InitializeFields()
		{
			Title = StringResources.ProgramName;
			Header.Content = StringResources.FileLoadHeader;
			Text.Content = StringResources.fileLoadText;

			OperatorsCover.IsEnabled = false;
			ConditionsCover.IsEnabled = false;
			SolutionsAndConditionsCover.IsEnabled = false;
			SolutionsCover.IsEnabled = false;

			OperatorsCover.Content = Criteries.OperatorsCover.GetString();
			ConditionsCover.Content = Criteries.ConditionsCover.GetString();
			SolutionsAndConditionsCover.Content = Criteries.SolutionsAndConditionsCover.GetString();
			SolutionsCover.Content = Criteries.SolutionsCover.GetString();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			greetingsWindow.Show();
			Hide();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var fileContent = "";
			try
			{
				var ofd = new OpenFileDialog
				{
					AddExtension = true,
					CheckFileExists = true,
					CheckPathExists = true,
					DefaultExt = ".cs",
					Filter = "C# source code|*.cs|Java source code|*.java|Text files|*.txt"
				};
				if (ofd.ShowDialog() ?? false)
				{
					var path = ofd.FileName;
					fileContent = FileManager.Load(path);
				}
				else
				{
					return;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Не удалось загрузить файл. Причина: " + ex.Message, "Ошибка загрузки файла");
			}

#if !DEBUG
			try
			{
#endif
			var logic = FakeTester.IsFake(fileContent) ? new FakeTester(fileContent) : new Tester(fileContent) as ITester;
			SetLogic(logic);
#if !DEBUG
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка логики. Причина: " + ex.Message, "Ошибка логики");
			}
#endif
		}

		private void SetLogic(ITester logic)
		{
			this.logic = logic;
			if (logic != null)
			{
				ShowText(logic);
				graphLayout.Graph = logic.GetBidirectionGraphForWholeGraph();
				OperatorsCover.IsEnabled = true;
				ConditionsCover.IsEnabled = true;
				SolutionsAndConditionsCover.IsEnabled = true;
				SolutionsCover.IsEnabled = true;
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (MessageBox.Show(StringResources.DoYouReallyWannaQuit, StringResources.Exit, MessageBoxButton.YesNo) ==
			    MessageBoxResult.Yes)
			{
				Environment.Exit(0);
			}
			else
			{
				e.Cancel = true;
			}
		}

		private void ShowText(ITester content)
		{
			CodeContent.Text = content.ToString();
		}

		private void OperatorsCover_Click(object sender, RoutedEventArgs e)
		{
			(new CriteriaWindow(Criteries.OperatorsCover, logic)).Show();
		}

		private void SolutionsCover_Click(object sender, RoutedEventArgs e)
		{
			(new CriteriaWindow(Criteries.SolutionsCover, logic)).Show();
		}

		private void ConditionsCover_Click(object sender, RoutedEventArgs e)
		{
			(new CriteriaWindow(Criteries.ConditionsCover, logic)).Show();
		}

		private void SolutionsAndConditionsCover_Click(object sender, RoutedEventArgs e)
		{
			(new CriteriaWindow(Criteries.SolutionsAndConditionsCover, logic)).Show();
		}
	}
}