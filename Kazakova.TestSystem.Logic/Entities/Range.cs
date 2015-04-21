namespace Kazakova.TestSystem.Logic.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ControlGraphItems;
	using Enums;

	internal class Range : HashSet<double>
	{
		private const double MinValue = -100.0;
		private const double MaxValue = 100.0;
		private const double Step = 0.5;
		private const double Tolerance = 0.1;

		public Range(ParsedCondition condition)
		{
			Variable = condition.Variable;
			switch (condition.OperationType)
			{
				case OperationTypes.Equal:
					HandleEqual(condition.Value);
					break;
				case OperationTypes.Less:
					HandleLess(condition.Value);
					break;
				case OperationTypes.LessOrEqual:
					HandleLessOrEqual(condition.Value);
					break;
				case OperationTypes.More:
					HandleMore(condition.Value);
					break;
				case OperationTypes.MoreOrEqual:
					HandleMoreOrEqual(condition.Value);
					break;
				case OperationTypes.NotEqual:
					HandleNotEqual(condition.Value);
					break;
			}
		}

		private Range(HashSet<int> values)
		{
			foreach (var value in values)
			{
				Add(value);
			}
		}

		public string Variable { get; private set; }

		public double? OneValue
		{
			get
			{
				if (this.Any())
				{
					return this.First();
				}
				return null;
			}
		}

		private void HandleEqual(int value)
		{
			Clear();
			Add(value);
		}

		private void HandleLess(int value)
		{
			for (var i = MinValue; i < value; i += Step)
			{
				Add(i);
			}
		}

		private void HandleLessOrEqual(int value)
		{
			for (var i = MinValue; i <= value + Tolerance; i += Step)
			{
				Add(i);
			}
		}

		private void HandleMore(int value)
		{
			for (var i = MaxValue; i > value; i -= Step)
			{
				Add(i);
			}
		}

		private void HandleMoreOrEqual(int value)
		{
			for (var i = MaxValue; i >= value - Tolerance; i -= Step)
			{
				Add(i);
			}
		}

		private void HandleNotEqual(int value)
		{
			for (var i = MinValue; i < MaxValue; i += Step)
			{
				if (Math.Abs(i - value) > Tolerance)
				{
					Add(i);
				}
			}
		}
	}
}