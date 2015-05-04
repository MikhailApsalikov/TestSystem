namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using Enums;
	using Interfaces;
	using Scopes;

	internal class SwitchCgi : Condition, IValuable, IScopeOwner
	{
		private const string ConditionRegex = @"switch *\((.*)\) *";

		public SwitchCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
			ParseVariable(ConditionRegex);
		}

		private void ParseVariable(string conditionRegex)
		{
			Variable = Regex.Match(content, conditionRegex).Groups[1].Value;
		}

		public string Variable { get; private set; }
		public List<CaseCgi> Cases { get; private set; }
		public DefaultCgi Default { get; private set; }

		public override int ValuableBranches
		{
			get
			{
				var result = 0;
				foreach (var caseitem in Cases.Where(switchItem => switchItem.Scope.HasValuableItems))
				{
					var tempBranches = 1;
					if (caseitem.Scope.HasNestedConditions)
					{
						tempBranches =
							graph.Where(condition => condition.Id > caseitem.Scope.Begin && condition.Id < caseitem.Scope.End)
								.OfType<Condition>()
								.Aggregate(tempBranches, (current, nestedCondition) => current*nestedCondition.ValuableBranches);
					}
					result += tempBranches;
				}

				if (Default != null && Default.Scope.HasValuableItems)
				{
					var tempBranches = 1;
					if (Default.Scope.HasNestedConditions)
					{
						tempBranches =
							graph.Where(condition => condition.Id > Default.Scope.Begin && condition.Id < Default.Scope.End)
								.OfType<Condition>()
								.Aggregate(tempBranches, (current, nestedCondition) => current*nestedCondition.ValuableBranches);
					}
					result += tempBranches;
				}
				return result;
			}
		}

		public override bool HasEmptyWay
		{
			get
			{
				if (Default == null || !Default.Scope.HasValuableItems)
				{
					return true;
				}

				return Cases.Any(item => !item.Scope.HasValuableItems);
			}
		}

		public Scope Scope { get; set; }

		public void InitializeScopes()
		{
			Scope = new Scope(graph, this);
		}

		public void InitializeRanges()
		{
			Scope.Range = Range.CreateFullRange(Variable);
			List<double> caseValues = new List<double>();
			foreach (var caseItem in Cases)
			{
				caseItem.Scope.Range = new Range(new ParsedCondition(Variable, OperationTypes.Equal, caseItem.Value));
				caseValues.Add(caseItem.Value);
			}
			if (Default != null)
			{
				Default.Scope.Range = Range.CreateFullRange(Variable).Except(caseValues);
			}
		}

		private List<CaseCgi> GetCasesForSwitch(out DefaultCgi defaultItem)
		{
			defaultItem =
				(DefaultCgi)
					graph.FirstOrDefault(
						item =>
							((item is DefaultCgi) && item.Id > Scope.Begin && item.Id < Scope.End &&
							 ((IScopeOwner) item).Scope.ParentScopeOwner == this));
			return
				graph.Where(
					item =>
						((item is CaseCgi) && item.Id > Scope.Begin && item.Id < Scope.End &&
						 ((IScopeOwner)item).Scope.ParentScopeOwner == this)).OfType<CaseCgi>().ToList();
		}

		public void InitializeCases()
		{
			DefaultCgi defaultCase;
			Cases = GetCasesForSwitch(out defaultCase);
			Default = defaultCase;
		}
	}
}