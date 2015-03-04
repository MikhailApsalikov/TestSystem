namespace Kazakova.TestSystem.Logic.Entities
{
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using Kazakova.TestSystem.Logic.Services;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class ControlGraph : IEnumerable<ControlGraphItem>, IEnumerator<ControlGraphItem>
	{
		private List<ControlGraphItem> items;

		public ControlGraph(String content)
		{
			this.items = Parser.Parse(this, content, 60);
			this.Count = items.Count;
			foreach (var item in items.OfType<IScopeOwner>())
			{
				item.InitializeScopes();
			}

			foreach (var item in items.OfType<SwitchCgi>())
			{
				item.InitializeCases();
			}
		}

		public ControlGraphItem this[int index]
		{
			get { return items[index]; }
		}

		public int Count { get; internal set; }

		public override string ToString()
		{
			return String.Join(Environment.NewLine, from item in items
													where !(item is BeginEndCgi)
													select String.Format("{0} {1}", item is IValuable ? ((IValuable)item).ShownId : "   ", item));
		}

		public int GetMaxValuableBranches(bool checkEmpty)
		{
			var conditions = items.OfType<ICondition>();
			if (!conditions.Any())
			{
				return 1;
			}
			return (int)conditions.Max(item => item.ValuableBranches);
		}

		public IEnumerator<ControlGraphItem> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}

		public ControlGraphItem Current
		{
			get
			{
				return items.GetEnumerator().Current;
			}
		}

		public void Dispose()
		{
			items.GetEnumerator().Dispose();
		}

		object System.Collections.IEnumerator.Current
		{
			get { return items.GetEnumerator().Current; }
		}

		public bool MoveNext()
		{
			return items.GetEnumerator().MoveNext();
		}

		public void Reset()
		{
			items.GetEnumerator().Dispose();
		}
	}
}