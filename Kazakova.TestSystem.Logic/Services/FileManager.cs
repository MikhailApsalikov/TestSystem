namespace Kazakova.TestSystem.Logic.Services
{
	using System;
	using System.IO;

	public static class FileManager
	{
		public static String Load(String path)
		{
			return File.ReadAllText(path);
		}
	}
}