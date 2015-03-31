namespace Kazakova.TestSystem.Logic.Enums
{
	using System.ComponentModel;

	public enum Criteries
	{
		[Description("Критерий покрытия операторов")] OperatorsCover,

		[Description("Критерий покрытия решений")] SolutionsCover,

		[Description("Критерий покрытия условий")] ConditionsCover,

		[Description("Критерий покрытия решений/условий")] SolutionsAndConditionsCover
	}
}