namespace Kazakova.TestSystem.Logic.Enums
{
	using System;
	using System.ComponentModel;
	using System.Linq;

	public enum Criteries
	{
		[Description("Критерий покрытия операторов")]
		OperatorsCover,

		[Description("Критерий покрытия решений")]
		SolutionsCover,

		[Description("Критерий покрытия условий")]
		ConditionsCover,

		[Description("Критерий покрытия решений/условий")]
		SolutionsAndConditionsCover,
	}
}