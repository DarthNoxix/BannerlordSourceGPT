using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace SandBox.View.Map;

public class MapNavigationHandler : INavigationHandler
{
	public class ClanScreenPermissionEvent : EventBase
	{
		public Action<bool, TextObject> IsClanScreenAvailable { get; private set; }

		public ClanScreenPermissionEvent(Action<bool, TextObject> isClanScreenAvailable)
		{
			IsClanScreenAvailable = isClanScreenAvailable;
		}
	}

	private readonly Game _game;

	private NavigationPermissionItem? _mostRecentClanScreenPermission;

	private readonly TextObject _needToBeInKingdomText;

	private readonly ClanScreenPermissionEvent _clanScreenPermissionEvent;

	public bool IsNavigationLocked { get; set; }

	private InquiryData _unsavedChangesInquiry => GetUnsavedChangedInquiry();

	private InquiryData _unapplicableChangesInquiry => GetUnapplicableChangedInquiry();

	public bool PartyEnabled
	{
		get
		{
			if (!IsNavigationBarEnabled())
			{
				return false;
			}
			if (PartyActive)
			{
				return false;
			}
			if (Hero.MainHero.HeroState == Hero.CharacterStates.Prisoner)
			{
				return false;
			}
			if (MobileParty.MainParty.MapEvent != null)
			{
				return false;
			}
			Mission current = Mission.Current;
			if (current != null && !current.IsPartyWindowAccessAllowed)
			{
				return false;
			}
			return true;
		}
	}

	public bool InventoryEnabled
	{
		get
		{
			if (!IsNavigationBarEnabled())
			{
				return false;
			}
			if (InventoryActive)
			{
				return false;
			}
			if (Hero.MainHero.HeroState == Hero.CharacterStates.Prisoner)
			{
				return false;
			}
			Mission current = Mission.Current;
			if (current != null && !current.IsInventoryAccessAllowed)
			{
				return false;
			}
			return true;
		}
	}

	public bool QuestsEnabled
	{
		get
		{
			if (!IsNavigationBarEnabled())
			{
				return false;
			}
			if (QuestsActive)
			{
				return false;
			}
			Mission current = Mission.Current;
			if (current != null && !current.IsQuestScreenAccessAllowed)
			{
				return false;
			}
			return true;
		}
	}

	public bool CharacterDeveloperEnabled
	{
		get
		{
			if (!IsNavigationBarEnabled())
			{
				return false;
			}
			if (CharacterDeveloperActive)
			{
				return false;
			}
			Mission current = Mission.Current;
			if (current != null && !current.IsCharacterWindowAccessAllowed)
			{
				return false;
			}
			return true;
		}
	}

	public NavigationPermissionItem ClanPermission
	{
		get
		{
			if (!IsNavigationBarEnabled())
			{
				return new NavigationPermissionItem(isAuthorized: false, null);
			}
			if (ClanActive)
			{
				return new NavigationPermissionItem(isAuthorized: false, null);
			}
			Mission current = Mission.Current;
			if (current != null && !current.IsClanWindowAccessAllowed)
			{
				return new NavigationPermissionItem(isAuthorized: false, null);
			}
			_mostRecentClanScreenPermission = null;
			Game.Current.EventManager.TriggerEvent(_clanScreenPermissionEvent);
			return _mostRecentClanScreenPermission ?? new NavigationPermissionItem(isAuthorized: true, null);
		}
	}

	public NavigationPermissionItem KingdomPermission
	{
		get
		{
			if (!IsNavigationBarEnabled())
			{
				return new NavigationPermissionItem(isAuthorized: false, null);
			}
			if (KingdomActive)
			{
				return new NavigationPermissionItem(isAuthorized: false, null);
			}
			if (!Hero.MainHero.MapFaction.IsKingdomFaction)
			{
				return new NavigationPermissionItem(isAuthorized: false, _needToBeInKingdomText);
			}
			Mission current = Mission.Current;
			if (current != null && !current.IsKingdomWindowAccessAllowed)
			{
				return new NavigationPermissionItem(isAuthorized: false, null);
			}
			return new NavigationPermissionItem(isAuthorized: true, null);
		}
	}

