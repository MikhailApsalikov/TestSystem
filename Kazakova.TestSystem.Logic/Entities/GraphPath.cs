namespace Kazakova.TestSystem.Logic.Entities
{
	using System;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.Linq;
	using ControlGraphItems.Interfaces;
	using Scopes;
	using Services.Compiler;

	internal class GraphPath : ICloneable
	{
		private Dictionary<string, Range> ranges;
		private readonly ControlGraph graph;

		public GraphPath(ControlGraph graph)
		{
			this.graph = graph;
			Items = new List<IValuable>();
			Scopes = new List<ScopeBase>();
		}

		public List<ScopeBase> Scopes { get; set; }
		public List<IValuable> Items { get; set; }

		public Dictionary<string, Range> Ranges
		{
			get
			{
				if (ranges == null)
				{
					ranges = new Dictionary<string, Range>();
					foreach (var scope in Scopes)
					{
						UnionRangeDictionaries(ranges, GetRangesForScope(scope));
					}
				}

				return ranges;
			}
		}

		public bool IsValid
		{
			get { return Ranges.All(range => range.Value.OneValue != null); }
		}

		public IValuable this[int index]
		{
			get { return Items[index]; }
		}

		public object Clone()
		{
			var newItems = new List<IValuable>(Items.Count);
			newItems.AddRange(Items);
			return new GraphPath(graph)
			{
				Items = newItems,
				Scopes = Scopes.ToList()
			};
		}

		public override string ToString()
		{
			return String.Join(" - ", Items.Select(item => item.ShownId.ToString()));
		}

		internal void Add(IValuable item)
		{
			Items.Add(item);
		}

		internal void AddScope(ScopeBase scope)
		{
			Scopes.Add(scope);
		}

		internal void AddScope(Scope scope)
		{
			Items.AddRange(graph.Where(item => item.Id < scope.End && item.Id > scope.Begin).OfType<IValuable>());
			Scopes.Add(scope);
		}

		internal bool IsSubsetOf(GraphPath path)
		{
			return new HashSet<IValuable>(Items).IsProperSubsetOf(new HashSet<IValuable>(path.Items));
		}

		internal string GetRequiredParametersAsString()
		{
			if (!Ranges.Any())
			{
				return "Нет параметров";
			}

			if (!IsValid)
			{
				return "Путь недостижим";
			}

			return String.Join("; ",
				Ranges.Select(pair => String.Format("{0} = {1}", pair.Key, pair.Value.OneValue.Value)).ToArray());
		}

		private Dictionary<string, Range> GetRangesForScope(ScopeBase scope)
		{
			var result = new Dictionary<string, Range>();
			if (scope.Range != null)
			{
				result.Add(scope.Range.Variable, scope.Range);
			}

			foreach (var parentScope in scope.GetParentScopes().Where(s=>s.Range!=null))
			{
				ApplyParentRange(result, parentScope.Range);
			}

			return result;
		}

		private void UnionRangeDictionaries(Dictionary<string, Range> main, Dictionary<string, Range> secondary)
		{
			foreach (var range in secondary)
			{
				if (main.ContainsKey(range.Key))
				{
					main[range.Key] = main[range.Key].Intersect(range.Value.ToList());
				}
				else
				{
					main.Add(range.Key, range.Value);
				}
			}
		}

		private void ApplyParentRange(Dictionary<string, Range> first, Range parentRange)
		{
			if (first.ContainsKey(parentRange.Variable))
			{
				first[parentRange.Variable] = first[parentRange.Variable].Intersect(parentRange.ToList());
			}
			else
			{
				first.Add(parentRange.Variable, parentRange);
			}
		}

		public string ExecuteCodeAndGetResultAsString()
		{
			return CodeExecutor.CompileAndExecute(graph, Ranges.Keys.ToList());
		}
	}
}