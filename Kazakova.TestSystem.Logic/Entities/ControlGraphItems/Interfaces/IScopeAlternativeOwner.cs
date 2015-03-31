namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces
{
	internal interface IScopeAlternativeOwner : IScopeOwner
	{
		Scope ScopeAlternative { get; set; }
	}
}