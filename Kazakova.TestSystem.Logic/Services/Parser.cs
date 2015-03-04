namespace Kazakova.TestSystem.Logic.Services
{
	using Kazakova.TestSystem.Logic.Entities;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal static class Parser
	{
		public static List<ControlGraphItem> Parse(ControlGraph graph, String source, int maxFileSize = 100)
		{
			var result = new List<ControlGraphItem>();
			string[] splittedContent = source.Split('\r');
			var begin = new BeginEndCgi(graph, "", 0);
			begin.SetShownId("Начало");
			result.Add(begin);
			for (int i = 0; i < splittedContent.Length; i++)
			{
				splittedContent[i] = splittedContent[i].Replace("\n", "").Trim();
				result.Add(ParseItem(graph, splittedContent[i], i + 1));
			}
			var end = new BeginEndCgi(graph, "", splittedContent.Length + 1);
			end.SetShownId("Конец");
			result.Add(end);
			if (result.Count > maxFileSize + 2)
			{
				throw new ArgumentOutOfRangeException(String.Format("Файл содержит больше {0} строк и не может быть обработан", maxFileSize));
			}

			SetShownIds(result);
			return result;
		}

		private static ControlGraphItem ParseItem(ControlGraph graph, string element, int index)
		{
			if (String.IsNullOrWhiteSpace(element))
			{
				return new UselessCgi(graph, element, index);
			}

			if (element == "{")
			{
				return new LeftBraceCgi(graph, element, index);
			}

			if (element == "}")
			{
				return new RightBraceCgi(graph, element, index);
			}

			if (element.Contains("if"))
			{
				return new IfCgi(graph, element, index);
			}

			if (element.Contains("else"))
			{
				return new ElseCgi(graph, element, index);
			}

			if (element.Contains("switch"))
			{
				return new SwitchCgi(graph, element, index);
			}

			if (element.Contains("for"))
			{
				return new ForCgi(graph, element, index);
			}

			if (element.Contains("while"))
			{
				return new WhileCgi(graph, element, index);
			}

			if (element.Contains("case"))
			{
				return new CaseCgi(graph, element, index);
			}

			if (element.Contains("default"))
			{
				return new DefaultCgi(graph, element, index);
			}

			return new CommonCgi(graph, element, index);
		}

		private static void SetShownIds(List<ControlGraphItem> graph)
		{
			int iterator = 1;
			foreach (var item in graph.OfType<IValuable>())
			{
				if (!(item is BeginEndCgi))
				{
					item.SetShownId(iterator);
					iterator++;
				}
				
			}
		}
	}
}