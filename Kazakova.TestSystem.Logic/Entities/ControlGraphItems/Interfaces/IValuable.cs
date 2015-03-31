namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces
{
	using System;

	internal interface IValuable
	{
		String ShownId { get; }
		void SetShownId(int shownId);
	}
}