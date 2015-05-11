namespace Kazakova.TestSystem.Logic
{
	using System;
	using System.Collections.Generic;
	using Criteria;
	using Entities;
	using Enums;
	using QuickGraph;
	using Services;

	public class Tester
	{
		internal Dictionary<Criteries, BaseCriteria> criteries = new Dictionary<Criteries, BaseCriteria>();
		private readonly ControlGraph controlGraph;

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

		public string GetRequiredParametersForPathAsString(Criteries criteria, int pathId)
		{
			CheckPathCache(criteria);
			return criteries[criteria].GetPathes()[pathId].GetRequiredParametersAsString();
		}

		public string ExecuteCodeAndGetResultAsString(Criteries criteria, int pathId)
		{
			CheckPathCache(criteria);
			return criteries[criteria].GetPathes()[pathId].ExecuteCodeAndGetResultAsString(criteries[criteria].GetVariablesList());
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
						criteries[criteria] = new SolutionCoverCriteria(controlGraph);
						break;
					case Criteries.SolutionsAndConditionsCover:
						criteries[criteria] = new SolutionCoverCriteria(controlGraph);
						break;
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