	public bool EscapeMenuEnabled
	{
		get
		{
			if (!IsNavigationBarEnabled())
			{
				return false;
			}
			if (EscapeMenuActive)
			{
				return false;
			}
			return _game.GameStateManager.ActiveState is MapState;
		}
	}

	public bool PartyActive => _game.GameStateManager.ActiveState is PartyState;

	public bool InventoryActive => _game.GameStateManager.ActiveState is InventoryState;

	public bool QuestsActive => _game.GameStateManager.ActiveState is QuestsState;

	public bool CharacterDeveloperActive => _game.GameStateManager.ActiveState is CharacterDeveloperState;

	public bool ClanActive => _game.GameStateManager.ActiveState is ClanState;

	public bool KingdomActive => _game.GameStateManager.ActiveState is KingdomState;

	public bool EscapeMenuActive
	{
		get
		{
			if (_game.GameStateManager.ActiveState is MapState)
			{
				return MapScreen.Instance?.IsEscapeMenuOpened ?? false;
			}
			return false;
		}
	}

	public MapNavigationHandler()
	{
		_game = Game.Current;
		_needToBeInKingdomText = GameTexts.FindText("str_need_to_be_in_kingdom");
		_clanScreenPermissionEvent = new ClanScreenPermissionEvent(OnClanScreenPermission);
	}

