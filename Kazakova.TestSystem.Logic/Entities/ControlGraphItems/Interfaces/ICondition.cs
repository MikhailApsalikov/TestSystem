namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces
{
	using System;
	using System.Linq;

	internal interface ICondition:IScopeOwner
	{
		int ValuableBranches { get; }

		bool HasEmptyWay { get; }
	}
}