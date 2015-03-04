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
			IEnumerable<GraphPath> pathes = AddScope(path, ifCgi.Scope);
			if (ifCgi.ScopeAlternative != null)
			{
				pathes = pathes.Union(AddScope(anotherPath, ifCgi.ScopeAlternative));
				scopeEnd = ifCgi.ScopeAlternative.End;
			}
			else
			{
				scopeEnd = ifCgi.Scope.End;
			}

			IEnumerable<GraphPath> result = new HashSet<GraphPath>();
			foreach (var p in pathes)
			{
				result.Union(GeneratePathes(p, scopeEnd + 1));
			}

			return result;
		}

		private IEnumerable<GraphPath> AddScope(GraphPath path, Scope scope)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<GraphPath> HandleSwitch(GraphPath path, SwitchCgi switchCgi)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<GraphPath> HandleCycles(GraphPath path, ICycle cycleCgi)
		{
			return GeneratePathes(path, ((ControlGraphItem)cycleCgi).Id + 1);
		}

		/*
		private static int HandleIf(bool isFirstIfCase, GraphPath path, IfCgi ifCgi)
		{
			if (isFirstIfCase)
			{
				if (ifCgi.Scope.HasValuableItems)
				{
					path.AddScope(ifCgi.Scope);
				}
				else
				{
					if (ifCgi.ScopeAlternative != null && ifCgi.ScopeAlternative.HasValuableItems)
					{
						path.AddScope(ifCgi.ScopeAlternative);
					}
				}
			}
			else
			{
				if (ifCgi.ScopeAlternative != null && ifCgi.ScopeAlternative.HasValuableItems)
				{
					path.AddScope(ifCgi.ScopeAlternative);
				}
				else
				{
					if (ifCgi.Scope.HasValuableItems)
					{
						path.AddScope(ifCgi.Scope);
					}
				}
			}

			if (ifCgi.ScopeAlternative != null)
			{
				return ifCgi.ScopeAlternative.End;
			}
			else
			{
				return ifCgi.Scope.End;
			}
		}

		private static int HandleSwitch(int index, GraphPath path, SwitchCgi switchCgi)
		{
			var cases = switchCgi.Cases.Where(item => item.Scope.HasValuableItems).ToList();
			if (switchCgi.Default != null && switchCgi.Default.Scope.HasValuableItems)
			{
				cases.Add(switchCgi.Default);
			}

			if (cases.Count > 0)
			{
				if (index < cases.Count)
				{
					path.AddScope(cases[index].Scope);
				}
				else
				{
					path.AddScope(cases[0].Scope);
				}
			}

			return switchCgi.Scope.End;
		}
		 * */
	}
}