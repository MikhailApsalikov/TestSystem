namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;

	internal class SwitchCgi : ControlGraphItem, IValuable, IScopeOwner, ICondition
	{
		public SwitchCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}

		public List<IScopeOwner> Cases { get; private set; }
		public IScopeOwner Default { get; private set; }

		public int ValuableBranches
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
								.OfType<ICondition>()
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
								.OfType<ICondition>()
								.Aggregate(tempBranches, (current, nestedCondition) => current*nestedCondition.ValuableBranches);
					}
					result += tempBranches;
				}
				return result;
			}
		}

		public bool HasEmptyWay
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

		public string ShownId { get; private set; }

		public void SetShownId(int shownId)
		{
			ShownId = shownId.ToString("D2");
		}

		private List<IScopeOwner> GetCasesForSwitch(out IScopeOwner defaultItem)
		{
			defaultItem =
				(IScopeOwner)
					graph.FirstOrDefault(
						item =>
							((item is DefaultCgi) && item.Id > Scope.Begin && item.Id < Scope.End &&
							 ((IScopeOwner) item).Scope.ParentScopeOwner == this));
			return
				graph.Where(
					item =>
						((item is CaseCgi) && item.Id > Scope.Begin && item.Id < Scope.End &&
						 ((IScopeOwner) item).Scope.ParentScopeOwner == this)).OfType<IScopeOwner>().ToList();
		}

		public void InitializeCases()
		{
			IScopeOwner defaultCase;
			Cases = GetCasesForSwitch(out defaultCase);
			Default = defaultCase;
		}
	}
}