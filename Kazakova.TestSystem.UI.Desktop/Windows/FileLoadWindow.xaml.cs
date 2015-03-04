using Kazakova.TestSystem.Logic;
using Kazakova.TestSystem.Logic.Enums;
using Kazakova.TestSystem.Logic.Services;
using Kazakova.TestSystem.UI.Desktop.Content;
using Microsoft.Win32;
using QuickGraph;
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
using System.Windows.Shapes;

namespace Kazakova.TestSystem.UI.Desktop.Windows
{
	public partial class FileLoadWindow : Window
	{
		private GreetingsWindow greetingsWindow = null;
		private Tester logic = null;

		public FileLoadWindow(GreetingsWindow greetingsWindow)
		{
			this.greetingsWindow = greetingsWindow;
			this.InitializeComponent();
			this.InitializeFields();
		}

		private void InitializeFields()
		{
			this.Title = StringResources.ProgramName;
			this.Header.Content = StringResources.FileLoadHeader;
			this.Text.Content = StringResources.fileLoadText;

			this.OperatorsCover.IsEnabled = false;
			this.ConditionsCover.IsEnabled = false;
			this.SolutionsAndConditionsCover.IsEnabled = false;
			this.SolutionsCover.IsEnabled = false;

			this.OperatorsCover.Content = Criteries.OperatorsCover.GetString();
			this.ConditionsCover.Content = Criteries.ConditionsCover.GetString();
			this.SolutionsAndConditionsCover.Content = Criteries.SolutionsAndConditionsCover.GetString();
			this.SolutionsCover.Content = Criteries.SolutionsCover.GetString();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			greetingsWindow.Show();
			this.Hide();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			String fileContent = "";
			try
			{
				String path = null;
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.AddExtension = true;
				ofd.CheckFileExists = true;
				ofd.CheckPathExists = true;
				ofd.DefaultExt = ".cs";
				ofd.Filter = "C# source code|*.cs|Java source code|*.java|Text files|*.txt";
				if (ofd.ShowDialog() ?? false)
				{
					path = ofd.FileName;
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
				Tester logic = new Tester(fileContent);
				this.setLogic(logic);
#if !DEBUG
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка логики. Причина: " + ex.Message, "Ошибка логики");
			}
#endif
		}

		private void setLogic(Tester logic)
		{
			this.logic = logic;
			if (logic != null)
			{
				this.ShowText(logic);
				this.graphLayout.Graph = logic.GetBidirectionGraphForWholeGraph();
				this.OperatorsCover.IsEnabled = true;
				this.ConditionsCover.IsEnabled = true;
				this.SolutionsAndConditionsCover.IsEnabled = true;
				this.SolutionsCover.IsEnabled = true;
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (MessageBox.Show(StringResources.DoYouReallyWannaQuit, StringResources.Exit, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				Environment.Exit(0);
			}
			else
			{
				e.Cancel = true;
			}
		}

		private void ShowText(Tester content)
		{
			this.CodeContent.Text = content.ToString();
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
