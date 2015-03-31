namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;

	internal class CommonCgi : ControlGraphItem, IValuable
	{
		public CommonCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}

		public string ShownId { get; protected set; }

		public void SetShownId(int shownId)
		{
			ShownId = shownId.ToString("D2");
		}
	}
}