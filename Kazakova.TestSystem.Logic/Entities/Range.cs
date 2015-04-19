namespace Kazakova.TestSystem.Logic.Entities
{
	using System;

	internal class Range
	{
		public double Left { get; set; }
		public double Right { get; set; }

		public Range Intersect(Range another)
		{
			var range = new Range
			{
				Left = Math.Max(Left, another.Left),
				Right = Math.Min(Right, another.Right)
			};

			if (range.Left >= range.Right)
			{
				return null;
			}

			return range;
		}
	}
}