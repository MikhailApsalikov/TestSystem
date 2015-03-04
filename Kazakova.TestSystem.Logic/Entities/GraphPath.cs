namespace Kazakova.TestSystem.Logic.Entities
{
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class GraphPath
	{
		private ControlGraph graph;

		public List<IValuable> Items { get; set; }

		public GraphPath(ControlGraph graph)
		{
			this.graph = graph;
			Items = new List<IValuable>();
		}

		public IValuable this[int index]
		{
			get { return Items[index]; }
		}

		public override string ToString()
		{
			return String.Join(" - ", Items.Select(item => item.ShownId.ToString()));
		}

		public GraphPath Clone()
		{
			List<IValuable> newItems = new List<IValuable>(Items.Count);
			foreach (var item in Items)
			{
				newItems.Add(item);
			}
			return new GraphPath(graph)
			{
				Items = newItems,
			};
		}

		internal void Add(IValuable item)
		{
			Items.Add(item);
		}

		internal void AddScope(Scope scope)
		{
			Items.AddRange(graph.Where(item => item.Id < scope.End && item.Id > scope.Begin).OfType<IValuable>());
		}
	}
}