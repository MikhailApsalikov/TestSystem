namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using System;
	using System.Linq;

	internal class BeginEndCgi : CommonCgi
	{
		public BeginEndCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}

		public void SetShownId(string shownId)
		{
			ShownId = shownId;
		}
	}
}