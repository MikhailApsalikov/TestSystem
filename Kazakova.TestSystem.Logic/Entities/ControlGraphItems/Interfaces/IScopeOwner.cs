namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces
{
	internal interface IScopeOwner
	{
		Scope Scope { get; set; }
		void InitializeScopes();
		void InitializeRanges();
	}
}