namespace Kazakova.TestSystem.Logic.Entities.Scopes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ControlGraphItems;
	using ControlGraphItems.Interfaces;

	internal sealed class Scope : ScopeBase
	{
		public Scope(ControlGraph graph, IScopeOwner owner)
			: base(graph, owner)
		{
			Begin = GetScopeBegin(((ControlGraphItem) owner).Id);
			End = GetScopeEnd(Begin);
			HasValuableItems = CalculateValuableItems();
			DetectAnyConditions();
			DetectParentScope();
		}

		public Scope(ControlGraph graph, IScopeOwner owner, int index)
			: base(graph, owner, index)
		{
			Begin = GetScopeBegin(index);
			End = GetScopeEnd(Begin);
			HasValuableItems = CalculateValuableItems();
			DetectAnyConditions();
			DetectParentScope();
		}

		public override int Begin { get; protected set; }
		public override int End { get; protected set; }
		public override bool HasValuableItems { get; protected set; }
		public override bool HasNestedConditions { get; protected set; }

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

				var cgi = ParentScopeOwner as IfCgi;
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

		public IEnumerable<IValuable> GetValuableVertexes()
		{
			return graph.Where(i => i.Id>Begin&&i.Id<End).OfType<IValuable>();
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
	}
}