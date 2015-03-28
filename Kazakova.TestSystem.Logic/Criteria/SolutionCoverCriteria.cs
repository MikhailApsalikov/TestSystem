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

			var anotherPath = path.Clone();
			var pathes = AddScope(path, ifCgi.Scope, endIndex);
			if (ifCgi.ScopeAlternative != null)
			{
				pathes = pathes.Union(AddScope(anotherPath, ifCgi.ScopeAlternative, endIndex));
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
				var anotherPath = path.Clone();
				pathes = pathes.Union(AddScope(anotherPath, item.Scope, endIndex));
			}

			if (switchCgi.Default == null || !switchCgi.Default.Scope.HasValuableItems)
			{
				var anotherPath = path.Clone();
				pathes = pathes.Union(GeneratePathes(anotherPath, switchCgi.Scope.End + 1, endIndex)).ToList();
			}
			else
			{
				var anotherPath = path.Clone();
				pathes = pathes.Union(AddScope(anotherPath, switchCgi.Default.Scope, endIndex));

				if (switchCgi.Cases.Any(caseItem => !caseItem.Scope.HasValuableItems) && switchCgi.Cases.Any())
				{
					var thirdPath = path.Clone();
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
			var anotherPath = path.Clone();
			var pathes = AddScope(anotherPath, cycleCgi.Scope, endIndex).ToList();
			pathes.Add(path.Clone());
			var result = new List<GraphPath>();

			foreach (var p in pathes)
			{
				result.AddRange(GeneratePathes(p, nextAfterScope, endIndex));
			}

			return pathes;
		}

		private IEnumerable<GraphPath> AddScope(GraphPath path, Scope scope, int endIndex)
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