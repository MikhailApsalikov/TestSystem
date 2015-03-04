namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class SwitchCgi : ControlGraphItem, IValuable, IScopeOwner, ICondition
	{
		public SwitchCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}

		public Scope Scope { get; set; }

		public string ShownId { get; private set; }

		public List<IScopeOwner> Cases { get; private set; }

		public IScopeOwner Default { get; private set; }

		public void SetShownId(int shownId)
		{
			ShownId = shownId.ToString("D2");
		}

		public int ValuableBranches
		{
			get
			{
				int result = 0;
				foreach (var caseitem in this.Cases.Where(switchItem => switchItem.Scope.HasValuableItems))
				{
					int tempBranches = 1;
					if (caseitem.Scope.HasNestedConditions)
					{
						foreach (var nestedCondition in graph.Where(condition => condition.Id > caseitem.Scope.Begin && condition.Id < caseitem.Scope.End).OfType<ICondition>())
						{
							tempBranches *= nestedCondition.ValuableBranches;
						}
					}
					result += tempBranches;
				}

				if (Default != null && Default.Scope.HasValuableItems)
				{
					int tempBranches = 1;
					if (Default.Scope.HasNestedConditions)
					{
						foreach (var nestedCondition in graph.Where(condition => condition.Id > Default.Scope.Begin && condition.Id < Default.Scope.End).OfType<ICondition>())
						{
							tempBranches *= nestedCondition.ValuableBranches;
						}
					}
					result += tempBranches;
				}
				return result;
			}
		}

		private List<IScopeOwner> GetCasesForSwitch(out IScopeOwner defaultItem)
		{
			defaultItem = (IScopeOwner)graph.FirstOrDefault(item => ((item is DefaultCgi) && item.Id > this.Scope.Begin && item.Id < this.Scope.End && (item as IScopeOwner).Scope.ParentScopeOwner == this));
			return graph.Where(item => ((item is CaseCgi) && item.Id > this.Scope.Begin && item.Id < this.Scope.End && (item as IScopeOwner).Scope.ParentScopeOwner == this)).OfType<IScopeOwner>().ToList();
		}

		public bool HasEmptyWay
		{
			get
			{
				if (Default == null || !Default.Scope.HasValuableItems)
				{
					return true;
				}

				foreach (var item in Cases)
				{
					if (!item.Scope.HasValuableItems)
					{
						return true;
					}
				}

				return false;
			}
		}

		public void InitializeScopes()
		{
			this.Scope = new Scope(graph, this);
		}

		public void InitializeCases()
		{
			IScopeOwner defaultCase;
			this.Cases = GetCasesForSwitch(out defaultCase);
			this.Default = defaultCase;
		}
	}
}