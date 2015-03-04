namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using System.Linq;

	internal class ElseCgi : ControlGraphItem
	{
		public ElseCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}
	}
}