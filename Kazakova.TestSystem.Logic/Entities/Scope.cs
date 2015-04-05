namespace Kazakova.TestSystem.Logic.Entities
{
	using System;
	using System.Linq;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;

	internal class Scope
	{
		private readonly ControlGraph graph;
		private readonly IScopeOwner owner;

		public Scope(ControlGraph graph, IScopeOwner owner)
		{
			IsAlternative = false;
			this.graph = graph;
			this.owner = owner;
			Begin = GetScopeBegin(((ControlGraphItem) owner).Id);
			End = GetScopeEnd(Begin);
			HasValuableItems = CalculateValuableItems();
			DetectAnyConditions();
			DetectParentScope();
		}

		public Scope(ControlGraph graph, IScopeOwner owner, int index)
		{
			IsAlternative = true;
			this.graph = graph;
			this.owner = owner;
			Begin = GetScopeBegin(index);
			End = GetScopeEnd(Begin);
			HasValuableItems = CalculateValuableItems();
			DetectAnyConditions();
			DetectParentScope();
		}

		public int Begin { get; private set; }
		public int End { get; private set; }
		public bool HasValuableItems { get; private set; }
		public IScopeOwner ParentScopeOwner { get; private set; }
		public bool IsAlternative { get; set; }
		public Scope ParentScope { get; private set; }
		public bool HasNestedConditions { get; private set; }

		public IValuable NextAfterScope
		{
			get
			{
				if (ParentScope == null || graph.Where(item => item.Id > End && item.Id < ParentScope.End).OfType<IValuable>().Any())
				{
					return
						graph.Where(item => item.Id > End)
							.OfType<IValuable>()
							.OrderBy(x => ((ControlGraphItem) x).Id)
							.ToList()
							.FirstOrDefault();
				}

				IfCgi cgi = ParentScopeOwner as IfCgi;
				if (cgi != null)
				{
					if (!ParentScope.IsAlternative)
					{
						return cgi.ScopeAlternative.NextAfterScope;
					}
				}

				if (ParentScopeOwner is CaseCgi || ParentScopeOwner is DefaultCgi)
				{
					return ParentScopeOwner.Scope.ParentScopeOwner.Scope.NextAfterScope;
				}

				return ParentScope.NextAfterScope;
			}
		}

		public IValuable FirstScopeitem
		{
			get
			{
				return
					graph.Where(item => item.Id > Begin)
						.OfType<IValuable>()
						.OrderBy(x => ((ControlGraphItem) x).Id)
						.ToList()
						.FirstOrDefault();
			}
		}

		public IValuable LastScopeitem
		{
			get
			{
				return
					graph.Where(item => item.Id < End)
						.OfType<IValuable>()
						.OrderByDescending(x => ((ControlGraphItem) x).Id)
						.ToList()
						.FirstOrDefault();
			}
		}

		public bool IsIncluded(int index)
		{
			return index > Begin && index < End;
		}

		private bool CalculateValuableItems()
		{
			for (var i = Begin; i < End; i++)
			{
				if (graph[i] is IValuable)
				{
					return true;
				}
			}

			return false;
		}

		private int GetScopeBegin(int index)
		{
			do
			{
				index++;
				if (index >= graph.Count)
				{
					throw new ArgumentOutOfRangeException(
						"Программа содержит неверное количество фигурных скобок. Возможно одна из них не находится на отдельной строке. Внимание: Каждая фигурная скобка должна находиться на отдельной строке!");
				}
			} while (graph[index] is LeftBraceCgi);

			return index - 1;
		}

		private int GetScopeEnd(int from)
		{
			var scopes = 1;
			var currentPosition = from;
			while (scopes != 0)
			{
				currentPosition++;
				if (graph[currentPosition] is LeftBraceCgi)
				{
					scopes++;
				}

				if (graph[currentPosition] is RightBraceCgi)
				{
					scopes--;
				}
			}

			return currentPosition;
		}

		private void DetectAnyConditions()
		{
			HasNestedConditions = graph.Where(item => item.Id > Begin && item.Id < End).OfType<Condition>().Any();
		}

		private void DetectParentScope()
		{
			for (var i = ((ControlGraphItem) owner).Id - 1; i > 0; i--)
			{
				if (graph[i] is IScopeOwner)
				{
					if (((IScopeOwner) graph[i]).Scope.IsIncluded(Begin))
					{
						ParentScopeOwner = graph[i] as IScopeOwner;
						ParentScope = ParentScopeOwner.Scope;
						return;
					}

					if (graph[i] is IScopeAlternativeOwner)
					{
						var scopeAlternative = ((IScopeAlternativeOwner) graph[i]).ScopeAlternative;
						if (scopeAlternative == null)
						{
							return;
						}

						if (scopeAlternative.IsIncluded(Begin))
						{
							ParentScopeOwner = graph[i] as IScopeOwner;
							ParentScope = (ParentScopeOwner as IScopeAlternativeOwner).ScopeAlternative;
							return;
						}
					}
				}
			}

			ParentScope = null;
		}
	}
}