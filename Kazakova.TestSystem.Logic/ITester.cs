namespace Kazakova.TestSystem.Logic
{
	using Enums;
	using QuickGraph;

	public interface ITester
	{
		BidirectionalGraph<object, IEdge<object>> GetBidirectionGraphForWholeGraph();
		int GetPathCountForCriteria(Criteries criteria);
		IBidirectionalGraph<object, IEdge<object>> GetBidirectionGraphForPath(Criteries criteria, int pathId);
		string GetRequiredParametersForPathAsString(Criteries criteria, int pathId);
		string ExecuteCodeAndGetResultAsString(Criteries criteria, int pathId);
		string ToString();
	}
}