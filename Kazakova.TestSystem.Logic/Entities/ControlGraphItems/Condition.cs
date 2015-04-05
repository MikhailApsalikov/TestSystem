namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using System.Linq.Expressions;
	using System.Text.RegularExpressions;
	using ExpressionEvaluator;
	using Interfaces;

	internal abstract class Condition : ControlGraphItem, IValuable
	{
		private BinaryExpression expression;

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
			var compiledExpression = new CompiledExpression<bool>(condition);
			compiledExpression.ScopeParse();
			expression = compiledExpression.Expression as BinaryExpression;
			if (expression == null)
			{
				throw new ArgumentException(String.Format("Условие в строке {0} не возвращает Boolean", ShownId));
			}
		}
	}
}