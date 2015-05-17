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
	using Entities.ControlGraphItems;
	using Microsoft.CSharp;

	internal static class CodeExecutor
	{
		public static string CompileAndExecute(ControlGraph graph, Dictionary<string, Range> ranges, List<string> variables)
		{
			var parameters = GenerateTemplateForParameters(graph, variables, ranges);
			if (graph.AssemblyFileName == null)
			{
				graph.AssemblyFileName = Compile(InjectCodeIntoTemplate(graph.Content, parameters));
			}

			return Execute(graph.AssemblyFileName, ranges, variables);
		}

		private static string GenerateTemplateForParameters(ControlGraph graph, List<string> variableNames,
			Dictionary<string, Range> ranges)
		{
			return String.Join(", ",
				variableNames.Select(vn => String.Format("{1} {0}", vn, ranges.ContainsKey(vn) && CanBeDouble(
					graph, ranges[vn])
					? "double"
					: "int")));
		}

		private static bool CanBeDouble(ControlGraph graph, Range range) // switch fix
		{
			return graph.OfType<SwitchCgi>().All(sw => sw.Variable != range.Variable);
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

		private static string Execute(string assemblyFileName, Dictionary<string, Range> ranges, List<string> variables)
		{
			var dll = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, assemblyFileName));
			var parameters = new object[variables.Count];
			for (var i = 0; i < parameters.Length; i++)
			{
				if (ranges.ContainsKey(variables[i]))
				{
					parameters[i] = ranges[variables[i]].OneIntValue;
					if (parameters[i] == null)
					{
						parameters[i] = ranges[variables[i]].OneValue;

						if (parameters[i] == null)
						{
							parameters[i] = 0;
						}
					}
				}
				else
				{
					parameters[i] = 0;
				}
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