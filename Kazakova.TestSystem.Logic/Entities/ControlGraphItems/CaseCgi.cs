namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using Interfaces;

	internal class CaseCgi : ControlGraphItem, IScopeOwner
	{
		public CaseCgi(ControlGraph graph, String content, int id)
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
			throw new NotImplementedException();
		}
	}
}