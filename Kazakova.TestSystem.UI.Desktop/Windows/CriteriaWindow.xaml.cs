using Kazakova.TestSystem.Logic;
using Kazakova.TestSystem.Logic.Entities;
using Kazakova.TestSystem.Logic.Enums;
using Kazakova.TestSystem.UI.Desktop.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Kazakova.TestSystem.UI.Desktop.Windows
{
	/// <summary>
	/// Interaction logic for Criterion.xaml
	/// </summary>
	public partial class CriteriaWindow : Window
	{
		Criteries criteria;
		Tester tester;

		public CriteriaWindow(Criteries criteria, Tester tester)
		{
			InitializeComponent();
			this.criteria = criteria;
			this.tester = tester;
			var pathesCount = tester.GetPathCountForCriteria(criteria);

			this.InitializeFields();
			this.ShowPathesInList(pathesCount);
			this.graphLayout.Graph = tester.GetBidirectionGraphForWholeGraph();
		}

		private void InitializeFields()
		{
			this.Title = StringResources.ProgramName;
			this.CodeContent.Text = tester.ToString();
			switch (criteria)
			{
				case Criteries.OperatorsCover:
					this.Header.Content = StringResources.OperatorsCoverCriteria;
					break;
				case Criteries.SolutionsCover:
					this.Header.Content = StringResources.SolutionsCoverCriteria;
					break;
				case Criteries.ConditionsCover:
                    this.Header.Content = StringResources.SolutionsCoverCriteria;
					break;
				case Criteries.SolutionsAndConditionsCover:
					this.Header.Content = StringResources.SolutionsAndConditionsCoverCriteria;
					break;
				default:
					break;
			}
		}

		private void ShowPathesInList(int pathes)
		{
			for (int i = 0; i < pathes; i++)
			{
				this.PathListBox.Items.Add("Путь #" + (i + 1).ToString());
			}
		}

		private void PathListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (this.PathListBox.SelectedIndex == -1)
			{
				return;
			}

			this.pathLayout.Graph = tester.GetBidirectionGraphForPath(criteria, this.PathListBox.SelectedIndex);
		}
	}
}