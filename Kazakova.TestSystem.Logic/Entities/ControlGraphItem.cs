namespace Kazakova.TestSystem.Logic.Entities
{
	using System;

	internal abstract class ControlGraphItem
	{
		protected ControlGraph graph;
		protected string content;

		public ControlGraphItem(ControlGraph graph, String content, int id)
		{
			this.graph = graph;
			this.content = content;
			this.Id = id;
		}

		public int Id { get; private set; }

		public override string ToString()
		{
			return this.content;
		}
	}
}