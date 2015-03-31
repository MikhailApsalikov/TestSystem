namespace System
{
	using System.ComponentModel;

	public static class EnumExtensions
	{
		public static string GetString(this Enum e)
		{
			var s = e.ToString();
			var fi = e.GetType().GetField(s);
			var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(
				typeof (DescriptionAttribute), false);
			if (attributes.Length > 0)
			{
				s = attributes[0].Description;
			}

			return s;
		}
	}
}