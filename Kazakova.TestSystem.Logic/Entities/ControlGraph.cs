namespace Kazakova.TestSystem.Logic.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ControlGraphItems;
	using ControlGraphItems.Interfaces;
	using Services;

	internal class ControlGraph : List<ControlGraphItem>
	{
		public String Content { get; private set; }

		public String AssemblyFileName { get; set; }

		public ControlGraph(String content)
		{
			Content = content;
			AddRange(Parser.Parse(this, content, 60));
			foreach (var item in this.OfType<IScopeOwner>())
			{
				item.InitializeScopes();
			}

			foreach (var item in this.OfType<SwitchCgi>())
			{
				item.InitializeCases();
			}

			foreach (var item in this.OfType<IScopeOwner>())
			{
				item.InitializeRanges();
			}
		}

		public override string ToString()
		{
			return String.Join(Environment.NewLine, from item in this
				where !(item is BeginEndCgi)
				select String.Format("{0} {1}", item is IValuable ? ((IValuable) item).ShownId : "   ", item));
		}

		public int GetMaxValuableBranches(bool checkEmpty)
		{
			var conditions = this.OfType<Condition>().ToList();
			if (!conditions.Any())
			{
				return 1;
			}
			return conditions.Max(item => item.ValuableBranches);
		}
	}
}