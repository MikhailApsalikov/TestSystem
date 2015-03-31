namespace Kazakova.TestSystem.Logic.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;

	internal class GraphPath
	{
		private readonly ControlGraph graph;

		public GraphPath(ControlGraph graph)
		{
			this.graph = graph;
			Items = new List<IValuable>();
		}

		public List<IValuable> Items { get; set; }

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
			var newItems = new List<IValuable>(Items.Count);
			newItems.AddRange(Items);
			return new GraphPath(graph)
			{
				Items = newItems
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

		public bool IsSubsetOf(GraphPath path)
		{
			return new HashSet<IValuable>(Items).IsProperSubsetOf(new HashSet<IValuable>(path.Items));
		}
	}
}