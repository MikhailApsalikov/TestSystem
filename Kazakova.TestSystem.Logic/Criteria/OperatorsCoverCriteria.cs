namespace Kazakova.TestSystem.Logic.Criteria
{
	using Kazakova.TestSystem.Logic.Entities;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class OperatorsCoverCriteria : BaseCriteria
	{
		public OperatorsCoverCriteria(ControlGraph controlGraph)
			: base(controlGraph)
		{
		}

		protected override List<GraphPath> GeneratePathes()
		{
			List<GraphPath> pathes = base.GeneratePathes();
			return RemoveWaste(pathes);
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
			IEnumerable<GraphPath> pathes = AddScope(path, ifCgi.Scope, endIndex);
			if (ifCgi.ScopeAlternative != null)
			{
				pathes = pathes.Union(AddScope(anotherPath, ifCgi.ScopeAlternative, endIndex));
			}
			else
			{
				pathes = pathes.Union(new List<GraphPath>() { anotherPath });
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
			int nextAfterScope = ((ControlGraphItem)switchCgi.Scope.NextAfterScope).Id;
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

			List<GraphPath> result = new List<GraphPath>();
			foreach (var p in pathes)
			{
				result.AddRange(GeneratePathes(p, nextAfterScope, endIndex));
			}

			return result;
		}

		protected override IEnumerable<GraphPath> HandleCycles(GraphPath path, ICycle cycleCgi, int endIndex = Int32.MaxValue)
		{
			return GeneratePathes(path, ((ControlGraphItem)cycleCgi).Id + 1);
		}

		private IEnumerable<GraphPath> AddScope(GraphPath path, Scope scope, int endIndex)
		{
			if (!scope.HasNestedConditions)
			{
				path.AddScope(scope);
				return new List<GraphPath>() { path };
			}

			return GeneratePathes(path, scope.Begin + 1, scope.End);
		}

		private List<GraphPath> RemoveWaste(List<GraphPath> pathes)
		{
			pathes = RemoveSubsets(pathes);
			GraphPath toBeRemoved;
			do
			{
				toBeRemoved = TryRemove(pathes);
				if (toBeRemoved != null)
				{
					pathes.Remove(toBeRemoved);
				}
			}
			while (toBeRemoved != null);
			return pathes;
		}

		private GraphPath TryRemove(List<GraphPath> pathes)
		{
			foreach (var path in pathes)
			{
				if (Check(pathes.Where(p => p != path)))
				{
					return path;
				}
			}

			return null;
		}


		private bool Check(IEnumerable<GraphPath> pathes)
		{
			foreach (var cgi in controlGraph.OfType<IValuable>())
			{
				if (CheckCgi(pathes, cgi) == 0)
				{
					return false;
				}
			}

			return true;
		}

		private int CheckCgi(IEnumerable<GraphPath> pathes, IValuable cgi)
		{
			int result = 0;
			foreach (var path in pathes)
			{
				if (path.Items.Contains(cgi))
				{
					result++;
				}
			}

			return result;
		}

		private List<GraphPath> RemoveSubsets(List<GraphPath> pathes)
		{
			var subsets = new List<GraphPath>();

			foreach (var path in pathes)
			{
				if (IsSubsetForAnything(pathes, path))
				{
					subsets.Add(path);
				}
			}

			return pathes.Except(subsets).ToList();
		}

		private bool IsSubsetForAnything(List<GraphPath> pathes, GraphPath path)
		{
			foreach (var p in pathes)
			{
				if (path.IsSubsetOf(p))
				{
					return true;
				}
			}

			return false;
		}
	}
}