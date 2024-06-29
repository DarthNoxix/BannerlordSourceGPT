using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class AssignRolesTutorial : TutorialItemBase
{
	private bool _playerAssignedRoleToClanMember;

	public AssignRolesTutorial()
	{
		base.Type = "AssignRolesTutorial";
		base.Placement = TutorialItemVM.ItemPlacements.Top;
		base.HighlightedVisualElementID = "RoleAssignmentWidget";
		base.MouseRequired = false;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.ClanScreen;
	}

	public override void OnClanRoleAssignedThroughClanScreen(ClanRoleAssignedThroughClanScreenEvent obj)
	{
		_playerAssignedRoleToClanMember = true;
	}

	public override bool IsConditionsMetForActivation()
	{
		return TutorialHelper.PlayerHasUnassignedRolesAndMember;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _playerAssignedRoleToClanMember;
	}
}
