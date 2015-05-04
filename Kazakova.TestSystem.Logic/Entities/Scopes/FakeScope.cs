namespace Kazakova.TestSystem.Logic.Entities.Scopes
{
	using ControlGraphItems.Interfaces;

	internal sealed class FakeScope : ScopeBase
	{
		public FakeScope(ControlGraph graph, IScopeOwner owner, int id, Range range)
			: base(graph, owner)
		{
			Begin = id;
			End = id;
			HasValuableItems = false;
			HasNestedConditions = false;
			Range = range;
			DetectParentScope();
		}

		public override int Begin { get; protected set; }
		public override int End { get; protected set; }
		public override bool HasValuableItems { get; protected set; }
		public override bool HasNestedConditions { get; protected set; }
	}
}