	private InquiryData GetUnsavedChangedInquiry()
	{
		return new InquiryData(string.Empty, GameTexts.FindText("str_unsaved_changes").ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_apply").ToString(), GameTexts.FindText("str_cancel").ToString(), null, null);
	}

	private InquiryData GetUnapplicableChangedInquiry()
	{
		return new InquiryData(string.Empty, GameTexts.FindText("str_unapplicable_changes").ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_apply").ToString(), GameTexts.FindText("str_cancel").ToString(), null, null);
	}

	private bool IsMapTopScreen()
	{
		return ScreenManager.TopScreen is MapScreen;
	}

	private bool IsNavigationBarEnabled()
	{
		if (Hero.MainHero != null)
		{
			Hero mainHero = Hero.MainHero;
			if (mainHero == null || !mainHero.IsDead)
			{
				Campaign current = Campaign.Current;
				if (current == null || !current.SaveHandler.IsSaving)
				{
					if (IsNavigationLocked)
					{
						return false;
					}
					if (InventoryManager.InventoryLogic != null && InventoryManager.Instance.CurrentMode != 0)
					{
						return false;
					}
					if (PartyScreenManager.PartyScreenLogic != null && PartyScreenManager.Instance.CurrentMode != 0)
					{
						return false;
					}
					if (PlayerEncounter.CurrentBattleSimulation != null)
					{
						return false;
					}
					if (ScreenManager.TopScreen is MapScreen mapScreen && (mapScreen.IsInArmyManagement || mapScreen.IsMarriageOfferPopupActive || mapScreen.IsMapCheatsActive))
					{
						return false;
					}
					if (EscapeMenuActive)
					{
						return false;
					}
					return true;
				}
			}
		}
		return false;
	}

	public void OnClanScreenPermission(bool isAvailable, TextObject reasonString)
	{
		if (!isAvailable)
		{
			_mostRecentClanScreenPermission = new NavigationPermissionItem(isAvailable, reasonString);
		}
	}

	void INavigationHandler.OpenQuests()
	{
		PrepareToOpenQuestsScreen(delegate
		{
			OpenQuestsAction();
		});
	}

	void INavigationHandler.OpenQuests(IssueBase issue)
	{
		PrepareToOpenQuestsScreen(delegate
		{
			OpenQuestsAction(issue);
		});
	}

	void INavigationHandler.OpenQuests(QuestBase quest)
	{
		PrepareToOpenQuestsScreen(delegate
		{
			OpenQuestsAction(quest);
		});
	}

	void INavigationHandler.OpenQuests(JournalLogEntry log)
	{
		PrepareToOpenQuestsScreen(delegate
		{
			OpenQuestsAction(log);
		});
	}

	private void PrepareToOpenQuestsScreen(Action openQuestsAction)
	{
		if (!QuestsEnabled)
		{
			return;
		}
		if (ScreenManager.TopScreen is IChangeableScreen changeableScreen && changeableScreen.AnyUnsavedChanges())
		{
			InquiryData obj = (changeableScreen.CanChangesBeApplied() ? _unsavedChangesInquiry : _unapplicableChangesInquiry);
			obj.SetAffirmativeAction(delegate
			{
				ApplyCurrentChanges();
				openQuestsAction();
			});
			InformationManager.ShowInquiry(obj);
		}
		else
		{
			if (!IsMapTopScreen())
			{
				_game.GameStateManager.PopState();
			}
			openQuestsAction();
		}
	}

	private void OpenQuestsAction()
	{
		QuestsState gameState = _game.GameStateManager.CreateState<QuestsState>();
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenQuestsAction(IssueBase issue)
	{
		QuestsState gameState = _game.GameStateManager.CreateState<QuestsState>(new object[1] { issue });
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenQuestsAction(QuestBase quest)
	{
		QuestsState gameState = _game.GameStateManager.CreateState<QuestsState>(new object[1] { quest });
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenQuestsAction(JournalLogEntry log)
	{
		QuestsState gameState = _game.GameStateManager.CreateState<QuestsState>(new object[1] { log });
		_game.GameStateManager.PushState(gameState);
	}

	void INavigationHandler.OpenInventory()
	{
		if (!InventoryEnabled)
		{
			return;
		}
		if (ScreenManager.TopScreen is IChangeableScreen changeableScreen)
		{
			if (changeableScreen.AnyUnsavedChanges())
			{
				InquiryData obj = (changeableScreen.CanChangesBeApplied() ? _unsavedChangesInquiry : _unapplicableChangesInquiry);
				obj.SetAffirmativeAction(delegate
				{
					ApplyCurrentChanges();
					OpenInventoryScreenAction();
				});
				InformationManager.ShowInquiry(obj);
			}
			else
			{
				OpenInventoryScreenAction();
			}
		}
		else
		{
			OpenInventoryScreenAction();
		}
	}

	private void OpenInventoryScreenAction()
	{
		if (!IsMapTopScreen())
		{
			_game.GameStateManager.PopState();
		}
		InventoryManager.OpenScreenAsInventory();
	}

	void INavigationHandler.OpenParty()
	{
		if (!PartyEnabled)
		{
			return;
		}
		if (ScreenManager.TopScreen is IChangeableScreen changeableScreen)
		{
			if (changeableScreen.AnyUnsavedChanges())
			{
				InquiryData obj = (changeableScreen.CanChangesBeApplied() ? _unsavedChangesInquiry : _unapplicableChangesInquiry);
				obj.SetAffirmativeAction(delegate
				{
					ApplyCurrentChanges();
					OpenPartyScreenAction();
				});
				InformationManager.ShowInquiry(obj);
			}
			else
			{
				OpenPartyScreenAction();
			}
		}
		else
		{
			OpenPartyScreenAction();
		}
	}

	private void OpenPartyScreenAction()
	{
		if (PartyEnabled)
		{
			if (!IsMapTopScreen())
			{
				_game.GameStateManager.PopState();
			}
			PartyScreenManager.OpenScreenAsNormal();
		}
	}

	void INavigationHandler.OpenCharacterDeveloper()
	{
		PrepareToOpenCharacterDeveloper(delegate
		{
			OpenCharacterDeveloperScreenAction();
		});
	}

	void INavigationHandler.OpenCharacterDeveloper(Hero hero)
	{
		PrepareToOpenCharacterDeveloper(delegate
		{
			OpenCharacterDeveloperScreenAction(hero);
		});
	}

	private void PrepareToOpenCharacterDeveloper(Action openCharacterDeveloperAction)
	{
		if (!CharacterDeveloperEnabled)
		{
			return;
		}
		if (ScreenManager.TopScreen is IChangeableScreen changeableScreen && changeableScreen.AnyUnsavedChanges())
		{
			InquiryData obj = (changeableScreen.CanChangesBeApplied() ? _unsavedChangesInquiry : _unapplicableChangesInquiry);
			obj.SetAffirmativeAction(delegate
			{
				ApplyCurrentChanges();
				openCharacterDeveloperAction();
			});
			InformationManager.ShowInquiry(obj);
		}
		else
		{
			if (!IsMapTopScreen())
			{
				_game.GameStateManager.PopState();
			}
			openCharacterDeveloperAction();
		}
	}

	private void OpenCharacterDeveloperScreenAction()
	{
		CharacterDeveloperState gameState = _game.GameStateManager.CreateState<CharacterDeveloperState>();
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenCharacterDeveloperScreenAction(Hero hero)
	{
		CharacterDeveloperState gameState = _game.GameStateManager.CreateState<CharacterDeveloperState>(new object[1] { hero });
		_game.GameStateManager.PushState(gameState);
	}

	void INavigationHandler.OpenKingdom()
	{
		PrepareToOpenKingdomScreen(delegate
		{
			OpenKingdomAction();
		});
	}

	void INavigationHandler.OpenKingdom(Army army)
	{
		PrepareToOpenKingdomScreen(delegate
		{
			OpenKingdomAction(army);
		});
	}

	void INavigationHandler.OpenKingdom(Settlement settlement)
	{
		PrepareToOpenKingdomScreen(delegate
		{
			OpenKingdomAction(settlement);
		});
	}

	void INavigationHandler.OpenKingdom(Clan clan)
	{
		PrepareToOpenKingdomScreen(delegate
		{
			OpenKingdomAction(clan);
		});
	}

	void INavigationHandler.OpenKingdom(PolicyObject policy)
	{
		PrepareToOpenKingdomScreen(delegate
		{
			OpenKingdomAction(policy);
		});
	}

	void INavigationHandler.OpenKingdom(IFaction faction)
	{
		PrepareToOpenKingdomScreen(delegate
		{
			OpenKingdomAction(faction);
		});
	}

	void INavigationHandler.OpenKingdom(KingdomDecision decision)
	{
		PrepareToOpenKingdomScreen(delegate
		{
			OpenKingdomAction(decision);
		});
	}

	private void PrepareToOpenKingdomScreen(Action openKingdomAction)
	{
		if (!KingdomPermission.IsAuthorized)
		{
			return;
		}
		if (ScreenManager.TopScreen is IChangeableScreen changeableScreen && changeableScreen.AnyUnsavedChanges())
		{
			InquiryData obj = (changeableScreen.CanChangesBeApplied() ? _unsavedChangesInquiry : _unapplicableChangesInquiry);
			obj.SetAffirmativeAction(delegate
			{
				ApplyCurrentChanges();
				openKingdomAction();
			});
			InformationManager.ShowInquiry(obj);
		}
		else
		{
			if (!IsMapTopScreen())
			{
				_game.GameStateManager.PopState();
			}
			openKingdomAction();
		}
	}

	private void OpenKingdomAction()
	{
		KingdomState gameState = _game.GameStateManager.CreateState<KingdomState>();
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenKingdomAction(Army army)
	{
		KingdomState gameState = _game.GameStateManager.CreateState<KingdomState>(new object[1] { army });
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenKingdomAction(Settlement settlement)
	{
		KingdomState gameState = _game.GameStateManager.CreateState<KingdomState>(new object[1] { settlement });
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenKingdomAction(Clan clan)
	{
		KingdomState gameState = _game.GameStateManager.CreateState<KingdomState>(new object[1] { clan });
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenKingdomAction(PolicyObject policy)
	{
		KingdomState gameState = _game.GameStateManager.CreateState<KingdomState>(new object[1] { policy });
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenKingdomAction(IFaction faction)
	{
		KingdomState gameState = _game.GameStateManager.CreateState<KingdomState>(new object[1] { faction });
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenKingdomAction(KingdomDecision decision)
	{
		KingdomState gameState = _game.GameStateManager.CreateState<KingdomState>(new object[1] { decision });
		_game.GameStateManager.PushState(gameState);
	}

	void INavigationHandler.OpenClan()
	{
		PrepareToOpenClanScreen(delegate
		{
			OpenClanScreenAction();
		});
	}

	void INavigationHandler.OpenClan(Hero hero)
	{
		PrepareToOpenClanScreen(delegate
		{
			OpenClanScreenAction(hero);
		});
	}

	void INavigationHandler.OpenClan(PartyBase party)
	{
		PrepareToOpenClanScreen(delegate
		{
			OpenClanScreenAction(party);
		});
	}

	void INavigationHandler.OpenClan(Settlement settlement)
	{
		PrepareToOpenClanScreen(delegate
		{
			OpenClanScreenAction(settlement);
		});
	}

	void INavigationHandler.OpenClan(Workshop workshop)
	{
		PrepareToOpenClanScreen(delegate
		{
			OpenClanScreenAction(workshop);
		});
	}

	void INavigationHandler.OpenClan(Alley alley)
	{
		PrepareToOpenClanScreen(delegate
		{
			OpenClanScreenAction(alley);
		});
	}

	private void PrepareToOpenClanScreen(Action openClanScreenAction)
	{
		if (!ClanPermission.IsAuthorized)
		{
			return;
		}
		if (ScreenManager.TopScreen is IChangeableScreen changeableScreen && changeableScreen.AnyUnsavedChanges())
		{
			InquiryData obj = (changeableScreen.CanChangesBeApplied() ? _unsavedChangesInquiry : _unapplicableChangesInquiry);
			obj.SetAffirmativeAction(delegate
			{
				ApplyCurrentChanges();
				openClanScreenAction();
			});
			InformationManager.ShowInquiry(obj);
		}
		else
		{
			if (!IsMapTopScreen())
			{
				_game.GameStateManager.PopState();
			}
			openClanScreenAction();
		}
	}

	private void OpenClanScreenAction()
	{
		ClanState gameState = _game.GameStateManager.CreateState<ClanState>();
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenClanScreenAction(Hero hero)
	{
		ClanState gameState = _game.GameStateManager.CreateState<ClanState>(new object[1] { hero });
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenClanScreenAction(PartyBase party)
	{
		ClanState gameState = _game.GameStateManager.CreateState<ClanState>(new object[1] { party });
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenClanScreenAction(Settlement settlement)
	{
		ClanState gameState = _game.GameStateManager.CreateState<ClanState>(new object[1] { settlement });
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenClanScreenAction(Workshop workshop)
	{
		ClanState gameState = _game.GameStateManager.CreateState<ClanState>(new object[1] { workshop });
		_game.GameStateManager.PushState(gameState);
	}

	private void OpenClanScreenAction(Alley alley)
	{
		ClanState gameState = _game.GameStateManager.CreateState<ClanState>(new object[1] { alley });
		_game.GameStateManager.PushState(gameState);
	}

	void INavigationHandler.OpenEscapeMenu()
	{
		if (EscapeMenuEnabled)
		{
			MapScreen.Instance?.OpenEscapeMenu();
		}
	}

	private void ApplyCurrentChanges()
	{
		if (ScreenManager.TopScreen is IChangeableScreen changeableScreen && changeableScreen.AnyUnsavedChanges())
		{
			if (changeableScreen.CanChangesBeApplied())
			{
				changeableScreen.ApplyChanges();
			}
			else
			{
				changeableScreen.ResetChanges();
			}
		}
		if (!IsMapTopScreen())
		{
			_game.GameStateManager.PopState();
		}
	}
}
