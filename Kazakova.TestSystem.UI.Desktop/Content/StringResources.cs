using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazakova.TestSystem.UI.Desktop.Content
{
	public static class StringResources
	{
		public const String ProgramName = "Test System";

		public const String GreetingsMessage = @"Вас приветствует программа для изучения
способов тестирования логики программного кода. 
Пожалуста, прочтите инструкцию, если вы еще не 
работали с данной программой.";

		public const String InstructionHeader = @"Пройдите несколько простых шагов:";

		public const String FileLoadHeader = "Загрузка программного кода";

		public const String ShowGraph = "Показать управляющий граф программы";

		public const String ControlGraphPreviewName = "Управляющий граф программы";

		public const Int32 MaxFileSize = 60;

		public const String CodeAnalysisHeader = @"Файл успешно обработан. Выберите метод расчета 
тестового пути";

		public const String DoYouReallyWannaQuit = "Вы действительно хотите выйти?";

		public const String Exit = "Выход";

		public const String OperatorsCoverCriteria = "Критерий покрытия операторов";

		public const String SolutionsCoverCriteria = "Критерий покрытия решений";

		public const String ConditionsCoverCriteria = "Критерий покрытия условий";

		public const String SolutionsAndConditionsCoverCriteria = "Критерий покрытия решений/условий";

		public static readonly String instructionText = @"1) Для начала работы загрузите файл с тестируемым кодом в формате .txt, .cs или .java. Код не 
должен содержать синтаксических ошибок и ошибок компиляции. Файл должен содержать быть не 
более " + MaxFileSize.ToString() + @" строк кода.
2) Если файл будет корректно обработан, то система сгенерирует управляющий граф 
тестируемого кода.
3) Далее система предложит продемонстрировать работу критериев тестирования логики 
работы для загруженного кода. Поддерживаются: 
    -Критерий покрытия операторов
    -Критерий покрытия решений
    -Критерий покрытия условий
    -Критерий покрытия решений/условий
";

		public static readonly String fileLoadText = @"Пожалуйста, загрузите файл с программным кодом для тестирования. Файл должен иметь расширение .txt .cs или .java, должен быть компилируемым и содержать не более " + MaxFileSize.ToString() + @" строк кода.";
	}
}
