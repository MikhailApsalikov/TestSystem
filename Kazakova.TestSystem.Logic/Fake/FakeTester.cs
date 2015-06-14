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
			for (var i = 1; i <= 19; i++)
			{
				result.AddVertex(i.ToString("D2"));
			}
			result.AddVertex("Конец");
			result.AddEdge(new Edge<object>("Начало", "01"));
			result.AddEdge(new Edge<object>("01", "02"));
			result.AddEdge(new Edge<object>("02", "03"));
			result.AddEdge(new Edge<object>("02", "06"));
			result.AddEdge(new Edge<object>("02", "12"));
			result.AddEdge(new Edge<object>("02", "16"));
			result.AddEdge(new Edge<object>("06", "07"));
			result.AddEdge(new Edge<object>("07", "08"));
			result.AddEdge(new Edge<object>("07", "10"));
			result.AddEdge(new Edge<object>("08", "09"));
			result.AddEdge(new Edge<object>("09", "19"));
			result.AddEdge(new Edge<object>("10", "11"));
			result.AddEdge(new Edge<object>("11", "19"));

			result.AddEdge(new Edge<object>("12", "15"));
			result.AddEdge(new Edge<object>("12", "13"));
			result.AddEdge(new Edge<object>("13", "14"));
			result.AddEdge(new Edge<object>("14", "12"));
			result.AddEdge(new Edge<object>("14", "15"));
			result.AddEdge(new Edge<object>("15", "19"));

			result.AddEdge(new Edge<object>("03", "04"));
			result.AddEdge(new Edge<object>("04", "03"));
			result.AddEdge(new Edge<object>("04", "05"));
			result.AddEdge(new Edge<object>("03", "05"));
			result.AddEdge(new Edge<object>("05", "19"));

			result.AddEdge(new Edge<object>("16", "17"));
			result.AddEdge(new Edge<object>("17", "18"));
			result.AddEdge(new Edge<object>("16", "18"));
			result.AddEdge(new Edge<object>("18", "19"));

			result.AddEdge(new Edge<object>("19", "Конец"));
			return result;
		}

		public int GetPathCountForCriteria(Criteries criteria)
		{
			switch (criteria)
			{
				case Criteries.OperatorsCover:
					return 5;
				default:
					return 6;
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
							return GetGraphByPathVertexes(1, 2, 3, 4, 5, 19);
						case 1:
							return GetGraphByPathVertexes(1, 2, 6, 7, 8, 9, 19);
						case 2:
							return GetGraphByPathVertexes(1, 2, 6, 7, 10, 11, 19);
						case 3:
							return GetGraphByPathVertexes(1, 2, 12, 13, 14, 15, 19);
						case 4:
							return GetGraphByPathVertexes(1, 2, 16, 17, 18, 19);
						default:
							throw new ArgumentException("Данного пути не существует");
					}
				}
				case Criteries.SolutionsCover:
				{
					switch (pathId)
					{
						case 0:
							return GetGraphByPathVertexes(1, 2, 3, 4, 5, 19);
						case 1:
							return GetGraphByPathVertexes(1, 2, 6, 7, 8, 9, 19);
						case 2:
							return GetGraphByPathVertexes(1, 2, 6, 7, 10, 11, 19);
						case 3:
							return GetGraphByPathVertexes(1, 2, 12, 13, 14, 15, 19);
						case 4:
							return GetGraphByPathVertexes(1, 2, 16, 17, 18, 19);
						case 5:
							return GetGraphByPathVertexes(1, 2, 16, 18, 19);
						default:
							throw new ArgumentException("Данного пути не существует");
					}
				}
				case Criteries.ConditionsCover:
				case Criteries.SolutionsAndConditionsCover:
				{
					switch (pathId)
					{
						case 0:
							return GetGraphByPathVertexes(1, 2, 3, 4, 5, 19);
						case 1:
							return GetGraphByPathVertexes(1, 2, 6, 7, 8, 9, 19);
						case 2:
							return GetGraphByPathVertexes(1, 2, 6, 7, 10, 11, 19);
						case 3:
							return GetGraphByPathVertexes(1, 2, 12, 13, 14, 15, 19);
						case 4:
							return GetGraphByPathVertexes(1, 2, 16, 17, 18, 19);
						case 5:
							return GetGraphByPathVertexes(1, 2, 16, 18, 19);
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
								return "level = 1";
							case 1:
								return "factor = 6; level = 2";
							case 2:
								return "factor = -100; level = 2";
							case 3:
								return "level = 3";
							case 4:
								return "level = 100; factor = 0; k = 11";
							default:
								throw new ArgumentException("Данного пути не существует");
						}
					}
				case Criteries.SolutionsCover:
					{
						switch (pathId)
						{
							case 0:
								return "level = 1";
							case 1:
								return "factor = 6; level = 2";
							case 2:
								return "factor = -100; level = 2";
							case 3:
								return "level = 3";
							case 4:
								return "level = 100; factor = 0; k = -100";
							case 5:
								return "level = 100; factor = 2; k = -100";
							default:
								throw new ArgumentException("Данного пути не существует");
						}
					}
				case Criteries.ConditionsCover:
				case Criteries.SolutionsAndConditionsCover:
					{
						switch (pathId)
						{
							case 0:
								return "level = 1";
							case 1:
								return "factor = 6; level = 2";
							case 2:
								return "factor = -100; level = 2";
							case 3:
								return "level = 3";
							case 4:
								return "level = 100; factor = 0; k = 11";
							case 5:
								return "level = 100; factor = 2; k = 0";
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

		public string ExecuteCodeAndGetResultAsString(Criteries criteria, int pathId)
		{
			switch (criteria)
			{
				case Criteries.OperatorsCover:
					{
						switch (pathId)
						{
							case 0:
								return "16";
							case 1:
								return "8";
							case 2:
								return "1";
							case 3:
								return "0";
							case 4:
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
								return "16";
							case 1:
								return "8";
							case 2:
								return "1";
							case 3:
								return "0";
							case 4:
								return "200";
							case 5:
								return "50";
							default:
								throw new ArgumentException("Данного пути не существует");
						}
					}
				case Criteries.ConditionsCover:
				case Criteries.SolutionsAndConditionsCover:
					{
						switch (pathId)
						{
							case 0:
								return "16";
							case 1:
								return "8";
							case 2:
								return "1";
							case 3:
								return "0";
							case 4:
								return "200";
							case 5:
								return "50";
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
			return @"01 int j=2; //<TEST_EXAMPLE>
02 switch (level)
   {
       case 1:
       {
03         for (int i = 1; i <= 5; i++)
           {
04             level= level+ i;
           }
05         return level;
       }
      case 2:
      {
06        Console.WriteLine(""Enter factor\"");
07        if (factor==6)
          {
08            level=level*4;
09            return level;
          }
          else
          {
10            level=level/2;
11            return level;
          }         
      }
      case 3:
      {
12        while (j > 0)
          {
13            level= level-j;
14            j--;
          }
15        return level*3;
        }
      default :
      {
16        if (factor==0 || k > 10)
          {
17            level=level*4;
          }          
18            return level/2;
      }
   }
19 Console.ReadLine();";
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