﻿namespace Kazakova.TestSystem.Logic.Criteria
{
	using Kazakova.TestSystem.Logic.Entities;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems;
	using Kazakova.TestSystem.Logic.Entities.ControlGraphItems.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Linq;

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

		protected abstract IEnumerable<GraphPath> HandleIf(GraphPath path, IfCgi ifCgi);

		protected abstract IEnumerable<GraphPath> HandleSwitch(GraphPath path, SwitchCgi switchCgi);

		protected abstract IEnumerable<GraphPath> HandleCycles(GraphPath path, ICycle cycleCgi);


		protected List<GraphPath> GeneratePathes()
		{
			return GeneratePathes(new GraphPath(controlGraph), 0).ToList();
		}

		protected IEnumerable<GraphPath> GeneratePathes(GraphPath path, int index, int endIndex = Int32.MaxValue)
		{
			if (index >= controlGraph.Count || index >= endIndex)
			{
				return new List<GraphPath>() { path };
			}

			if (!(controlGraph[index] is IValuable))
			{
				return GeneratePathes(path, index + 1);
			}

			path.Add(controlGraph[index] as IValuable);

			IfCgi ifCgi = controlGraph[index] as IfCgi;
			SwitchCgi switchCgi = controlGraph[index] as SwitchCgi;
			ICycle cycleCgi = controlGraph[index] as ICycle;

			if (ifCgi != null)
			{
				return HandleIf(path, ifCgi);
			}

			if (switchCgi != null)
			{
				return HandleSwitch(path, switchCgi);
			}

			if (cycleCgi != null)
			{
				return HandleCycles(path, cycleCgi);
			}

			return GeneratePathes(path, index + 1);
		}
	}
}