namespace Kazakova.TestSystem.Logic.Criteria
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Entities;
	using Entities.ControlGraphItems;
	using Entities.ControlGraphItems.Interfaces;

	internal class SolutionCoverCriteria : BaseCriteria
	{
		public SolutionCoverCriteria(ControlGraph controlGraph)
			: base(controlGraph)
		{
		}

		protected override List<GraphPath> GeneratePathes()
		{
			var pathes = base.GeneratePathes();
			return RemoveUnreachablePathes(pathes);
		}

		protected override IEnumerable<GraphPath> HandleIf(GraphPath path, IfCgi ifCgi, int endIndex = Int32.MaxValue)
		{
			int nextAfterScope;
			if (ifCgi.ScopeAlternative != null)
			{
				nextAfterScope = ((ControlGraphItem) ifCgi.ScopeAlternative.NextAfterScope).Id;
			}
			else
			{
				nextAfterScope = ((ControlGraphItem) ifCgi.Scope.NextAfterScope).Id;
			}

			if (!ifCgi.Scope.HasValuableItems && (ifCgi.ScopeAlternative == null || !ifCgi.ScopeAlternative.HasValuableItems))
			{
				return GeneratePathes(path, nextAfterScope, endIndex);
			}

			GraphPath anotherPath = (GraphPath)path.Clone();
			var pathes = AddScope(path, ifCgi.Scope);
			if (ifCgi.ScopeAlternative != null)
			{
				pathes = pathes.Union(AddScope(anotherPath, ifCgi.ScopeAlternative));
			}
			else
			{
				pathes = pathes.Union(new List<GraphPath> {anotherPath});
			}

			var result = new List<GraphPath>();
			foreach (var p in pathes)
			{
				result.AddRange(GeneratePathes(p, nextAfterScope, endIndex));
			}

			return result;
		}

		protected override IEnumerable<GraphPath> HandleSwitch(GraphPath path, SwitchCgi switchCgi,
			int endIndex = Int32.MaxValue)
		{
			var nextAfterScope = ((ControlGraphItem) switchCgi.Scope.NextAfterScope).Id;
			IEnumerable<GraphPath> pathes = new List<GraphPath>();
			foreach (var item in switchCgi.Cases.Where(caseItem => caseItem.Scope.HasValuableItems))
			{
				GraphPath anotherPath = (GraphPath)path.Clone();
				pathes = pathes.Union(AddScope(anotherPath, item.Scope));
			}

			if (switchCgi.Default == null || !switchCgi.Default.Scope.HasValuableItems)
			{
				GraphPath anotherPath = (GraphPath)path.Clone();
				pathes = pathes.Union(GeneratePathes(anotherPath, switchCgi.Scope.End + 1, endIndex)).ToList();
			}
			else
			{
				GraphPath anotherPath = (GraphPath)path.Clone();
				pathes = pathes.Union(AddScope(anotherPath, switchCgi.Default.Scope));

				if (switchCgi.Cases.Any(caseItem => !caseItem.Scope.HasValuableItems) && switchCgi.Cases.Any())
				{
					GraphPath thirdPath = (GraphPath)path.Clone();
					pathes = pathes.Union(GeneratePathes(thirdPath, switchCgi.Scope.End + 1, endIndex)).ToList();
				}
			}

			var result = new List<GraphPath>();
			foreach (var p in pathes)
			{
				result.AddRange(GeneratePathes(p, nextAfterScope, endIndex));
			}

			return result;
		}

		protected override IEnumerable<GraphPath> HandleCycles(GraphPath path, ICycle cycleCgi, int endIndex = Int32.MaxValue)
		{
			var nextAfterScope = ((ControlGraphItem) cycleCgi.Scope.NextAfterScope).Id;
			GraphPath anotherPath = (GraphPath)path.Clone();
			var pathes = AddScope(anotherPath, cycleCgi.Scope).ToList();
			pathes.Add((GraphPath)path.Clone());
			var result = new List<GraphPath>();

			foreach (var p in pathes)
			{
				result.AddRange(GeneratePathes(p, nextAfterScope, endIndex));
			}

			return pathes;
		}

		private IEnumerable<GraphPath> AddScope(GraphPath path, Scope scope)
		{
			if (!scope.HasNestedConditions)
			{
				path.AddScope(scope);
				return new List<GraphPath> {path};
			}

			return GeneratePathes(path, scope.Begin + 1, scope.End);
		}
	}
}