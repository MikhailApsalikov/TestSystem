namespace Kazakova.TestSystem.Logic
{
	using System;
	using System.Collections.Generic;
	using Kazakova.TestSystem.Logic.Criteria;
	using Kazakova.TestSystem.Logic.Entities;
	using Kazakova.TestSystem.Logic.Enums;
	using Kazakova.TestSystem.Logic.Services;
	using QuickGraph;

	public class Tester
	{
		private readonly ControlGraph controlGraph;
		internal Dictionary<Criteries, BaseCriteria> criteries = new Dictionary<Criteries, BaseCriteria>();

		public Tester(string data)
		{
			controlGraph = new ControlGraph(data);
		}

		public BidirectionalGraph<object, IEdge<object>> GetBidirectionGraphForWholeGraph()
		{
			return ControlGraphCompiler.CompileBidirectionalGraph(controlGraph);
		}

		public int GetPathCountForCriteria(Criteries criteria)
		{
			CheckPathCache(criteria);
			return criteries[criteria].GetPathes().Count;
		}

		public IBidirectionalGraph<object, IEdge<object>> GetBidirectionGraphForPath(Criteries criteria, int pathId)
		{
			CheckPathCache(criteria);
			return GetPathForDrawing(criteries[criteria].GetPathes()[pathId]);
		}

		private void CheckPathCache(Criteries criteria)
		{
			if (!criteries.ContainsKey(criteria))
			{
				switch (criteria)
				{
					case Criteries.OperatorsCover:
						criteries[criteria] = new OperatorsCoverCriteria(controlGraph);
						break;
					case Criteries.SolutionsCover:
						criteries[criteria] = new SolutionCoverCriteria(controlGraph);
						break;
					case Criteries.ConditionsCover:
						throw new NotImplementedException();
					case Criteries.SolutionsAndConditionsCover:
						throw new NotImplementedException();
					default:
						throw new ArgumentException(
							"Метод GetBidirectionGraphForPath получил в качестве параметра несуществующий критерий");
				}
			}
		}

		private IBidirectionalGraph<object, IEdge<object>> GetPathForDrawing(GraphPath graphPath)
		{
			return ControlGraphCompiler.CompileBidirectionalGraph(graphPath);
		}

		public override string ToString()
		{
			return controlGraph.ToString();
		}
	}
}