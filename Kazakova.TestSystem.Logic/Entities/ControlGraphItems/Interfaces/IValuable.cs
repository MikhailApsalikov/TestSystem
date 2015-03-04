namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces
{
	using System;
	using System.Linq;

	internal interface IValuable
	{
		String ShownId { get; }

		void SetShownId(int shownId);
	}
}