namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using System;
	using System.Linq;

	internal class CaseCgi : ControlGraphItem, IScopeOwner
	{
		public CaseCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}

		public Scope Scope { get; set; }

		public void InitializeScopes()
		{
			this.Scope = new Scope(graph, this);
		}
	}
}