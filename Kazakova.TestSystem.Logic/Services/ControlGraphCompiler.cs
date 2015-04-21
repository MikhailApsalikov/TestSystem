namespace Kazakova.TestSystem.Logic.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Entities;
	using Entities.ControlGraphItems;
	using Entities.ControlGraphItems.Interfaces;
	using QuickGraph;

	internal static class ControlGraphCompiler
	{
		public static BidirectionalGraph<object, IEdge<object>> CompileBidirectionalGraph(ControlGraph сontrolGraph)
		{
			var result = new BidirectionalGraph<object, IEdge<object>>();
			var vertexes = new List<IValuable>();

			vertexes.AddRange(сontrolGraph.OfType<IValuable>());

			foreach (var item in vertexes)
			{
				result.AddVertex(item.ShownId);
			}

			for (var i = 0; i < vertexes.Count - 1; i++)
			{
				AddEdge(result, vertexes[i], vertexes[i + 1]);
			}
			HandleIfConditions(vertexes, result);
			HandleSwitchConstructions(vertexes, result);
			HandleCycles(vertexes, result);
			return result;
		}

		public static IBidirectionalGraph<object, IEdge<object>> CompileBidirectionalGraph(GraphPath graphPath)
		{
			var result = new BidirectionalGraph<object, IEdge<object>>();
			var vertexes = graphPath.Items.ToList();
			foreach (var item in vertexes)
			{
				result.AddVertex(item.ShownId);
			}
			for (var i = 0; i < vertexes.Count - 1; i++)
			{
				AddEdge(result, vertexes[i], vertexes[i + 1]);
			}
			return result;
		}

		private static void HandleIfConditions(List<IValuable> vertexes, BidirectionalGraph<object, IEdge<object>> result)
		{
			foreach (var ifCgi in vertexes.OfType<IfCgi>())
			{
				if (ifCgi.ScopeAlternative == null)
				{
					AddEdge(result, ifCgi, ifCgi.Scope.NextAfterScope);
				}
				else
				{
					if (ifCgi.Scope.HasValuableItems)
					{
						AddEdge(result, ifCgi, ifCgi.ScopeAlternative.FirstScopeitem);
						RemoveEdge(result, ifCgi.Scope.LastScopeitem, ifCgi.ScopeAlternative.FirstScopeitem);
						AddEdge(result, ifCgi.Scope.LastScopeitem, ifCgi.ScopeAlternative.NextAfterScope);
					}
				}
			}
		}

		private static void HandleSwitchConstructions(List<IValuable> vertexes,
			BidirectionalGraph<object, IEdge<object>> result)
		{
			foreach (var switchCgi in vertexes.OfType<SwitchCgi>())
			{
				var afterSwitch = switchCgi.Scope.NextAfterScope;
				for (var j = 0; j < switchCgi.Cases.Count - 1; j++)
				{
					AddEdge(result, switchCgi, switchCgi.Cases[j].Scope.FirstScopeitem);
					RemoveEdge(result, switchCgi.Cases[j].Scope.LastScopeitem, switchCgi.Cases[j + 1].Scope.FirstScopeitem);
					AddEdge(result, switchCgi.Cases[j].Scope.LastScopeitem, afterSwitch);
				}

				if (switchCgi.Cases.Any(item => item.Scope.HasValuableItems))
				{
					var lastCase = switchCgi.Cases.Last();
					AddEdge(result, switchCgi, lastCase.Scope.FirstScopeitem);
					if (switchCgi.Default != null)
					{
						AddEdge(result, switchCgi, switchCgi.Default.Scope.FirstScopeitem);
						RemoveEdge(result, lastCase.Scope.LastScopeitem, switchCgi.Default.Scope.FirstScopeitem);
						AddEdge(result, lastCase.Scope.LastScopeitem, afterSwitch);
					}
					else
					{
						AddEdge(result, switchCgi, afterSwitch);
					}
				}
			}
		}

		private static void HandleCycles(List<IValuable> vertexes, BidirectionalGraph<object, IEdge<object>> result)
		{
			foreach (var cycle in vertexes.OfType<ICycle>())
			{
				AddEdge(result, cycle.ShownId, GetVertexName(cycle.Scope.NextAfterScope));

				foreach (var item in GetPreviousVertexes(result, cycle.Scope.NextAfterScope.ShownId).ToList())
				{
					AddEdge(result, item, GetVertexName(cycle));
				}
			}
		}

		private static String GetVertexName(IValuable item)
		{
			if (item != null)
			{
				return item.ShownId;
			}

			return "Конец";
		}

		private static void AddEdge(BidirectionalGraph<object, IEdge<object>> result, String vertex1, String vertex2,
			bool haveToCheck = true)
		{
			if (haveToCheck &&
			    result.Edges.FirstOrDefault(edge => edge.Source.ToString() == vertex1 && edge.Target.ToString() == vertex2) ==
			    null)
			{
				result.AddEdge(new Edge<object>(vertex1, vertex2));
			}
		}

		private static void AddEdge(BidirectionalGraph<object, IEdge<object>> result, IValuable vertex1, IValuable vertex2,
			bool haveToCheck = true)
		{
			AddEdge(result, GetVertexName(vertex1), GetVertexName(vertex2), haveToCheck);
		}

		private static void RemoveEdge(BidirectionalGraph<object, IEdge<object>> result, String vertex1, String vertex2)
		{
			try
			{
				result.RemoveEdge(
					result.Edges.FirstOrDefault(edge => edge.Source.ToString() == vertex1 && edge.Target.ToString() == vertex2));
			}
			catch
			{
				// ignored
			}
		}

		private static void RemoveEdge(BidirectionalGraph<object, IEdge<object>> result, IValuable vertex1, IValuable vertex2)
		{
			RemoveEdge(result, GetVertexName(vertex1), GetVertexName(vertex2));
		}

		private static IEnumerable<String> GetPreviousVertexes(BidirectionalGraph<object, IEdge<object>> graph, String index)
		{
			return graph.Edges.Where(edge => edge.Target.ToString() == index).Select(edge => edge.Source.ToString());
		}
	}
}