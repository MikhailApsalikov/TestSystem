namespace Kazakova.TestSystem.Logic.Criteria
{
	using Kazakova.TestSystem.Logic.Entities;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class ConditionCoverCriteria : BaseCriteria
	{
		public ConditionCoverCriteria(ControlGraph controlGraph)
			: base(controlGraph)
		{
		}

		protected override IEnumerable<GraphPath> HandleIf(GraphPath path, IfCgi ifCgi, int endIndex = Int32.MaxValue)
		{
			int nextAfterScope;
			if (ifCgi.ScopeAlternative != null)
			{
				nextAfterScope = ((ControlGraphItem)ifCgi.ScopeAlternative.NextAfterScope).Id;
			}
			else
			{
				nextAfterScope = ((ControlGraphItem)ifCgi.Scope.NextAfterScope).Id;
			}

			if (!ifCgi.Scope.HasValuableItems && (ifCgi.ScopeAlternative == null || !ifCgi.ScopeAlternative.HasValuableItems))
			{
				return GeneratePathes(path, nextAfterScope, endIndex);
			}

			var anotherPath = path.Clone();
			IEnumerable<GraphPath> pathes = AddScope(path, ifCgi.Scope, nextAfterScope, endIndex);
			if (ifCgi.ScopeAlternative != null)
			{
				pathes = pathes.Union(AddScope(anotherPath, ifCgi.ScopeAlternative, nextAfterScope, endIndex));
			}

			List<GraphPath> result = new List<GraphPath>();
			foreach (var p in pathes)
			{
				result.AddRange(GeneratePathes(p, nextAfterScope, endIndex));
			}

			return result;
		}

		protected override IEnumerable<GraphPath> HandleSwitch(GraphPath path, SwitchCgi switchCgi, int endIndex = Int32.MaxValue)
		{
			List<GraphPath> pathes = new List<GraphPath>();
			foreach (var item in switchCgi.Cases.Where(caseItem => caseItem.Scope.HasValuableItems))
			{
				var anotherPath = path.Clone();
				anotherPath.AddScope(item.Scope);
				pathes = pathes.Union(GeneratePathes(anotherPath, switchCgi.Scope.End + 1, endIndex)).ToList();
			}

			if (switchCgi.Default == null || !switchCgi.Default.Scope.HasValuableItems)
			{
				var anotherPath = path.Clone();
				pathes = pathes.Union(GeneratePathes(anotherPath, switchCgi.Scope.End + 1, endIndex)).ToList();
			}
			else
			{
				var anotherPath = path.Clone();
				anotherPath.AddScope(switchCgi.Default.Scope);
				pathes = pathes.Union(GeneratePathes(anotherPath, switchCgi.Scope.End + 1, endIndex)).ToList();

				if (switchCgi.Cases.Any(caseItem => !caseItem.Scope.HasValuableItems) && switchCgi.Cases.Any())
				{
					var thirdPath = path.Clone();
					pathes = pathes.Union(GeneratePathes(thirdPath, switchCgi.Scope.End + 1, endIndex)).ToList();
				}
			}
			return pathes;
		}

		protected override IEnumerable<GraphPath> HandleCycles(GraphPath path, ICycle cycleCgi, int endIndex = Int32.MaxValue)
		{
			var anotherPath = path.Clone();
			anotherPath.AddScope(cycleCgi.Scope);
			var branch1 = GeneratePathes(path, cycleCgi.Scope.End + 1, endIndex);
			var branch2 = GeneratePathes(anotherPath, cycleCgi.Scope.End + 1, endIndex);
			return branch1.Union(branch2).ToList();
		}

		private IEnumerable<GraphPath> AddScope(GraphPath path, Scope scope, int nextAfterScope, int endIndex)
		{
			if (!scope.HasNestedConditions)
			{
				path.AddScope(scope);
				return new List<GraphPath>() { path };
			}

			return GeneratePathes(path, scope.Begin + 1, scope.End);
		}
	}
}