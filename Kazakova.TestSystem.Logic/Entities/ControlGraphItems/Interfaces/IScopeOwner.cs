namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces
{
	using System;
	using System.Linq;

	internal interface IScopeOwner
	{
		Scope Scope { get; set; }

		void InitializeScopes();
	}
}