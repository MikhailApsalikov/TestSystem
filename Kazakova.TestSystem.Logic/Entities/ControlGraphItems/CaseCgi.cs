namespace Kazakova.TestSystem.Logic.Entities.ControlGraphItems
{
	using System;
	using System.Text.RegularExpressions;
	using Interfaces;

	internal class CaseCgi : ControlGraphItem, IScopeOwner
	{
		private const string ValueRegex = @"case *(\d*) *";
		public CaseCgi(ControlGraph graph, String content, int id)
			: base(graph, content, id)
		{
			ParseValue(ValueRegex);
		}

		public Scope Scope { get; set; }

		public int Value { get; private set; }

		public void InitializeScopes()
		{
			Scope = new Scope(graph, this);
		}

		public void InitializeRanges()
		{
		}

		private void ParseValue(string regex)
		{
			Value = Int32.Parse(Regex.Match(content, regex).Groups[1].Value);
		}
	}
}