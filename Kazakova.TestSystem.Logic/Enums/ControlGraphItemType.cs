namespace Kazakova.TestSystem.Logic.Enums
{
	using System.ComponentModel;

	internal enum ControlGraphItemType
	{
		[Description("Обычная строка")] Common,

		[Description("Условие IF")] If,

		[Description("Ветка ELSE условия IF")] Else,

		[Description("Условие Switch")] Switch,

		[Description("Ветка условия Switch")] Case,

		[Description("Default ветка условия Switch")] Default,

		[Description("Цикл FOR")] For,

		[Description("Цикл WHILE")] While,

		[Description("Строка, не содержащая управляющих конструкций")] Useless,

		[Description("Левая фигурная скобка")] LeftBrace,

		[Description("Правая фигурная скобка")] RightBrace
	}
}