namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces
{
	internal interface ICondition : IScopeOwner
	{
		int ValuableBranches { get; }
		bool HasEmptyWay { get; }
		string ParsedCondition { get; }
	}
}