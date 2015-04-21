namespace Kazakova.TestSystem.UI.Desktop.Windows
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using Content;
	using Logic;
	using Logic.Enums;

	/// <summary>
	///     Interaction logic for Criterion.xaml
	/// </summary>
	public partial class CriteriaWindow : Window
	{
		private readonly Criteries criteria;
		private readonly Tester tester;

		public CriteriaWindow(Criteries criteria, Tester tester)
		{
			InitializeComponent();
			this.criteria = criteria;
			this.tester = tester;
			var pathesCount = tester.GetPathCountForCriteria(criteria);

			InitializeFields();
			ShowPathesInList(pathesCount);
			graphLayout.Graph = tester.GetBidirectionGraphForWholeGraph();
		}

		private void InitializeFields()
		{
			Title = StringResources.ProgramName;
			CodeContent.Text = tester.ToString();
			switch (criteria)
			{
				case Criteries.OperatorsCover:
					Header.Content = StringResources.OperatorsCoverCriteria;
					break;
				case Criteries.SolutionsCover:
					Header.Content = StringResources.SolutionsCoverCriteria;
					break;
				case Criteries.ConditionsCover:
					Header.Content = StringResources.SolutionsCoverCriteria;
					break;
				case Criteries.SolutionsAndConditionsCover:
					Header.Content = StringResources.SolutionsAndConditionsCoverCriteria;
					break;
			}
		}

		private void ShowPathesInList(int pathes)
		{
			for (var i = 0; i < pathes; i++)
			{
				PathListBox.Items.Add("Путь #" + (i + 1));
			}
		}

		private void PathListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (PathListBox.SelectedIndex == -1)
			{
				return;
			}

			var pathIndex = PathListBox.SelectedIndex;

			pathLayout.Graph = tester.GetBidirectionGraphForPath(criteria, pathIndex);
			Path.Content = String.Format("Входными данными для этого пути являются: {0}{1}", Environment.NewLine,
				tester.GetRequiredParametersForPathAsString(criteria, pathIndex));
		}
	}
}