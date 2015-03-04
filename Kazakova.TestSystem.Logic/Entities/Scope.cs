namespace Kazakova.TestSystem.Logic.Entities
{
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using System;
	using System.Linq;

	internal class Scope
	{
		private ControlGraph graph;
		private IScopeOwner owner;

		public Scope(ControlGraph graph, IScopeOwner owner)
		{
			this.IsAlternative = false;
			this.graph = graph;
			this.owner = owner;
			this.Begin = GetScopeBegin(((ControlGraphItem)owner).Id);
			this.End = GetScopeEnd(this.Begin);
			this.HasValuableItems = CalculateValuableItems();
			this.DetectAnyConditions();
			this.DetectParentScope();
		}

		public Scope(ControlGraph graph, IScopeOwner owner, int index)
		{
			this.IsAlternative = true;
			this.graph = graph;
			this.owner = owner;
			this.Begin = GetScopeBegin(index);
			this.End = GetScopeEnd(this.Begin);
			this.HasValuableItems = CalculateValuableItems();
			this.DetectAnyConditions();
			this.DetectParentScope();
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
					return graph.Where(item => item.Id > End).OfType<IValuable>().OrderBy(x => ((ControlGraphItem)x).Id).ToList().FirstOrDefault();
				}

				if (ParentScopeOwner is IfCgi)
				{
					if (!ParentScope.IsAlternative && ParentScopeOwner is IScopeAlternativeOwner)
					{
						return (ParentScopeOwner as IScopeAlternativeOwner).ScopeAlternative.NextAfterScope;
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
				return graph.Where(item => item.Id > Begin).OfType<IValuable>().OrderBy(x => ((ControlGraphItem)x).Id).ToList().FirstOrDefault();
			}
		}

		public IValuable LastScopeitem
		{
			get
			{
				return graph.Where(item => item.Id < End).OfType<IValuable>().OrderByDescending(x => ((ControlGraphItem)x).Id).ToList().FirstOrDefault();
			}
		}

		public bool IsIncluded(int index)
		{
			return index > Begin && index < End;
		}

		private bool CalculateValuableItems()
		{
			for (int i = Begin; i < End; i++)
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
					throw new ArgumentOutOfRangeException("Программа содержит неверное количество фигурных скобок. Возможно одна из них не находится на отдельной строке. Внимание: Каждая фигурная скобка должна находиться на отдельной строке!");
				}
			} while (graph[index] is LeftBraceCgi);

			return index - 1;
		}

		private int GetScopeEnd(int from)
		{
			int scopes = 1;
			int currentPosition = from;
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
			this.HasNestedConditions = graph.Where(item => item.Id > Begin && item.Id < End).OfType<ICondition>().Any();
		}

		private void DetectParentScope()
		{
			for (int i = ((ControlGraphItem)owner).Id - 1; i > 0; i--)
			{
				if (graph[i] is IScopeOwner)
				{
					if (((IScopeOwner)graph[i]).Scope.IsIncluded(Begin))
					{
						this.ParentScopeOwner = graph[i] as IScopeOwner;
						this.ParentScope = this.ParentScopeOwner.Scope;
						return;
					}

					if (graph[i] is IScopeAlternativeOwner)
					{
						if (((IScopeAlternativeOwner)graph[i]).ScopeAlternative.IsIncluded(Begin))
						{
							this.ParentScopeOwner = graph[i] as IScopeOwner;
							this.ParentScope = (this.ParentScopeOwner as IScopeAlternativeOwner).ScopeAlternative;
							return;
						}
					}
				}
			}

			this.ParentScope = null;
		}
	}
}