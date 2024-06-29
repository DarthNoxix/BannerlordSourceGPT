using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace StoryMode.View;

public class StoryModeViewSubModule : MBSubModuleBase
{
	protected override void OnSubModuleLoad()
	{
		base.OnSubModuleLoad();
		TextObject coreContentDisabledReason = new TextObject("{=V8BXjyYq}Disabled during installation.");
		Module.CurrentModule.AddInitialStateOption(new InitialStateOption("StoryModeNewGame", new TextObject("{=sf_menu_storymode_new_game}New Campaign"), 2, delegate
		{
			MBGameManager.StartNewGame((MBGameManager)(object)new StoryModeGameManager());
		}, () => (Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason)));
		Module.CurrentModule.ImguiProfilerTick += OnImguiProfilerTick;
	}

	protected virtual void FillDataForCampaign()
	{
	}

	protected override void OnSubModuleUnloaded()
	{
		Module.CurrentModule.ImguiProfilerTick -= OnImguiProfilerTick;
		base.OnSubModuleUnloaded();
	}

	private void OnImguiProfilerTick()
	{
		if (Campaign.Current == null)
		{
			return;
		}
		MBReadOnlyList<MobileParty> all = MobileParty.All;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (MobileParty item in all)
		{
			if (item.IsVisible)
			{
				num++;
			}
			PartyVisual visualOfParty = PartyVisualManager.Current.GetVisualOfParty(item.Party);
			if (visualOfParty.HumanAgentVisuals != null)
			{
				num2++;
			}
			if (visualOfParty.MountAgentVisuals != null)
			{
				num2++;
			}
			if (visualOfParty.CaravanMountAgentVisuals != null)
			{
				num2++;
			}
			num3++;
		}
		Imgui.BeginMainThreadScope();
		Imgui.Begin("Bannerlord Campaign Statistics");
		Imgui.Columns(2);
		Imgui.Text("Name");
		Imgui.NextColumn();
		Imgui.Text("Count");
		Imgui.NextColumn();
		Imgui.Separator();
		Imgui.Text("Total Mobile Party");
		Imgui.NextColumn();
		Imgui.Text(num3.ToString());
		Imgui.NextColumn();
		Imgui.Text("Visible Mobile Party");
		Imgui.NextColumn();
		Imgui.Text(num.ToString());
		Imgui.NextColumn();
		Imgui.Text("Total Agent Visuals");
		Imgui.NextColumn();
		Imgui.Text(num2.ToString());
		Imgui.NextColumn();
		Imgui.End();
		Imgui.EndMainThreadScope();
	}
}
