namespace Kazakova.TestSystem.Logic.Services.Compiler
{
	using System;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Entities;
	using Microsoft.CSharp;

	internal static class CodeExecutor
	{
		public static string CompileAndExecute(ControlGraph graph, Dictionary<String, Range> ranges)
		{
			var sortedRanges = new SortedDictionary<string, Range>(ranges);
			var parameters = GenerateTemplateForParameters(sortedRanges.Keys.ToList());
			if (graph.AssemblyFileName == null)
			{
				graph.AssemblyFileName = Compile(InjectCodeIntoTemplate(graph.Content, parameters));
			}

			return Execute(graph.AssemblyFileName, sortedRanges.Values.ToList());
		}

		private static string GenerateTemplateForParameters(List<string> variableNames)
		{
			return String.Join(", ", variableNames.Select(vn => String.Format("double {0}", vn)));
		}

		private static string InjectCodeIntoTemplate(String source, String parameters)
		{
			return Template1 + parameters + Template2 + source + Template3;
		}

		private static String Compile(String source)
		{
			var codeProvider = new CSharpCodeProvider();
			var fileName = Guid.NewGuid() + ".dll";
			var results = codeProvider.CompileAssemblyFromSource(new CompilerParameters
			{
				GenerateExecutable = false,
				OutputAssembly = fileName
			}, source);

			if (results.Errors.Count > 0)
			{
				var errorMessage = results.Errors.Cast<CompilerError>()
					.Aggregate(String.Empty,
						(current, compilerError) =>
							current +
							String.Format("Строка {0}. Ошибка #{1}: {2}{3}{3}", compilerError.Line, compilerError.ErrorNumber,
								compilerError.ErrorText, Environment.NewLine));

				throw new SyntaxErrorException(errorMessage);
			}

			return fileName;
		}

		private static string Execute(string assemblyFileName, List<Range> ranges)
		{
			var dll = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, assemblyFileName));
			var parameters = new object[ranges.Count];
			for (var i = 0; i < parameters.Length; i++)
			{
				parameters[i] = ranges[i].OneValue ?? 0;
			}

			return
				dll.GetType("Kazakova.TestSystem.GeneratedCode.GeneratedClass")
					.GetMethod("GeneratedMethod")
					.Invoke(null, parameters)
					.ToString();
		}

		#region Template

		private const string Template1 =
			@"namespace Kazakova.TestSystem.GeneratedCode
{
	using System;
	
	public static class GeneratedClass
	{
		public static object GeneratedMethod(";

		private const string Template2 = @")
		{";
		private const string Template3 = @"		}
	}
}";

		#endregion
	}
}