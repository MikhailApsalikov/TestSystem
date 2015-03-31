namespace Kazakova.TestSystem.Logic.Entities
{
	using System;

	internal abstract class ControlGraphItem
	{
		protected string content;
		protected ControlGraph graph;

		protected ControlGraphItem(ControlGraph graph, String content, int id)
		{
			this.graph = graph;
			this.content = content;
			Id = id;
		}

		public int Id { get; private set; }

		public override string ToString()
		{
			return content;
		}
	}
}