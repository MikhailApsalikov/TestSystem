namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using Scopes;

	internal class DefaultCgi : ControlGraphItem, IScopeOwner
	{
		public DefaultCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}

		public Scope Scope { get; set; }

		public void InitializeScopes()
		{
			Scope = new Scope(graph, this);
		}

		public void InitializeRanges()
		{
		}
	}
}