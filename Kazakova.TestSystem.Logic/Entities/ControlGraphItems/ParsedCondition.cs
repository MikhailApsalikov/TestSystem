namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using System.Text.RegularExpressions;
	using Enums;

	internal class ParsedCondition
	{
		public ParsedCondition(string condition)
		{
			try
			{
				var parsed = Regex.Match(condition, @"^(.*) *(<|>|!=|==|<=|>=) *(-?\d*)$");
				Variable = parsed.Groups[1].Value;
				OperationType = ParseOperationType(parsed.Groups[2].Value);
				Value = Int32.Parse(parsed.Groups[3].Value);
			}
			catch (Exception)
			{
				throw new ArgumentException("Выражение в условии не поддерживается");
			}
		}

		public ParsedCondition(string variable, OperationTypes operationType, int value)
		{
			Variable = variable;
			OperationType = operationType;
			Value = value;
		}

		public string Variable { get; private set; }
		public OperationTypes OperationType { get; private set; }
		public int Value { get; private set; }

		private OperationTypes ParseOperationType(string operationString)
		{
			switch (operationString)
			{
				case "==":
					return OperationTypes.Equal;
				case "!=":
					return OperationTypes.NotEqual;
				case "<=":
					return OperationTypes.LessOrEqual;
				case ">=":
					return OperationTypes.MoreOrEqual;
				case "<":
					return OperationTypes.Less;
				case ">":
					return OperationTypes.More;
				default:
					throw new ArgumentException("ParseOperationType неверный параметр");
			}
		}

		public ParsedCondition Revert()
		{
			switch (OperationType)
			{
				case OperationTypes.Equal:
					return new ParsedCondition(Variable, OperationTypes.NotEqual, Value);
				case OperationTypes.Less:
					return new ParsedCondition(Variable, OperationTypes.MoreOrEqual, Value);
				case OperationTypes.LessOrEqual:
					return new ParsedCondition(Variable, OperationTypes.More, Value);
				case OperationTypes.More:
					return new ParsedCondition(Variable, OperationTypes.LessOrEqual, Value);
				case OperationTypes.MoreOrEqual:
					return new ParsedCondition(Variable, OperationTypes.Less, Value);
				case OperationTypes.NotEqual:
					return new ParsedCondition(Variable, OperationTypes.Equal, Value);
				default:
					throw new ArgumentException("Неверный параметр OperationType");
			}
		}
	}
}