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

		protected override IEnumerable<GraphPath> HandleIf(GraphPath path, IfCgi ifCgi)
		{
			int scopeEnd;
			if (!ifCgi.Scope.HasValuableItems && (ifCgi.ScopeAlternative == null || !ifCgi.ScopeAlternative.HasValuableItems))
			{
				if (ifCgi.ScopeAlternative != null)
				{
					scopeEnd = ifCgi.ScopeAlternative.End;
				}
				else
				{
					scopeEnd = ifCgi.Scope.End;
				}
				return GeneratePathes(path, scopeEnd + 1);
			}

			var anotherPath = path.Clone();
			IEnumerable<GraphPath> pathes = AddScope(path,ifCgi.Scope);
			if (ifCgi.ScopeAlternative != null)
			{
				pathes = pathes.Union(AddScope(anotherPath, ifCgi.ScopeAlternative));
				scopeEnd = ifCgi.ScopeAlternative.End;
			}
			else
			{
				scopeEnd = ifCgi.Scope.End;
			}

			List<GraphPath> result = new List<GraphPath>();
			foreach (var p in pathes)
			{
				result.AddRange(GeneratePathes(p, scopeEnd + 1));
			}

			return result;
		}

		protected override IEnumerable<GraphPath> HandleSwitch(GraphPath path, SwitchCgi switchCgi)
		{
			List<GraphPath> pathes = new List<GraphPath>();
			foreach (var item in switchCgi.Cases.Where(caseItem => caseItem.Scope.HasValuableItems))
			{
				var anotherPath = path.Clone();
				anotherPath.AddScope(item.Scope);
				pathes = pathes.Union(GeneratePathes(anotherPath, switchCgi.Scope.End + 1)).ToList();
			}

			if (switchCgi.Default == null || !switchCgi.Default.Scope.HasValuableItems)
			{
				var anotherPath = path.Clone();
				pathes = pathes.Union(GeneratePathes(anotherPath, switchCgi.Scope.End + 1)).ToList();
			}
			else
			{
				var anotherPath = path.Clone();
				anotherPath.AddScope(switchCgi.Default.Scope);
				pathes = pathes.Union(GeneratePathes(anotherPath, switchCgi.Scope.End + 1)).ToList();

				if (switchCgi.Cases.Any(caseItem => !caseItem.Scope.HasValuableItems) && switchCgi.Cases.Any())
				{
					var thirdPath = path.Clone();
					pathes = pathes.Union(GeneratePathes(thirdPath, switchCgi.Scope.End + 1)).ToList();
				}
			}
			return pathes;
		}

		protected override IEnumerable<GraphPath> HandleCycles(GraphPath path, ICycle cycleCgi)
		{
			var anotherPath = path.Clone();
			anotherPath.AddScope(cycleCgi.Scope);
			var branch1 = GeneratePathes(path, cycleCgi.Scope.End + 1);
			var branch2 = GeneratePathes(anotherPath, cycleCgi.Scope.End + 1);
			return branch1.Union(branch2).ToList();
		}

		private IEnumerable<GraphPath> AddScope(GraphPath path, Scope scope)
		{
			if (!scope.HasNestedConditions)
			{
				path.AddScope(scope);
				return GeneratePathes(path, ((ControlGraphItem)scope.NextAfterScope).Id);
			}

			return GeneratePathes(path, scope.Begin + 1, scope.End);
		}
	}
}