namespace Kazakova.TestSystem.Logic.Entities.Scopes
{
	using System.Collections.Generic;
	using ControlGraphItems.Interfaces;

	internal abstract class ScopeBase
	{
		protected readonly ControlGraph graph;
		protected readonly IScopeOwner owner;

		protected ScopeBase(ControlGraph graph, IScopeOwner owner)
		{
			IsAlternative = false;
			this.graph = graph;
			this.owner = owner;
		}

		protected ScopeBase(ControlGraph graph, IScopeOwner owner, int index)
		{
			IsAlternative = true;
			this.graph = graph;
			this.owner = owner;
		}

		public abstract int Begin { get; protected set; }
		public abstract int End { get; protected set; }
		public abstract bool HasValuableItems { get; protected set; }
		public abstract bool HasNestedConditions { get; protected set; }
		public IScopeOwner ParentScopeOwner { get; private set; }
		public bool IsAlternative { get; private set; }
		public Scope ParentScope { get; private set; }
		public Range Range { get; set; }

		protected void DetectParentScope()
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

		public List<ScopeBase> GetParentScopes()
		{
			var result = new List<ScopeBase>
			{
				this
			};
			var scope = this;
			while (scope.ParentScope != null)
			{
				result.Add(scope.ParentScope);
				scope = scope.ParentScope;
			}

			return result;
		}
	}
}