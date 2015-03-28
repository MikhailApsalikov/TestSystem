namespace Kazakova.TestSystem.Logic.Criteria
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Entities;
	using Entities.ControlGraphItems;
	using Entities.ControlGraphItems.Interfaces;

	internal abstract class BaseCriteria
	{
		protected List<GraphPath> cachedPathes;
		protected ControlGraph controlGraph;

		public BaseCriteria(ControlGraph controlGraph)
		{
			this.controlGraph = controlGraph;
		}

		internal List<GraphPath> GetPathes()
		{
			if (cachedPathes == null)
			{
				cachedPathes = GeneratePathes();
			}

			return cachedPathes;
		}

		protected abstract IEnumerable<GraphPath> HandleIf(GraphPath path, IfCgi ifCgi, int endIndex = Int32.MaxValue);

		protected abstract IEnumerable<GraphPath> HandleSwitch(GraphPath path, SwitchCgi switchCgi,
			int endIndex = Int32.MaxValue);

		protected abstract IEnumerable<GraphPath> HandleCycles(GraphPath path, ICycle cycleCgi, int endIndex = Int32.MaxValue);

		protected virtual List<GraphPath> GeneratePathes()
		{
			return GeneratePathes(new GraphPath(controlGraph), 0).ToList();
		}

		protected IEnumerable<GraphPath> GeneratePathes(GraphPath path, int index, int endIndex = Int32.MaxValue)
		{
			if (index >= controlGraph.Count || index >= endIndex)
			{
				return new List<GraphPath> {path};
			}

			if (!(controlGraph[index] is IValuable))
			{
				return GeneratePathes(path, index + 1, endIndex);
			}

			path.Add(controlGraph[index] as IValuable);

			var ifCgi = controlGraph[index] as IfCgi;
			var switchCgi = controlGraph[index] as SwitchCgi;
			var cycleCgi = controlGraph[index] as ICycle;

			if (ifCgi != null)
			{
				return HandleIf(path, ifCgi, endIndex);
			}

			if (switchCgi != null)
			{
				return HandleSwitch(path, switchCgi, endIndex);
			}

			if (cycleCgi != null)
			{
				return HandleCycles(path, cycleCgi, endIndex);
			}

			return GeneratePathes(path, index + 1, endIndex);
		}
	}
}