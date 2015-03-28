using Kazakova.TestSystem.UI.Desktop.Content;
using Kazakova.TestSystem.UI.Desktop.Windows;
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

namespace Kazakova.TestSystem.UI.Desktop
{
	public partial class GreetingsWindow : Window
	{
		InstructionWindow instruction = null;

		public GreetingsWindow()
		{
			this.InitializeComponent();
			this.InitializeFields();
		}

		private void InitializeFields()
		{
			this.Title = StringResources.ProgramName;
			//this.Header.Content = StringResources.GreetingsMessage;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Environment.Exit(0);
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			if (this.instruction == null)
			{
				this.instruction = new InstructionWindow(this);
			}

			this.instruction.Show();
			this.Hide();
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			(new FileLoadWindow(this)).Show();
			this.Hide();
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

  

      
	}
}
