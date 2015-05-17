namespace Kazakova.TestSystem.Logic.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ControlGraphItems;
	using Enums;

	internal class Range : HashSet<double>, ICloneable
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

		public Range(String variable, IEnumerable<double> values)
		{
			Variable = variable;
			foreach (var value in values)
			{
				Add(value);
			}
		}

		private Range(String variable)
		{
			Variable = variable;
		}

		public string Variable { get; private set; }

		public int? OneIntValue
		{
			get
			{
				var hasIntValue = this.Any(v => Math.Abs(v - (int) v) < Tolerance);
				if (hasIntValue)
				{
					return (int) this.First(v => Math.Abs(v - (int) v) < Tolerance);
				}

				return null;
			}
		}

		public double? OneValue
		{
			get
			{
				var intValue = OneIntValue;
				if (intValue != null)
				{
					return intValue;
				}

				if (this.Any())
				{
					return this.First();
				}
				return null;
			}
		}

		public object Clone()
		{
			return new Range(Variable, this);
		}

		public static Range CreateFullRange(String variable)
		{
			var result = new Range(variable);
			for (var i = MinValue; i < MaxValue + Tolerance; i += Step)
			{
				result.Add(i);
			}

			return result;
		}

		public new void UnionWith(IEnumerable<double> other)
		{
			foreach (var value in other)
			{
				if (!this.Any(v => Math.Abs(v - value) < Tolerance))
				{
					Add(value);
				}
			}
		}

		public Range Intersect(IEnumerable<double> other)
		{
			return new Range(Variable, new HashSet<double>(other.Where(v => this.Any(z => Math.Abs(z - v) < Tolerance))));
		}

		public Range Except(IEnumerable<double> other)
		{
			return new Range(Variable, new HashSet<double>(this.Where(v => !other.Any(z => Math.Abs(z - v) < Tolerance))));
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