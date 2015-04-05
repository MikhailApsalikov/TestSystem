namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;

	internal abstract class Condition : ControlGraphItem
	{
		internal Condition(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
		}

		public abstract int ValuableBranches { get; }
		public abstract bool HasEmptyWay { get; }
	}
}