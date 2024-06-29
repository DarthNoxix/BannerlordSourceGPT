using SandBox.View.Map;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace StoryMode.GauntletUI.Permissions;

public class StoryModePermissionsSystem
{
	private static StoryModePermissionsSystem Current;

	private StoryModePermissionsSystem()
	{
		RegisterEvents();
	}

	public static void OnInitialize()
	{
		if (Current == null)
		{
			Current = new StoryModePermissionsSystem();
		}
	}

	internal static void OnUnload()
	{
		if (Current != null)
		{
			Current.UnregisterEvents();
			Current = null;
		}
	}

	private void OnClanScreenPermission(ClanScreenPermissionEvent obj)
	{
		StoryModeManager current = StoryModeManager.Current;
		if (current != null && current.MainStoryLine.IsPlayerInteractionRestricted)
		{
			obj.IsClanScreenAvailable(arg1: false, new TextObject("{=75nwCTEn}Clan Screen is disabled during Tutorial."));
		}
	}

	private void OnSettlementOverlayTalkPermission(SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent obj)
	{
		bool num = StoryModeManager.Current != null;
		TutorialPhase instance = TutorialPhase.Instance;
		bool flag = instance != null && instance.TutorialQuestPhase >= TutorialQuestPhase.RecruitAndPurchaseStarted;
		StoryModeManager current = StoryModeManager.Current;
		bool flag2 = current != null && current.MainStoryLine?.TutorialPhase.IsCompleted == true;
		if (num && !flag && !flag2)
		{
			obj.IsTalkAvailable(arg1: false, new TextObject("{=UjERCi2F}This feature is disabled."));
		}
	}

	private void OnSettlementOverlayQuickTalkPermission(SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent obj)
	{
		bool num = StoryModeManager.Current != null;
		TutorialPhase instance = TutorialPhase.Instance;
		bool flag = instance != null && instance.TutorialQuestPhase >= TutorialQuestPhase.Finalized;
		StoryModeManager current = StoryModeManager.Current;
		bool flag2 = current != null && current.MainStoryLine?.TutorialPhase.IsCompleted == true;
		if (num && !flag && !flag2)
		{
			obj.IsTalkAvailable(arg1: false, new TextObject("{=UjERCi2F}This feature is disabled."));
		}
	}

	private void OnSettlementOverlayLeaveMemberPermission(SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent obj)
	{
		bool num = StoryModeManager.Current != null;
		TutorialPhase instance = TutorialPhase.Instance;
		bool flag = instance != null && instance.TutorialQuestPhase >= TutorialQuestPhase.RecruitAndPurchaseStarted;
		StoryModeManager current = StoryModeManager.Current;
		bool flag2 = current != null && current.MainStoryLine?.TutorialPhase.IsCompleted == true;
		if (num && !flag && !flag2)
		{
			obj.IsLeaveAvailable(arg1: false, new TextObject("{=UjERCi2F}This feature is disabled."));
		}
	}

	private void OnLeaveKingdomPermissionEvent(LeaveKingdomPermissionEvent obj)
	{
		if (StoryModeManager.Current?.MainStoryLine.PlayerSupportedKingdom != null && Clan.PlayerClan.Kingdom == StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom)
		{
			obj.IsLeaveKingdomPossbile?.Invoke(arg1: true, new TextObject("{=WFNLizqL}You've supported a kingdom through main story line. Leaving this kingdom will fail your quest.{newline}{newline}Are you sure?"));
		}
	}

	private void RegisterEvents()
	{
		Game.Current.EventManager.RegisterEvent<ClanScreenPermissionEvent>(OnClanScreenPermission);
		Game.Current.EventManager.RegisterEvent<SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent>(OnSettlementOverlayTalkPermission);
		Game.Current.EventManager.RegisterEvent<SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent>(OnSettlementOverlayQuickTalkPermission);
		Game.Current.EventManager.RegisterEvent<SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent>(OnSettlementOverlayLeaveMemberPermission);
		Game.Current.EventManager.RegisterEvent<LeaveKingdomPermissionEvent>(OnLeaveKingdomPermissionEvent);
	}

	internal void UnregisterEvents()
	{
		Game.Current.EventManager.UnregisterEvent<ClanScreenPermissionEvent>(OnClanScreenPermission);
		Game.Current.EventManager.UnregisterEvent<SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent>(OnSettlementOverlayTalkPermission);
		Game.Current.EventManager.UnregisterEvent<SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent>(OnSettlementOverlayQuickTalkPermission);
		Game.Current.EventManager.UnregisterEvent<SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent>(OnSettlementOverlayLeaveMemberPermission);
		Game.Current.EventManager.UnregisterEvent<LeaveKingdomPermissionEvent>(OnLeaveKingdomPermissionEvent);
	}
}
