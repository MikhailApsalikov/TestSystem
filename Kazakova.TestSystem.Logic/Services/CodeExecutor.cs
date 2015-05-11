namespace Kazakova.TestSystem.Logic.Services.Compiler
{
	using System;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using Entities;
	using Microsoft.CSharp;

	internal static class CodeExecutor
	{
		public static string CompileAndExecute(ControlGraph graph, List<String> variableNames)
		{
			var parameters = GenerateTemplateForParameters(variableNames);
			String assemblyFileName = Compile(InjectCodeIntoTemplate(graph.Content, parameters));
			return "123";
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
			var fileName = Guid.NewGuid() + "out.dll";

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