namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;
	using Interfaces;
	using Scopes;

	internal class IfCgi : Condition, IScopeOwner, IScopeAlternativeOwner
	{
		private const string ConditionRegex = @"if *\((.*)\) *";

		public IfCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
			InitializeExpression(ConditionRegex);
		}

		public override int ValuableBranches
		{
			get
			{
				var branches = 0;
				if (Scope.HasValuableItems)
				{
					branches++;
					if (Scope.HasNestedConditions)
					{
						branches =
							graph.Where(condition => condition.Id > Scope.Begin && condition.Id < Scope.End)
								.OfType<Condition>()
								.Aggregate(branches, (current, nestedCondition) => current*nestedCondition.ValuableBranches);
					}
				}

				if (ScopeAlternative != null && ScopeAlternative.HasValuableItems)
				{
					var tempBranches = 1;
					if (ScopeAlternative.HasNestedConditions)
					{
						tempBranches =
							graph.Where(condition => condition.Id > ScopeAlternative.Begin && condition.Id < ScopeAlternative.End)
								.OfType<Condition>()
								.Aggregate(tempBranches, (current, nestedCondition) => current*nestedCondition.ValuableBranches);
					}

					branches += tempBranches;
				}

				return branches;
			}
		}

		public override bool HasEmptyWay
		{
			get { return ValuableBranches != 2; }
		}

		public ParsedCondition Expression { get; private set; }
		public Scope ScopeAlternative { get; set; }
		public Scope Scope { get; set; }

		public void InitializeScopes()
		{
			Scope = new Scope(graph, this);
			var tempIndex = Scope.End;
			while (!(graph[tempIndex] is IValuable))
			{
				if (graph[tempIndex] is ElseCgi)
				{
					tempIndex++;
					ScopeAlternative = new Scope(graph, this, tempIndex);
				}

				tempIndex++;
				if (tempIndex >= graph.Count)
				{
					return;
				}
			}
		}

		public void InitializeRanges()
		{
			Scope.Range = new Range(Expression);
			if (ScopeAlternative != null)
			{
				ScopeAlternative.Range = new Range(Expression.Revert());
			}
		}

		private void InitializeExpression(string conditionRegex)
		{
			var condition = Regex.Match(content, conditionRegex).Groups[1].Value.Trim();
			Expression = new ParsedCondition(condition);
		}
	}
}