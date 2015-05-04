namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces
{
	using Scopes;

	internal interface IScopeAlternativeOwner : IScopeOwner
	{
		Scope ScopeAlternative { get; set; }
	}
}