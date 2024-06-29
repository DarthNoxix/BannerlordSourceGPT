using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox;

public static class SandBoxHelpers
{
	public static class MissionHelper
	{
		public static void FadeOutAgents(IEnumerable<Agent> agents, bool hideInstantly, bool hideMount)
		{
			if (agents == null)
			{
				return;
			}
			Agent[] array = agents.ToArray();
			Agent[] array2 = array;
			foreach (Agent agent in array2)
			{
				if (!agent.IsMount)
				{
					agent.FadeOut(hideInstantly, hideMount);
				}
			}
			array2 = array;
			foreach (Agent agent2 in array2)
			{
				if (agent2.State != AgentState.Routed)
				{
					agent2.FadeOut(hideInstantly, hideMount);
				}
			}
		}
	}
}
