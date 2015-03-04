namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using System;
	using System.Linq;

	internal class IfCgi : ControlGraphItem, IScopeOwner, IScopeAlternativeOwner, IValuable, ICondition
	{
		public IfCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}

		public Scope Scope { get; set; }

		public Scope ScopeAlternative { get; set; }

		public string ShownId { get; private set; }

		public void SetShownId(int shownId)
		{
			ShownId = shownId.ToString("D2");
		}

		public int ValuableBranches
		{
			get
			{
				int branches = 0;
				if (Scope.HasValuableItems)
				{
					branches++;
					if (Scope.HasNestedConditions)
					{
						foreach (var nestedCondition in graph.Where(condition=>condition.Id>Scope.Begin&&condition.Id<Scope.End).OfType<ICondition>())
						{
							branches *= nestedCondition.ValuableBranches;
						}
					}
				}

				if (ScopeAlternative != null && ScopeAlternative.HasValuableItems)
				{
					int tempBranches = 1;
					if (ScopeAlternative.HasNestedConditions)
					{
						foreach (var nestedCondition in graph.Where(condition => condition.Id > ScopeAlternative.Begin && condition.Id < ScopeAlternative.End).OfType<ICondition>())
						{
							tempBranches *= nestedCondition.ValuableBranches;
						}
					}

					branches += tempBranches;
				}

				return branches;
			}
		}

		public bool HasEmptyWay
		{
			get
			{
				return this.ValuableBranches != 2;
			}
		}

		public void InitializeScopes()
		{
			this.Scope = new Scope(graph, this);
			int tempIndex = this.Scope.End;
			while (!(graph[tempIndex] is IValuable))
			{
				if (graph[tempIndex] is ElseCgi)
				{
					tempIndex++;
					this.ScopeAlternative = new Scope(graph, this, tempIndex);
				}

				tempIndex++;
				if (tempIndex >= graph.Count)
				{
					return;
				}
			}
		}
	}
}