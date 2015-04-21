namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using System.Linq.Expressions;
	using System.Text.RegularExpressions;
	using Interfaces;

	internal abstract class Condition : ControlGraphItem, IValuable
	{
		public ParsedCondition Expression { get; private set; }

		internal Condition(ControlGraph graph, String content, int id, string conditionRegex)
			: base(graph, content, id)
		{
			InitializeExpression(conditionRegex);
		}

		public abstract int ValuableBranches { get; }
		public abstract bool HasEmptyWay { get; }
		public string ShownId { get; private set; }

		public void SetShownId(int shownId)
		{
			ShownId = shownId.ToString("D2");
		}

		private void InitializeExpression(string conditionRegex)
		{
			var condition = Regex.Match(content, conditionRegex).Groups[1].Value;
			Expression = new ParsedCondition(condition);
		}
	}
}