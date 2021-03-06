﻿namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces
{
	using Scopes;

	internal interface IScopeOwner
	{
		Scope Scope { get; set; }
		void InitializeScopes();
		void InitializeRanges();
	}
}