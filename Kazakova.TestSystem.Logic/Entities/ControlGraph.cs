namespace Kazakova.TestSystem.Logic.Entities
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using Kazakova.TestSystem.Logic.Services;

	internal class ControlGraph : IEnumerable<ControlGraphItem>, IEnumerator<ControlGraphItem>
	{
		private readonly List<ControlGraphItem> items;

		public ControlGraph(String content)
		{
			items = Parser.Parse(this, content, 60);
			Count = items.Count;
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

		public IEnumerator<ControlGraphItem> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}

		public ControlGraphItem Current
		{
			get { return items.GetEnumerator().Current; }
		}

		public void Dispose()
		{
			items.GetEnumerator().Dispose();
		}

		object IEnumerator.Current
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

		public override string ToString()
		{
			return String.Join(Environment.NewLine, from item in items
				where !(item is BeginEndCgi)
				select String.Format("{0} {1}", item is IValuable ? ((IValuable) item).ShownId : "   ", item));
		}

		public int GetMaxValuableBranches(bool checkEmpty)
		{
			var conditions = items.OfType<ICondition>();
			if (!conditions.Any())
			{
				return 1;
			}
			return conditions.Max(item => item.ValuableBranches);
		}
	}
}