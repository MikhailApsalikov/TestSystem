namespace Kazakova.TestSystem.Logic.Fake
{
	using System;
	using System.Linq;
	using Enums;
	using QuickGraph;

	public class FakeTester : ITester
	{
		private const string FakeString = "<TEST_EXAMPLE>";
		private string fileContent;

		public FakeTester(string fileContent)
		{
			this.fileContent = fileContent;
		}

		public BidirectionalGraph<object, IEdge<object>> GetBidirectionGraphForWholeGraph()
		{
			var result = new BidirectionalGraph<object, IEdge<object>>();
			result.AddVertex("Начало");
			for (var i = 1; i <= 04; i++)
			{
				result.AddVertex(i.ToString("D2"));
			}
			result.AddVertex("Конец");
			
			result.AddEdge(new Edge<object>("Начало", "01"));
			result.AddEdge(new Edge<object>("01", "02"));
			result.AddEdge(new Edge<object>("02", "03"));
			result.AddEdge(new Edge<object>("02", "04"));
			result.AddEdge(new Edge<object>("03", "04"));
			result.AddEdge(new Edge<object>("04", "Конец"));
			return result;
		}

		public int GetPathCountForCriteria(Criteries criteria)
		{
			switch (criteria)
			{
				case Criteries.OperatorsCover:
					return 1;
				default:
					return 2;
			}
		}

		public IBidirectionalGraph<object, IEdge<object>> GetBidirectionGraphForPath(Criteries criteria, int pathId)
		{
			switch (criteria)
			{
				case Criteries.OperatorsCover:
				{
					switch (pathId)
					{
						case 0:
							return GetGraphByPathVertexes(1, 2, 3, 4);
						default:
							throw new ArgumentException("Данного пути не существует");
					}
				}
				case Criteries.SolutionsCover:
				{
					switch (pathId)
					{
						case 0:
							return GetGraphByPathVertexes(1, 2, 3, 4);
						case 1:
							return GetGraphByPathVertexes(1, 2, 4);
						default:
							throw new ArgumentException("Данного пути не существует");
					}
				}
				case Criteries.ConditionsCover:
				{
					switch (pathId)
					{
						case 0:
							return GetGraphByPathVertexes(1, 2, 4);
						case 1:
							return GetGraphByPathVertexes(1, 2, 4);
						default:
							throw new ArgumentException("Данного пути не существует");
					}
				}
				case Criteries.SolutionsAndConditionsCover:
				{
					switch (pathId)
					{
						case 0:
							return GetGraphByPathVertexes(1, 2, 3, 4);
						case 1:
							return GetGraphByPathVertexes(1, 2, 4);
						default:
							throw new ArgumentException("Данного пути не существует");
					}
				}
				default:
				{
					throw new ArgumentException("Неизвестный критерий");
				}
			}
		}

		public string GetRequiredParametersForPathAsString(Criteries criteria, int pathId)
		{
			switch (criteria)
			{
				case Criteries.OperatorsCover:
					{
						switch (pathId)
						{
							case 0:
								return "a = 0; b = 100";
							default:
								throw new ArgumentException("Данного пути не существует");
						}
					}
				case Criteries.SolutionsCover:
					{
						switch (pathId)
						{
							case 0:
								return "a = 0; b = 100";
							case 1:
								return "a = 1; b = 100";
							default:
								throw new ArgumentException("Данного пути не существует");
						}
					}
				case Criteries.ConditionsCover:
					{
						switch (pathId)
						{
							case 0:
								return "a = 0; b = 0";
							case 1:
								return "a = 1; b = 100";
							default:
								throw new ArgumentException("Данного пути не существует");
						}
					}
				case Criteries.SolutionsAndConditionsCover:
					{
						switch (pathId)
						{
							case 0:
								return "a = 0; b = 100";
							case 1:
								return "a = 1; b = -100";
							default:
								throw new ArgumentException("Данного пути не существует");
						}
					}
				default:
					{
						throw new ArgumentException("Неизвестный критерий");
					}
			}
		}

		public string ExecuteCodeAndGetResultAsString(Criteries criteria, int pathId) // TODO
		{
			switch (criteria)
			{
				case Criteries.OperatorsCover:
					{
						switch (pathId)
						{
							case 0:
								return "200";
							default:
								throw new ArgumentException("Данного пути не существует");
						}
					}
				case Criteries.SolutionsCover:
					{
						switch (pathId)
						{
							case 0:
								return "200";
							case 1:
								return "100";
							default:
								throw new ArgumentException("Данного пути не существует");
						}
					}
				case Criteries.ConditionsCover:
					{
						switch (pathId)
						{
							case 0:
								return "0";
							case 1:
								return "100";
							default:
								throw new ArgumentException("Данного пути не существует");
						}
					}
				case Criteries.SolutionsAndConditionsCover:
					{
						switch (pathId)
						{
							case 0:
								return "200";
							case 1:
								return "-100";
							default:
								throw new ArgumentException("Данного пути не существует");
						}
					}
				default:
					{
						throw new ArgumentException("Неизвестный критерий");
					}
			}
		}

		public override string ToString()
		{
			return @"01 int a, b; //<TEST_EXAMPLE>
02 if (a == 0 & b>10)
   {
03     b=b*2;
   }
04 return b;";
		}

		private IBidirectionalGraph<object, IEdge<object>> GetGraphByPathVertexes(params int[] args)
		{
			if (args.Length < 1)
			{
				throw new ArgumentException("Пустой путь невозможно отобразить");
			}

			var argsAsString = args.Select(arg => arg.ToString("D2")).ToList();
			var result = new BidirectionalGraph<object, IEdge<object>>();
			result.AddVertex("Начало");
			result.AddVertexRange(argsAsString);
			result.AddVertex("Конец");

			result.AddEdge(new Edge<object>("Начало", argsAsString[0]));
			for (var i = 0; i < argsAsString.Count - 1; i++)
			{
				result.AddEdge(new Edge<object>(argsAsString[i], argsAsString[i + 1]));
			}
			result.AddEdge(new Edge<object>(argsAsString[argsAsString.Count - 1], "Конец"));
			return result;
		}

		

		public static bool IsFake(string fileContent)
		{
			return fileContent.Contains(FakeString);
		}
	}
}