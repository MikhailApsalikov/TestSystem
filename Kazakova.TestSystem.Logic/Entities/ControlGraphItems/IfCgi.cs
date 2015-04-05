﻿namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;
	using Interfaces;

	internal class IfCgi : ControlGraphItem, IScopeOwner, IScopeAlternativeOwner, IValuable, ICondition
	{
		public IfCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}

		public int ValuableBranches
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
								.OfType<ICondition>()
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
								.OfType<ICondition>()
								.Aggregate(tempBranches, (current, nestedCondition) => current*nestedCondition.ValuableBranches);
					}

					branches += tempBranches;
				}

				return branches;
			}
		}

		public bool HasEmptyWay
		{
			get { return ValuableBranches != 2; }
		}

		public string ParsedCondition
		{
			get { return Regex.Match(content, @"if *\((.*)\) *").Groups[1].Value; }
		}

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

		public string ShownId { get; private set; }

		public void SetShownId(int shownId)
		{
			ShownId = shownId.ToString("D2");
		}
	}
}