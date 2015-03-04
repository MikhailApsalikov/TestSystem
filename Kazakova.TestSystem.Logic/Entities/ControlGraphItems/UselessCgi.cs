namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using System.Linq;

	internal class UselessCgi : ControlGraphItem
	{
		public UselessCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}
	}
}