namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces
{
	using System;
	using System.Linq;

	internal interface IScopeAlternativeOwner : IScopeOwner
	{
		Scope ScopeAlternative { get; set; }
	}
}