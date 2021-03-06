﻿namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using Interfaces;
	using Scopes;

	internal class WhileCgi : ControlGraphItem, IScopeOwner, IValuable, ICycle
	{
		public WhileCgi(ControlGraph graph, String content, int id)
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

		public string ShownId { get; private set; }

		public void SetShownId(int shownId)
		{
			ShownId = shownId.ToString("D2");
		}
	}
}