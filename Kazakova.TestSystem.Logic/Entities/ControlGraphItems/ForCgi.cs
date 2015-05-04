namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using Scopes;

	internal class ForCgi : ControlGraphItem, IScopeOwner, IValuable, ICycle
	{
		public ForCgi(ControlGraph graph, String content, int id)
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

		public string ShownId { get; private set; }

		public void SetShownId(int shownId)
		{
			ShownId = shownId.ToString("D2");
		}
	}
}