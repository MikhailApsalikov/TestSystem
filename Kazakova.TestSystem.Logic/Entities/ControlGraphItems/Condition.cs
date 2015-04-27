namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using Interfaces;

	internal abstract class Condition : ControlGraphItem, IValuable
	{
		internal Condition(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}

		public abstract int ValuableBranches { get; }
		public abstract bool HasEmptyWay { get; }
		public string ShownId { get; private set; }

		public void SetShownId(int shownId)
		{
			ShownId = shownId.ToString("D2");
		}
	}
}