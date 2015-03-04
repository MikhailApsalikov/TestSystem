namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using System;
	using System.Linq;

	internal class ForCgi : ControlGraphItem, IScopeOwner, IValuable, ICycle
	{
		public ForCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}

		public Scope Scope { get; set; }

		public string ShownId { get; private set; }

		public void SetShownId(int shownId)
		{
			ShownId = shownId.ToString("D2");
		}

		public void InitializeScopes()
		{
			this.Scope = new Scope(graph, this);
		}
	}
}