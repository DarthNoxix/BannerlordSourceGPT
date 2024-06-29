using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Helpers;
using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.View.Map;

public class PartyVisual
{
	private struct SiegeBombardmentData
	{
		public Vec3 LaunchGlobalPosition;

		public Vec3 TargetPosition;

		public MatrixFrame ShooterGlobalFrame;

		public MatrixFrame TargetAlignedShooterGlobalFrame;

		public float MissileSpeed;

		public float Gravity;

		public float LaunchAngle;

		public float RotationDuration;

		public float ReloadDuration;

		public float AimingDuration;

		public float MissileLaunchDuration;

		public float FireDuration;

		public float FlightDuration;

		public float TotalDuration;
	}

	private const string MapSiegeEngineTag = "map_siege_engine";

	private const string MapBreachableWallTag = "map_breachable_wall";

	private const string MapDefenderEngineTag = "map_defensive_engine";

	private const string CircleTag = "map_settlement_circle";

	private const string BannerPlaceHolderTag = "map_banner_placeholder";

	private const string MapCampArea1Tag = "map_camp_area_1";

	private const string MapCampArea2Tag = "map_camp_area_2";

	private const string MapSiegeEngineRamTag = "map_siege_ram";

	private const string TownPhysicalTag = "bo_town";

	private const string MapSiegeEngineTowerTag = "map_siege_tower";

	private const string MapPreparationTag = "siege_preparation";

	private const string BurnedTag = "looted";

	private const float PartyScale = 0.3f;

	private const float HorseAnimationSpeedFactor = 1.3f;

	private static readonly ActionIndexCache _raidOnFoot = ActionIndexCache.Create("act_map_raid");

	private static readonly ActionIndexCache _camelSwordAttack = ActionIndexCache.Create("act_map_rider_camel_attack_1h");

	private static readonly ActionIndexCache _camelSpearAttack = ActionIndexCache.Create("act_map_rider_camel_attack_1h_spear");

	private static readonly ActionIndexCache _camel1HandedSwingAttack = ActionIndexCache.Create("act_map_rider_camel_attack_1h_swing");

	private static readonly ActionIndexCache _camel2HandedSwingAttack = ActionIndexCache.Create("act_map_rider_camel_attack_2h_swing");

	private static readonly ActionIndexCache _camelUnarmedAttack = ActionIndexCache.Create("act_map_rider_camel_attack_unarmed");

	private static readonly ActionIndexCache _horseSwordAttack = ActionIndexCache.Create("act_map_rider_horse_attack_1h");

	private static readonly ActionIndexCache _horseSpearAttack = ActionIndexCache.Create("act_map_rider_horse_attack_1h_spear");

	private static readonly ActionIndexCache _horse1HandedSwingAttack = ActionIndexCache.Create("act_map_rider_horse_attack_1h_swing");

	private static readonly ActionIndexCache _horse2HandedSwingAttack = ActionIndexCache.Create("act_map_rider_horse_attack_2h_swing");

	private static readonly ActionIndexCache _horseUnarmedAttack = ActionIndexCache.Create("act_map_rider_horse_attack_unarmed");

	private static readonly ActionIndexCache _swordAttackMount = ActionIndexCache.Create("act_map_mount_attack_1h");

	private static readonly ActionIndexCache _spearAttackMount = ActionIndexCache.Create("act_map_mount_attack_spear");

	private static readonly ActionIndexCache _swingAttackMount = ActionIndexCache.Create("act_map_mount_attack_swing");

	private static readonly ActionIndexCache _unarmedAttackMount = ActionIndexCache.Create("act_map_mount_attack_unarmed");

	private static readonly ActionIndexCache _attack1H = ActionIndexCache.Create("act_map_attack_1h");

	private static readonly ActionIndexCache _attack2H = ActionIndexCache.Create("act_map_attack_2h");

	private static readonly ActionIndexCache _attackSpear1HOr2H = ActionIndexCache.Create("act_map_attack_spear_1h_or_2h");

	private static readonly ActionIndexCache _attackUnarmed = ActionIndexCache.Create("act_map_attack_unarmed");

	private readonly List<(GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity)> _siegeRangedMachineEntities;

	private readonly List<(GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity)> _siegeMeleeMachineEntities;

	private readonly List<(GameEntity, BattleSideEnum, int)> _siegeMissileEntities;

	private Dictionary<int, List<GameEntity>> _gateBannerEntitiesWithLevels;

	private GameEntity[] _attackerRangedEngineSpawnEntities;

	private GameEntity[] _attackerBatteringRamSpawnEntities;

	private GameEntity[] _defenderBreachableWallEntitiesCacheForCurrentLevel;

	private GameEntity[] _attackerSiegeTowerSpawnEntities;

	private GameEntity[] _defenderRangedEngineSpawnEntitiesForAllLevels;

	private GameEntity[] _defenderRangedEngineSpawnEntitiesCacheForCurrentLevel;

	private GameEntity[] _defenderBreachableWallEntitiesForAllLevels;

	private (string, GameEntityComponent) _cachedBannerComponent;

	private (string, GameEntity) _cachedBannerEntity;

	private MatrixFrame _hoveredSiegeEntityFrame = MatrixFrame.Identity;

	private GameEntity.UpgradeLevelMask _currentSettlementUpgradeLevelMask;

	private float _speed;

	private float _entityAlpha;

	private Scene _mapScene;

	private Mesh _contourMaskMesh;

	private uint _currentLevelMask;

	public readonly PartyBase PartyBase;

	private Vec2 _lastFrameVisualPositionWithoutError;

	public MapScreen MapScreen => MapScreen.Instance;

	public GameEntity StrategicEntity { get; private set; }

	public List<GameEntity> TownPhysicalEntities { get; private set; }

	public MatrixFrame CircleLocalFrame { get; private set; }

	public Vec2 Position => PartyBase.Position2D;

	public bool TargetVisibility => PartyBase.IsVisible;

	private Scene MapScene
	{
		get
		{
			if (_mapScene == null && Campaign.Current != null && Campaign.Current.MapSceneWrapper != null)
			{
				_mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
			}
			return _mapScene;
		}
	}

	public AgentVisuals HumanAgentVisuals { get; private set; }

	public AgentVisuals MountAgentVisuals { get; private set; }

	public AgentVisuals CaravanMountAgentVisuals { get; private set; }

	public bool IsEnemy { get; private set; }

	public bool IsFriendly { get; private set; }

	public IMapEntity GetMapEntity()
	{
		return PartyBase.MapEntity;
	}

	public bool IsEntityMovingVisually()
	{
		if (!PartyBase.IsMobile)
		{
			return false;
		}
		if (!PartyBase.MobileParty.VisualPosition2DWithoutError.NearlyEquals(_lastFrameVisualPositionWithoutError))
		{
			if (Campaign.Current.TimeControlMode != 0)
			{
				_lastFrameVisualPositionWithoutError = PartyBase.MobileParty.VisualPosition2DWithoutError;
			}
			return true;
		}
		return false;
	}

	public PartyVisual(PartyBase partyBase)
	{
		PartyBase = partyBase;
		_siegeRangedMachineEntities = new List<(GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity)>();
		_siegeMeleeMachineEntities = new List<(GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity)>();
		_siegeMissileEntities = new List<(GameEntity, BattleSideEnum, int)>();
		CircleLocalFrame = MatrixFrame.Identity;
	}

	private void AddMountToPartyIcon(Vec3 positionOffset, string mountItemId, string harnessItemId, uint contourColor, CharacterObject character)
	{
		ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(mountItemId);
		Monster monster = @object.HorseComponent.Monster;
		ItemObject item = null;
		if (!string.IsNullOrEmpty(harnessItemId))
		{
			item = Game.Current.ObjectManager.GetObject<ItemObject>(harnessItemId);
		}
		Equipment equipment = new Equipment();
		equipment[EquipmentIndex.ArmorItemEndSlot] = new EquipmentElement(@object);
		equipment[EquipmentIndex.HorseHarness] = new EquipmentElement(item);
		AgentVisualsData agentVisualsData = new AgentVisualsData().Equipment(equipment).Scale(@object.ScaleFactor * 0.3f).Frame(new MatrixFrame(Mat3.Identity, positionOffset))
			.ActionSet(MBGlobals.GetActionSet(monster.ActionSetCode + "_map"))
			.Scene(MapScene)
			.Monster(monster)
			.PrepareImmediately(prepareImmediately: false)
			.UseScaledWeapons(useScaledWeapons: true)
			.HasClippingPlane(hasClippingPlane: true)
			.MountCreationKey(MountCreationKey.GetRandomMountKeyString(@object, character.GetMountKeySeed()));
		CaravanMountAgentVisuals = AgentVisuals.Create(agentVisualsData, "PartyIcon " + mountItemId, false, false, false);
		CaravanMountAgentVisuals.GetEntity().SetContourColor(contourColor, alwaysVisible: false);
		MatrixFrame frame = CaravanMountAgentVisuals.GetFrame();
		frame.rotation.ApplyScaleLocal(CaravanMountAgentVisuals.GetScale());
		frame = StrategicEntity.GetFrame().TransformToParent(frame);
		CaravanMountAgentVisuals.GetEntity().SetFrame(ref frame);
		float num = TaleWorlds.Library.MathF.Min(0.325f * _speed / 0.3f, 20f);
		CaravanMountAgentVisuals.Tick((AgentVisuals)null, 0.0001f, IsEntityMovingVisually(), num);
		CaravanMountAgentVisuals.GetEntity().Skeleton.ForceUpdateBoneFrames();
	}

	private void AddCharacterToPartyIcon(CharacterObject characterObject, uint contourColor, string bannerKey, int wieldedItemIndex, uint teamColor1, uint teamColor2, ActionIndexCache leaderAction, ActionIndexCache mountAction, float animationStartDuration, ref bool clearBannerEntityCache)
	{
		Equipment equipment = characterObject.Equipment.Clone();
		bool flag = !string.IsNullOrEmpty(bannerKey) && (((characterObject.IsPlayerCharacter || characterObject.HeroObject.Clan == Clan.PlayerClan) && Clan.PlayerClan.Tier >= Campaign.Current.Models.ClanTierModel.BannerEligibleTier) || (!characterObject.IsPlayerCharacter && (!characterObject.IsHero || (characterObject.IsHero && characterObject.HeroObject.Clan != Clan.PlayerClan))));
		int leftWieldedItemIndex = 4;
		if (flag)
		{
			ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>("campaign_banner_small");
			equipment[EquipmentIndex.ExtraWeaponSlot] = new EquipmentElement(@object);
		}
		Monster baseMonsterFromRace = TaleWorlds.Core.FaceGen.GetBaseMonsterFromRace(characterObject.Race);
		MBActionSet actionSetWithSuffix = MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, characterObject.IsFemale, flag ? "_map_with_banner" : "_map");
		AgentVisualsData agentVisualsData = new AgentVisualsData().UseMorphAnims(useMorphAnims: true).Equipment(equipment).BodyProperties(characterObject.GetBodyProperties(characterObject.Equipment))
			.SkeletonType(characterObject.IsFemale ? SkeletonType.Female : SkeletonType.Male)
			.Scale(0.3f)
			.Frame(StrategicEntity.GetFrame())
			.ActionSet(actionSetWithSuffix)
			.Scene(MapScene)
			.Monster(baseMonsterFromRace)
			.PrepareImmediately(prepareImmediately: false)
			.RightWieldedItemIndex(wieldedItemIndex)
			.HasClippingPlane(hasClippingPlane: true)
			.UseScaledWeapons(useScaledWeapons: true)
			.ClothColor1(teamColor1)
			.ClothColor2(teamColor2)
			.CharacterObjectStringId(characterObject.StringId)
			.AddColorRandomness(!characterObject.IsHero)
			.Race(characterObject.Race);
		if (flag)
		{
			Banner banner = new Banner(bannerKey);
			agentVisualsData.Banner(banner).LeftWieldedItemIndex(leftWieldedItemIndex);
			if (_cachedBannerEntity.Item1 == bannerKey + "campaign_banner_small")
			{
				agentVisualsData.CachedWeaponEntity(EquipmentIndex.ExtraWeaponSlot, _cachedBannerEntity.Item2);
			}
		}
		HumanAgentVisuals = AgentVisuals.Create(agentVisualsData, "PartyIcon " + characterObject.Name, false, false, false);
		if (flag)
		{
			GameEntity entity = HumanAgentVisuals.GetEntity();
			GameEntity child = entity.GetChild(entity.ChildCount - 1);
			if (child.GetComponentCount(GameEntity.ComponentType.ClothSimulator) > 0)
			{
				clearBannerEntityCache = false;
				_cachedBannerEntity = (bannerKey + "campaign_banner_small", child);
			}
		}
		if (leaderAction != ActionIndexCache.act_none)
		{
			float actionAnimationDuration = MBActionSet.GetActionAnimationDuration(actionSetWithSuffix, leaderAction);
			if (actionAnimationDuration < 1f)
			{
				HumanAgentVisuals.GetVisuals().GetSkeleton().SetAgentActionChannel(0, leaderAction, animationStartDuration);
			}
			else
			{
				HumanAgentVisuals.GetVisuals().GetSkeleton().SetAgentActionChannel(0, leaderAction, animationStartDuration / actionAnimationDuration);
			}
		}
		if (characterObject.HasMount())
		{
			Monster monster = characterObject.Equipment[EquipmentIndex.ArmorItemEndSlot].Item.HorseComponent.Monster;
			MBActionSet actionSet = MBGlobals.GetActionSet(monster.ActionSetCode + "_map");
			AgentVisualsData agentVisualsData2 = new AgentVisualsData().Equipment(characterObject.Equipment).Scale(characterObject.Equipment[EquipmentIndex.ArmorItemEndSlot].Item.ScaleFactor * 0.3f).Frame(MatrixFrame.Identity)
				.ActionSet(actionSet)
				.Scene(MapScene)
				.Monster(monster)
				.PrepareImmediately(prepareImmediately: false)
				.UseScaledWeapons(useScaledWeapons: true)
				.HasClippingPlane(hasClippingPlane: true)
				.MountCreationKey(MountCreationKey.GetRandomMountKeyString(characterObject.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, characterObject.GetMountKeySeed()));
			MountAgentVisuals = AgentVisuals.Create(agentVisualsData2, string.Concat("PartyIcon ", characterObject.Name, " mount"), false, false, false);
			if (mountAction != ActionIndexCache.act_none)
			{
				float actionAnimationDuration2 = MBActionSet.GetActionAnimationDuration(actionSet, mountAction);
				if (actionAnimationDuration2 < 1f)
				{
					MountAgentVisuals.GetEntity().Skeleton.SetAgentActionChannel(0, mountAction, animationStartDuration);
				}
				else
				{
					MountAgentVisuals.GetEntity().Skeleton.SetAgentActionChannel(0, mountAction, animationStartDuration / actionAnimationDuration2);
				}
			}
			MountAgentVisuals.GetEntity().SetContourColor(contourColor, alwaysVisible: false);
			MatrixFrame frame = StrategicEntity.GetFrame();
			frame.rotation.ApplyScaleLocal(agentVisualsData2.ScaleData);
			MountAgentVisuals.GetEntity().SetFrame(ref frame);
		}
		HumanAgentVisuals.GetEntity().SetContourColor(contourColor, alwaysVisible: false);
		MatrixFrame frame2 = StrategicEntity.GetFrame();
		frame2.rotation.ApplyScaleLocal(agentVisualsData.ScaleData);
		HumanAgentVisuals.GetEntity().SetFrame(ref frame2);
		float num = ((MountAgentVisuals != null) ? 1.3f : 1f);
		float num2 = TaleWorlds.Library.MathF.Min(0.25f * num * _speed / 0.3f, 20f);
		if (MountAgentVisuals != null)
		{
			MountAgentVisuals.Tick((AgentVisuals)null, 0.0001f, IsEntityMovingVisually(), num2);
			MountAgentVisuals.GetEntity().Skeleton.ForceUpdateBoneFrames();
		}
		HumanAgentVisuals.Tick(MountAgentVisuals, 0.0001f, IsEntityMovingVisually(), num2);
		HumanAgentVisuals.GetEntity().Skeleton.ForceUpdateBoneFrames();
	}

	private static MetaMesh GetBannerOfCharacter(Banner banner, string bannerMeshName)
	{
		MetaMesh copy = MetaMesh.GetCopy(bannerMeshName);
		for (int i = 0; i < copy.MeshCount; i++)
		{
			Mesh meshAtIndex = copy.GetMeshAtIndex(i);
			if (meshAtIndex.HasTag("dont_use_tableau"))
			{
				continue;
			}
			Material material = meshAtIndex.GetMaterial();
			Material tableauMaterial = null;
			Tuple<Material, BannerCode> key = new Tuple<Material, BannerCode>(material, BannerCode.CreateFrom(banner));
			if (MapScreen.Instance._characterBannerMaterialCache.ContainsKey(key))
			{
				tableauMaterial = MapScreen.Instance._characterBannerMaterialCache[key];
			}
			else
			{
				tableauMaterial = material.CreateCopy();
				Action<Texture> action = delegate(Texture tex)
				{
					tableauMaterial.SetTexture(Material.MBTextureType.DiffuseMap2, tex);
					uint num = (uint)tableauMaterial.GetShader().GetMaterialShaderFlagMask("use_tableau_blending");
					ulong shaderFlags = tableauMaterial.GetShaderFlags();
					tableauMaterial.SetShaderFlags(shaderFlags | num);
				};
				BannerVisualExtensions.GetTableauTextureLarge(banner, action);
				MapScreen.Instance._characterBannerMaterialCache[key] = tableauMaterial;
			}
			meshAtIndex.SetMaterial(tableauMaterial);
		}
		return copy;
	}

	public void Tick(float dt, ref int dirtyPartiesCount, ref PartyVisual[] dirtyPartiesList)
	{
		if (PartyBase.IsSettlement)
		{
			TickSettlementVisual(dt, ref dirtyPartiesCount, ref dirtyPartiesList);
		}
		else
		{
			TickMobilePartyVisual(dt, ref dirtyPartiesCount, ref dirtyPartiesList);
		}
		if (PartyBase.LevelMaskIsDirty)
		{
			RefreshLevelMask();
		}
	}

	private void TickSettlementVisual(float dt, ref int dirtyPartiesCount, ref PartyVisual[] dirtyPartiesList)
	{
		if (StrategicEntity == null)
		{
			return;
		}
		if (PartyBase.IsVisualDirty)
		{
			int num = Interlocked.Increment(ref dirtyPartiesCount);
			dirtyPartiesList[num] = this;
			return;
		}
		double toHours = CampaignTime.Now.ToHours;
		foreach (var siegeMissileEntity in _siegeMissileEntities)
		{
			GameEntity item = siegeMissileEntity.Item1;
			ISiegeEventSide siegeEventSide = PartyBase.Settlement.SiegeEvent.GetSiegeEventSide(siegeMissileEntity.Item2);
			int item2 = siegeMissileEntity.Item3;
			bool flag = false;
			if (siegeEventSide.SiegeEngineMissiles.Count > item2)
			{
				SiegeEvent.SiegeEngineMissile siegeEngineMissile = siegeEventSide.SiegeEngineMissiles[item2];
				double toHours2 = siegeEngineMissile.CollisionTime.ToHours;
				CalculateDataAndDurationsForSiegeMachine(siegeEngineMissile.ShooterSlotIndex, siegeEngineMissile.ShooterSiegeEngineType, siegeEventSide.BattleSide, siegeEngineMissile.TargetType, siegeEngineMissile.TargetSlotIndex, out var bombardmentData);
				float num2 = bombardmentData.MissileSpeed * TaleWorlds.Library.MathF.Cos(bombardmentData.LaunchAngle);
				if (toHours > toHours2 - (double)bombardmentData.TotalDuration)
				{
					bool flag2 = toHours - (double)dt > toHours2 - (double)bombardmentData.FlightDuration && toHours - (double)dt < toHours2;
					bool flag3 = toHours > toHours2 - (double)bombardmentData.FlightDuration && toHours < toHours2;
					if (flag3)
					{
						flag = true;
						float num3 = (float)(toHours - (toHours2 - (double)bombardmentData.FlightDuration));
						float num4 = bombardmentData.MissileSpeed * TaleWorlds.Library.MathF.Sin(bombardmentData.LaunchAngle);
						Vec2 vec = new Vec2(num2 * num3, num4 * num3 - bombardmentData.Gravity * 0.5f * num3 * num3);
						Vec3 vec2 = bombardmentData.LaunchGlobalPosition + bombardmentData.TargetAlignedShooterGlobalFrame.rotation.f.NormalizedCopy() * vec.x + bombardmentData.TargetAlignedShooterGlobalFrame.rotation.u.NormalizedCopy() * vec.y;
						float num5 = num3 + 0.1f;
						Vec2 vec3 = new Vec2(num2 * num5, num4 * num5 - bombardmentData.Gravity * 0.5f * num5 * num5);
						Vec3 vec4 = bombardmentData.LaunchGlobalPosition + bombardmentData.TargetAlignedShooterGlobalFrame.rotation.f.NormalizedCopy() * vec3.x + bombardmentData.TargetAlignedShooterGlobalFrame.rotation.u.NormalizedCopy() * vec3.y;
						Mat3 rotation = item.GetGlobalFrame().rotation;
						rotation.f = vec4 - vec2;
						rotation.Orthonormalize();
						rotation.ApplyScaleLocal(MapScreen.PrefabEntityCache.GetScaleForSiegeEngine(siegeEngineMissile.ShooterSiegeEngineType, siegeEventSide.BattleSide));
						MatrixFrame frame = new MatrixFrame(rotation, vec2);
						item.SetGlobalFrame(in frame);
					}
					item.GetChild(0).SetVisibilityExcludeParents(flag3);
					int soundCodeId = -1;
					if (!flag2 && flag3)
					{
						soundCodeId = ((siegeEngineMissile.ShooterSiegeEngineType != DefaultSiegeEngineTypes.Ballista && siegeEngineMissile.ShooterSiegeEngineType != DefaultSiegeEngineTypes.FireBallista) ? ((siegeEngineMissile.ShooterSiegeEngineType != DefaultSiegeEngineTypes.Catapult && siegeEngineMissile.ShooterSiegeEngineType != DefaultSiegeEngineTypes.FireCatapult && siegeEngineMissile.ShooterSiegeEngineType != DefaultSiegeEngineTypes.Onager && siegeEngineMissile.ShooterSiegeEngineType != DefaultSiegeEngineTypes.FireOnager) ? MiscSoundContainer.SoundCodeAmbientNodeSiegeTrebuchetFire : MiscSoundContainer.SoundCodeAmbientNodeSiegeMangonelFire) : MiscSoundContainer.SoundCodeAmbientNodeSiegeBallistaFire);
					}
					else if (flag2 && !flag3)
					{
						StrategicEntity.Scene.CreateBurstParticle(ParticleSystemManager.GetRuntimeIdByName((siegeEngineMissile.TargetType == SiegeBombardTargets.RangedEngines) ? "psys_game_ballista_destruction" : "psys_campaign_boulder_stone_coll"), item.GetGlobalFrame());
						soundCodeId = ((siegeEngineMissile.ShooterSiegeEngineType == DefaultSiegeEngineTypes.Ballista || siegeEngineMissile.ShooterSiegeEngineType == DefaultSiegeEngineTypes.FireBallista) ? MiscSoundContainer.SoundCodeAmbientNodeSiegeBallistaHit : MiscSoundContainer.SoundCodeAmbientNodeSiegeBoulderHit);
					}
					MBSoundEvent.PlaySound(soundCodeId, item.GlobalPosition);
					if (!(toHours < toHours2 - (double)(bombardmentData.TotalDuration - bombardmentData.RotationDuration - bombardmentData.ReloadDuration)))
					{
						if (toHours < toHours2 - (double)(bombardmentData.TotalDuration - bombardmentData.RotationDuration - bombardmentData.ReloadDuration - bombardmentData.AimingDuration))
						{
							if (siegeEventSide.SiegeEngines.DeployedRangedSiegeEngines[siegeEngineMissile.ShooterSlotIndex] != null && siegeEventSide.SiegeEngines.DeployedRangedSiegeEngines[siegeEngineMissile.ShooterSlotIndex].SiegeEngine == siegeEngineMissile.ShooterSiegeEngineType)
							{
								foreach (var siegeRangedMachineEntity in _siegeRangedMachineEntities)
								{
									if (!flag && siegeRangedMachineEntity.Item2 == siegeEventSide.BattleSide && siegeRangedMachineEntity.Item3 == siegeEngineMissile.ShooterSlotIndex)
									{
										GameEntity item3 = siegeRangedMachineEntity.Item5;
										if (item3 != null)
										{
											flag = true;
											MatrixFrame frame2 = item3.GetGlobalFrame().TransformToParent(MBSkeletonExtensions.GetBoneEntitialFrame(item3.Skeleton, Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapProjectileBoneIndex(siegeEngineMissile.ShooterSiegeEngineType, siegeEventSide.BattleSide)));
											item.SetGlobalFrame(in frame2);
										}
									}
								}
							}
						}
						else if (toHours < toHours2 - (double)(bombardmentData.TotalDuration - bombardmentData.RotationDuration - bombardmentData.ReloadDuration - bombardmentData.AimingDuration - bombardmentData.FireDuration) && !flag3 && siegeEventSide.SiegeEngines.DeployedRangedSiegeEngines[siegeEngineMissile.ShooterSlotIndex] != null && siegeEventSide.SiegeEngines.DeployedRangedSiegeEngines[siegeEngineMissile.ShooterSlotIndex].SiegeEngine == siegeEngineMissile.ShooterSiegeEngineType)
						{
							foreach (var siegeRangedMachineEntity2 in _siegeRangedMachineEntities)
							{
								if (!flag && siegeRangedMachineEntity2.Item2 == siegeEventSide.BattleSide && siegeRangedMachineEntity2.Item3 == siegeEngineMissile.ShooterSlotIndex)
								{
									GameEntity item4 = siegeRangedMachineEntity2.Item5;
									if (item4 != null)
									{
										flag = true;
										MatrixFrame frame2 = item4.GetGlobalFrame().TransformToParent(MBSkeletonExtensions.GetBoneEntitialFrame(item4.Skeleton, Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapProjectileBoneIndex(siegeEngineMissile.ShooterSiegeEngineType, siegeEventSide.BattleSide)));
										item.SetGlobalFrame(in frame2);
									}
								}
							}
						}
					}
				}
			}
			item.SetVisibilityExcludeParents(flag);
		}
		foreach (var siegeRangedMachineEntity3 in _siegeRangedMachineEntities)
		{
			GameEntity item5 = siegeRangedMachineEntity3.Item1;
			BattleSideEnum item6 = siegeRangedMachineEntity3.Item2;
			int item7 = siegeRangedMachineEntity3.Item3;
			GameEntity item8 = siegeRangedMachineEntity3.Item5;
			SiegeEngineType siegeEngine = PartyBase.Settlement.SiegeEvent.GetSiegeEventSide(item6).SiegeEngines.DeployedRangedSiegeEngines[item7].SiegeEngine;
			if (!(item8 != null))
			{
				continue;
			}
			Skeleton skeleton = item8.Skeleton;
			string siegeEngineMapFireAnimationName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapFireAnimationName(siegeEngine, item6);
			string siegeEngineMapReloadAnimationName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapReloadAnimationName(siegeEngine, item6);
			SiegeEvent.RangedSiegeEngine rangedSiegeEngine = PartyBase.Settlement.SiegeEvent.GetSiegeEventSide(item6).SiegeEngines.DeployedRangedSiegeEngines[item7].RangedSiegeEngine;
			CalculateDataAndDurationsForSiegeMachine(item7, siegeEngine, item6, rangedSiegeEngine.CurrentTargetType, rangedSiegeEngine.CurrentTargetIndex, out var bombardmentData2);
			MatrixFrame frame3 = bombardmentData2.ShooterGlobalFrame;
			if (rangedSiegeEngine.PreviousTargetIndex >= 0)
			{
				Vec3 vec5 = ((rangedSiegeEngine.PreviousDamagedTargetType != SiegeBombardTargets.Wall) ? ((item6 == BattleSideEnum.Attacker) ? _defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[rangedSiegeEngine.PreviousTargetIndex].GetGlobalFrame().origin : _attackerRangedEngineSpawnEntities[rangedSiegeEngine.PreviousTargetIndex].GetGlobalFrame().origin) : _defenderBreachableWallEntitiesCacheForCurrentLevel[rangedSiegeEngine.PreviousTargetIndex].GlobalPosition);
				frame3.rotation.f.AsVec2 = (vec5 - frame3.origin).AsVec2;
				frame3.rotation.f.NormalizeWithoutChangingZ();
				frame3.rotation.Orthonormalize();
			}
			item5.SetGlobalFrame(in frame3);
			skeleton.TickAnimations(dt, MatrixFrame.Identity, tickAnimsForChildren: false);
			double toHours3 = rangedSiegeEngine.NextProjectileCollisionTime.ToHours;
			if (!(toHours > toHours3 - (double)bombardmentData2.TotalDuration))
			{
				continue;
			}
			if (toHours < toHours3 - (double)(bombardmentData2.TotalDuration - bombardmentData2.RotationDuration))
			{
				float rotationInRadians = (bombardmentData2.TargetPosition - frame3.origin).AsVec2.RotationInRadians;
				float rotationInRadians2 = frame3.rotation.f.AsVec2.RotationInRadians;
				float f = rotationInRadians - rotationInRadians2;
				float num6 = TaleWorlds.Library.MathF.Abs(f);
				float num7 = (float)(toHours3 - (double)(bombardmentData2.TotalDuration - bombardmentData2.RotationDuration) - toHours);
				if (num6 > num7 * 2f)
				{
					frame3.rotation.f.AsVec2 = Vec2.FromRotation(rotationInRadians2 + (float)TaleWorlds.Library.MathF.Sign(f) * (num6 - num7 * 2f));
					frame3.rotation.f.NormalizeWithoutChangingZ();
					frame3.rotation.Orthonormalize();
					item5.SetGlobalFrame(in frame3);
				}
			}
			else if (toHours < toHours3 - (double)(bombardmentData2.TotalDuration - bombardmentData2.RotationDuration - bombardmentData2.ReloadDuration))
			{
				item5.SetGlobalFrame(in bombardmentData2.TargetAlignedShooterGlobalFrame);
				skeleton.SetAnimationAtChannel(siegeEngineMapReloadAnimationName, 0, 1f, 0f, (float)((toHours - (toHours3 - (double)(bombardmentData2.TotalDuration - bombardmentData2.RotationDuration))) / (double)bombardmentData2.ReloadDuration));
			}
			else if (toHours < toHours3 - (double)(bombardmentData2.TotalDuration - bombardmentData2.RotationDuration - bombardmentData2.ReloadDuration - bombardmentData2.AimingDuration))
			{
				item5.SetGlobalFrame(in bombardmentData2.TargetAlignedShooterGlobalFrame);
				skeleton.SetAnimationAtChannel(siegeEngineMapReloadAnimationName, 0, 1f, 0f, 1f);
			}
			else if (toHours < toHours3 - (double)(bombardmentData2.TotalDuration - bombardmentData2.RotationDuration - bombardmentData2.ReloadDuration - bombardmentData2.AimingDuration - bombardmentData2.FireDuration))
			{
				item5.SetGlobalFrame(in bombardmentData2.TargetAlignedShooterGlobalFrame);
				skeleton.SetAnimationAtChannel(siegeEngineMapFireAnimationName, 0, 1f, 0f, (float)((toHours - (toHours3 - (double)(bombardmentData2.TotalDuration - bombardmentData2.RotationDuration - bombardmentData2.ReloadDuration - bombardmentData2.AimingDuration))) / (double)bombardmentData2.FireDuration));
			}
			else
			{
				item5.SetGlobalFrame(in bombardmentData2.TargetAlignedShooterGlobalFrame);
				skeleton.SetAnimationAtChannel(siegeEngineMapFireAnimationName, 0, 1f, 0f, 1f);
			}
		}
	}

	private void TickMobilePartyVisual(float dt, ref int dirtyPartiesCount, ref PartyVisual[] dirtyPartiesList)
	{
		if (StrategicEntity == null)
		{
			return;
		}
		if (PartyBase.IsVisualDirty && (_entityAlpha > 0f || TargetVisibility))
		{
			int num = Interlocked.Increment(ref dirtyPartiesCount);
			dirtyPartiesList[num] = this;
		}
		_speed = PartyBase.MobileParty.Speed;
		if (_entityAlpha > 0f && HumanAgentVisuals != null && !HumanAgentVisuals.GetEquipment()[EquipmentIndex.ExtraWeaponSlot].IsEmpty)
		{
			HumanAgentVisuals.SetClothWindToWeaponAtIndex(-StrategicEntity.GetGlobalFrame().rotation.f, false, EquipmentIndex.ExtraWeaponSlot);
		}
		float num2 = ((MountAgentVisuals != null) ? 1.3f : 1f);
		float num3 = TaleWorlds.Library.MathF.Min(0.25f * num2 * _speed / 0.3f, 20f);
		bool flag = IsEntityMovingVisually();
		AgentVisuals humanAgentVisuals = HumanAgentVisuals;
		if (humanAgentVisuals != null)
		{
			humanAgentVisuals.Tick(MountAgentVisuals, dt, flag, num3);
		}
		AgentVisuals mountAgentVisuals = MountAgentVisuals;
		if (mountAgentVisuals != null)
		{
			mountAgentVisuals.Tick((AgentVisuals)null, dt, flag, num3);
		}
		AgentVisuals caravanMountAgentVisuals = CaravanMountAgentVisuals;
		if (caravanMountAgentVisuals != null)
		{
			caravanMountAgentVisuals.Tick((AgentVisuals)null, dt, flag, num3);
		}
		if (!IsVisibleOrFadingOut())
		{
			return;
		}
		MobileParty mobileParty = PartyBase.MobileParty;
		MatrixFrame frame = MatrixFrame.Identity;
		frame.origin = GetVisualPosition();
		if (mobileParty.Army != null && mobileParty.Army.LeaderParty.AttachedParties.Contains(mobileParty))
		{
			MatrixFrame frame2 = GetFrame();
			Vec2 vec = frame.origin.AsVec2 - frame2.origin.AsVec2;
			if (vec.Length / dt > 20f)
			{
				frame.rotation.RotateAboutUp(PartyBase.AverageBearingRotation);
			}
			else if (mobileParty.CurrentSettlement == null)
			{
				float a = MBMath.LerpRadians(frame2.rotation.f.AsVec2.RotationInRadians, (vec + Vec2.FromRotation(PartyBase.AverageBearingRotation) * 0.01f).RotationInRadians, 6f * dt, 0.03f * dt, 10f * dt);
				frame.rotation.RotateAboutUp(a);
			}
			else
			{
				float rotationInRadians = frame2.rotation.f.AsVec2.RotationInRadians;
				frame.rotation.RotateAboutUp(rotationInRadians);
			}
		}
		else if (mobileParty.CurrentSettlement == null)
		{
			frame.rotation.RotateAboutUp(PartyBase.AverageBearingRotation);
		}
		SetFrame(ref frame);
	}

	public Vec3 GetVisualPosition()
	{
		float height = 0f;
		Vec2 vec = Vec2.Zero;
		if (PartyBase.IsMobile)
		{
			MobileParty mobileParty = PartyBase.MobileParty;
			vec = new Vec2(mobileParty.EventPositionAdder.x + mobileParty.ArmyPositionAdder.x + mobileParty.ErrorPosition.x, mobileParty.EventPositionAdder.y + mobileParty.ArmyPositionAdder.y + mobileParty.ErrorPosition.y);
		}
		Vec2 vec2 = new Vec2(PartyBase.Position2D.x + vec.x, PartyBase.Position2D.y + vec.y);
		Campaign.Current.MapSceneWrapper.GetHeightAtPoint(vec2, ref height);
		return new Vec3(vec2, height);
	}

	private void CalculateDataAndDurationsForSiegeMachine(int machineSlotIndex, SiegeEngineType machineType, BattleSideEnum side, SiegeBombardTargets targetType, int targetSlotIndex, out SiegeBombardmentData bombardmentData)
	{
		bombardmentData = default(SiegeBombardmentData);
		MatrixFrame shooterGlobalFrame = ((side == BattleSideEnum.Defender) ? _defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[machineSlotIndex].GetGlobalFrame() : _attackerRangedEngineSpawnEntities[machineSlotIndex].GetGlobalFrame());
		shooterGlobalFrame.rotation.MakeUnit();
		bombardmentData.ShooterGlobalFrame = shooterGlobalFrame;
		string siegeEngineMapFireAnimationName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapFireAnimationName(machineType, side);
		string siegeEngineMapReloadAnimationName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapReloadAnimationName(machineType, side);
		bombardmentData.ReloadDuration = MBAnimation.GetAnimationDuration(siegeEngineMapReloadAnimationName) * 0.25f;
		bombardmentData.AimingDuration = 0.25f;
		bombardmentData.RotationDuration = 0.4f;
		bombardmentData.FireDuration = MBAnimation.GetAnimationDuration(siegeEngineMapFireAnimationName) * 0.25f;
		float animationParameter = MBAnimation.GetAnimationParameter1(siegeEngineMapFireAnimationName);
		bombardmentData.MissileLaunchDuration = bombardmentData.FireDuration * animationParameter;
		bombardmentData.MissileSpeed = 14f;
		bombardmentData.Gravity = ((machineType == DefaultSiegeEngineTypes.Ballista || machineType == DefaultSiegeEngineTypes.FireBallista) ? 10f : 40f);
		switch (targetType)
		{
		case SiegeBombardTargets.RangedEngines:
			bombardmentData.TargetPosition = ((side == BattleSideEnum.Attacker) ? _defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[targetSlotIndex].GetGlobalFrame().origin : _attackerRangedEngineSpawnEntities[targetSlotIndex].GetGlobalFrame().origin);
			break;
		case SiegeBombardTargets.Wall:
			bombardmentData.TargetPosition = _defenderBreachableWallEntitiesCacheForCurrentLevel[targetSlotIndex].GlobalPosition;
			break;
		default:
			if (targetSlotIndex == -1)
			{
				bombardmentData.TargetPosition = Vec3.Zero;
				break;
			}
			bombardmentData.TargetPosition = ((side == BattleSideEnum.Attacker) ? _defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[targetSlotIndex].GetGlobalFrame().origin : _attackerRangedEngineSpawnEntities[targetSlotIndex].GetGlobalFrame().origin);
			bombardmentData.TargetPosition += (bombardmentData.TargetPosition - bombardmentData.ShooterGlobalFrame.origin).NormalizedCopy() * 2f;
			Campaign.Current.MapSceneWrapper.GetHeightAtPoint(bombardmentData.TargetPosition.AsVec2, ref bombardmentData.TargetPosition.z);
			break;
		}
		bombardmentData.TargetAlignedShooterGlobalFrame = bombardmentData.ShooterGlobalFrame;
		bombardmentData.TargetAlignedShooterGlobalFrame.rotation.f.AsVec2 = (bombardmentData.TargetPosition - bombardmentData.ShooterGlobalFrame.origin).AsVec2;
		bombardmentData.TargetAlignedShooterGlobalFrame.rotation.f.NormalizeWithoutChangingZ();
		bombardmentData.TargetAlignedShooterGlobalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		bombardmentData.LaunchGlobalPosition = bombardmentData.TargetAlignedShooterGlobalFrame.TransformToParent(MapScreen.PrefabEntityCache.GetLaunchEntitialFrameForSiegeEngine(machineType, side).origin);
		float lengthSquared = (bombardmentData.LaunchGlobalPosition.AsVec2 - bombardmentData.TargetPosition.AsVec2).LengthSquared;
		float num = TaleWorlds.Library.MathF.Sqrt(lengthSquared);
		float num2 = bombardmentData.LaunchGlobalPosition.z - bombardmentData.TargetPosition.z;
		float num3 = bombardmentData.MissileSpeed * bombardmentData.MissileSpeed;
		float num4 = num3 * num3;
		float num5 = num4 - bombardmentData.Gravity * (bombardmentData.Gravity * lengthSquared - 2f * num2 * num3);
		if (num5 >= 0f)
		{
			bombardmentData.LaunchAngle = TaleWorlds.Library.MathF.Atan((num3 - TaleWorlds.Library.MathF.Sqrt(num5)) / (bombardmentData.Gravity * num));
		}
		else
		{
			bombardmentData.Gravity = 1f;
			num5 = num4 - bombardmentData.Gravity * (bombardmentData.Gravity * lengthSquared - 2f * num2 * num3);
			bombardmentData.LaunchAngle = TaleWorlds.Library.MathF.Atan((num3 - TaleWorlds.Library.MathF.Sqrt(num5)) / (bombardmentData.Gravity * num));
		}
		float num6 = bombardmentData.MissileSpeed * TaleWorlds.Library.MathF.Cos(bombardmentData.LaunchAngle);
		bombardmentData.FlightDuration = num / num6;
		bombardmentData.TotalDuration = bombardmentData.RotationDuration + bombardmentData.ReloadDuration + bombardmentData.AimingDuration + bombardmentData.MissileLaunchDuration + bombardmentData.FlightDuration;
	}

	private void RemoveContourMesh()
	{
		if (_contourMaskMesh != null)
		{
			MapScreen.ContourMaskEntity.RemoveComponentWithMesh(_contourMaskMesh);
			_contourMaskMesh = null;
		}
	}

	public void ReleaseResources()
	{
		RemoveSiege();
		ResetPartyIcon();
	}

	public void ValidateIsDirty(float realDt, float dt)
	{
		if (PartyBase.IsSettlement)
		{
			RefreshPartyIcon();
			PartyVisualManager.Current.RegisterFadingVisual(this);
		}
		else if (PartyBase.MemberRoster.TotalManCount != 0)
		{
			RefreshPartyIcon();
			if ((_entityAlpha < 1f && TargetVisibility) || (_entityAlpha > 0f && !TargetVisibility))
			{
				PartyVisualManager.Current.RegisterFadingVisual(this);
			}
		}
		else
		{
			ResetPartyIcon();
		}
	}

	public void TickFadingState(float realDt, float dt)
	{
		if ((_entityAlpha < 1f && TargetVisibility) || (_entityAlpha > 0f && !TargetVisibility))
		{
			if (TargetVisibility)
			{
				if (_entityAlpha <= 0f)
				{
					StrategicEntity.SetVisibilityExcludeParents(visible: true);
					AgentVisuals humanAgentVisuals = HumanAgentVisuals;
					if (humanAgentVisuals != null)
					{
						humanAgentVisuals.GetEntity()?.SetVisibilityExcludeParents(visible: true);
					}
					AgentVisuals mountAgentVisuals = MountAgentVisuals;
					if (mountAgentVisuals != null)
					{
						mountAgentVisuals.GetEntity()?.SetVisibilityExcludeParents(visible: true);
					}
					AgentVisuals caravanMountAgentVisuals = CaravanMountAgentVisuals;
					if (caravanMountAgentVisuals != null)
					{
						caravanMountAgentVisuals.GetEntity()?.SetVisibilityExcludeParents(visible: true);
					}
				}
				_entityAlpha = TaleWorlds.Library.MathF.Min(_entityAlpha + realDt * 2f, 1f);
				StrategicEntity.SetAlpha(_entityAlpha);
				AgentVisuals humanAgentVisuals2 = HumanAgentVisuals;
				if (humanAgentVisuals2 != null)
				{
					humanAgentVisuals2.GetEntity()?.SetAlpha(_entityAlpha);
				}
				AgentVisuals mountAgentVisuals2 = MountAgentVisuals;
				if (mountAgentVisuals2 != null)
				{
					mountAgentVisuals2.GetEntity()?.SetAlpha(_entityAlpha);
				}
				AgentVisuals caravanMountAgentVisuals2 = CaravanMountAgentVisuals;
				if (caravanMountAgentVisuals2 != null)
				{
					caravanMountAgentVisuals2.GetEntity()?.SetAlpha(_entityAlpha);
				}
				StrategicEntity.EntityFlags &= ~EntityFlags.DoNotTick;
				return;
			}
			_entityAlpha = TaleWorlds.Library.MathF.Max(_entityAlpha - realDt * 2f, 0f);
			StrategicEntity.SetAlpha(_entityAlpha);
			AgentVisuals humanAgentVisuals3 = HumanAgentVisuals;
			if (humanAgentVisuals3 != null)
			{
				humanAgentVisuals3.GetEntity()?.SetAlpha(_entityAlpha);
			}
			AgentVisuals mountAgentVisuals3 = MountAgentVisuals;
			if (mountAgentVisuals3 != null)
			{
				mountAgentVisuals3.GetEntity()?.SetAlpha(_entityAlpha);
			}
			AgentVisuals caravanMountAgentVisuals3 = CaravanMountAgentVisuals;
			if (caravanMountAgentVisuals3 != null)
			{
				caravanMountAgentVisuals3.GetEntity()?.SetAlpha(_entityAlpha);
			}
			if (_entityAlpha <= 0f)
			{
				StrategicEntity.SetVisibilityExcludeParents(visible: false);
				AgentVisuals humanAgentVisuals4 = HumanAgentVisuals;
				if (humanAgentVisuals4 != null)
				{
					humanAgentVisuals4.GetEntity()?.SetVisibilityExcludeParents(visible: false);
				}
				AgentVisuals mountAgentVisuals4 = MountAgentVisuals;
				if (mountAgentVisuals4 != null)
				{
					mountAgentVisuals4.GetEntity()?.SetVisibilityExcludeParents(visible: false);
				}
				AgentVisuals caravanMountAgentVisuals4 = CaravanMountAgentVisuals;
				if (caravanMountAgentVisuals4 != null)
				{
					caravanMountAgentVisuals4.GetEntity()?.SetVisibilityExcludeParents(visible: false);
				}
				StrategicEntity.EntityFlags |= EntityFlags.DoNotTick;
			}
		}
		else
		{
			PartyVisualManager.Current.UnRegisterFadingVisual(this);
		}
	}

	public void ResetPartyIcon()
	{
		if (StrategicEntity != null)
		{
			RemoveContourMesh();
		}
		if (HumanAgentVisuals != null)
		{
			HumanAgentVisuals.Reset();
			HumanAgentVisuals = null;
		}
		if (MountAgentVisuals != null)
		{
			MountAgentVisuals.Reset();
			MountAgentVisuals = null;
		}
		if (CaravanMountAgentVisuals != null)
		{
			CaravanMountAgentVisuals.Reset();
			CaravanMountAgentVisuals = null;
		}
		if (StrategicEntity != null)
		{
			if ((StrategicEntity.EntityFlags & EntityFlags.Ignore) != 0)
			{
				StrategicEntity.RemoveFromPredisplayEntity();
			}
			StrategicEntity.ClearComponents();
		}
		PartyVisualManager.Current.UnRegisterFadingVisual(this);
	}

	private void RefreshPartyIcon()
	{
		if (!PartyBase.IsVisualDirty)
		{
			return;
		}
		PartyBase.OnVisualsUpdated();
		bool flag = true;
		bool clearBannerComponentCache = true;
		if (!PartyBase.IsSettlement)
		{
			ResetPartyIcon();
			MatrixFrame circleLocalFrame = CircleLocalFrame;
			circleLocalFrame.origin = Vec3.Zero;
			CircleLocalFrame = circleLocalFrame;
		}
		else
		{
			RemoveSiege();
			StrategicEntity.RemoveAllParticleSystems();
			StrategicEntity.EntityFlags |= EntityFlags.DoNotTick;
		}
		if (PartyBase.MobileParty?.CurrentSettlement != null)
		{
			Dictionary<int, List<GameEntity>> gateBannerEntitiesWithLevels = PartyVisualManager.Current.GetVisualOfParty(PartyBase.MobileParty.CurrentSettlement.Party)._gateBannerEntitiesWithLevels;
			if (!PartyBase.MobileParty.MapFaction.IsAtWarWith(PartyBase.MobileParty.CurrentSettlement.MapFaction) && gateBannerEntitiesWithLevels != null && !gateBannerEntitiesWithLevels.IsEmpty() && PartyBase.LeaderHero?.ClanBanner != null)
			{
				string text = PartyBase.LeaderHero.ClanBanner.Serialize();
				if (!string.IsNullOrEmpty(text))
				{
					int num = 0;
					foreach (MobileParty party in PartyBase.MobileParty.CurrentSettlement.Parties)
					{
						if (party == PartyBase.MobileParty)
						{
							break;
						}
						if (party.LeaderHero?.ClanBanner != null)
						{
							num++;
						}
					}
					MatrixFrame identity = MatrixFrame.Identity;
					int wallLevel = PartyBase.MobileParty.CurrentSettlement.Town.GetWallLevel();
					int count = gateBannerEntitiesWithLevels[wallLevel].Count;
					if (count == 0)
					{
						Debug.FailedAssert($"{PartyBase.MobileParty.CurrentSettlement.Name} - has no Banner Entities at level {wallLevel}.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\PartyVisual.cs", "RefreshPartyIcon", 1060);
					}
					else
					{
						GameEntity gameEntity = gateBannerEntitiesWithLevels[wallLevel][num % count];
						GameEntity child = gameEntity.GetChild(0);
						MatrixFrame matrixFrame = ((child != null) ? child.GetGlobalFrame() : gameEntity.GetGlobalFrame());
						num /= count;
						int num2 = PartyBase.MobileParty.CurrentSettlement.Parties.Count((MobileParty p) => p.LeaderHero?.ClanBanner != null);
						float num3 = 0.75f / (float)TaleWorlds.Library.MathF.Max(1, num2 / (count * 2));
						int num4 = ((num % 2 != 0) ? 1 : (-1));
						Vec3 vec = matrixFrame.rotation.f / 2f * num4;
						if (vec.Length < matrixFrame.rotation.s.Length)
						{
							vec = matrixFrame.rotation.s / 2f * num4;
						}
						identity.origin = matrixFrame.origin + vec * ((num + 1) / 2) * (num % 2 * 2 - 1) * num3 * num4;
						identity.origin = StrategicEntity.GetGlobalFrame().TransformToLocal(identity.origin);
						float num5 = MBMath.Map((float)PartyBase.NumberOfAllMembers / 400f * ((PartyBase.MobileParty.Army != null && PartyBase.MobileParty.Army.LeaderParty == PartyBase.MobileParty) ? 1.25f : 1f), 0f, 1f, 0.2f, 0.5f);
						identity = identity.Elevate(0f - num5);
						identity.rotation.ApplyScaleLocal(num5);
						identity.rotation = StrategicEntity.GetGlobalFrame().rotation.TransformToLocal(identity.rotation);
						StrategicEntity.AddSphereAsBody(identity.origin + Vec3.Up * 0.3f, 0.15f, BodyFlags.None);
						flag = false;
						string text2 = "campaign_flag";
						if (_cachedBannerComponent.Item1 == text + text2)
						{
							_cachedBannerComponent.Item2.GetFirstMetaMesh().Frame = identity;
							StrategicEntity.AddComponent(_cachedBannerComponent.Item2);
						}
						else
						{
							MetaMesh bannerOfCharacter = GetBannerOfCharacter(new Banner(text), text2);
							bannerOfCharacter.Frame = identity;
							int componentCount = StrategicEntity.GetComponentCount(GameEntity.ComponentType.ClothSimulator);
							StrategicEntity.AddMultiMesh(bannerOfCharacter);
							if (StrategicEntity.GetComponentCount(GameEntity.ComponentType.ClothSimulator) > componentCount)
							{
								_cachedBannerComponent.Item1 = text + text2;
								_cachedBannerComponent.Item2 = StrategicEntity.GetComponentAtIndex(componentCount, GameEntity.ComponentType.ClothSimulator);
							}
						}
					}
				}
			}
			else
			{
				StrategicEntity.RemovePhysics();
			}
		}
		else
		{
			IsEnemy = PartyBase.MapFaction != null && FactionManager.IsAtWarAgainstFaction(PartyBase.MapFaction, Hero.MainHero.MapFaction);
			IsFriendly = PartyBase.MapFaction != null && FactionManager.IsAlliedWithFaction(PartyBase.MapFaction, Hero.MainHero.MapFaction);
			InitializePartyCollider(PartyBase);
			if (PartyBase.IsSettlement)
			{
				if (PartyBase.Settlement.IsFortification)
				{
					UpdateDefenderSiegeEntitiesCache();
				}
				AddSiegeIconComponents(PartyBase);
				SetSettlementLevelVisibility();
				RefreshWallState();
				RefreshTownPhysicalEntitiesState(PartyBase);
				RefreshSiegePreparations(PartyBase);
				if (PartyBase.Settlement.IsVillage)
				{
					MapEvent mapEvent = PartyBase.MapEvent;
					if (mapEvent != null && mapEvent.IsRaid)
					{
						StrategicEntity.EntityFlags &= ~EntityFlags.DoNotTick;
						StrategicEntity.AddParticleSystemComponent("psys_fire_smoke_env_point");
					}
					else if (PartyBase.Settlement.IsRaided)
					{
						StrategicEntity.EntityFlags &= ~EntityFlags.DoNotTick;
						StrategicEntity.AddParticleSystemComponent("map_icon_village_plunder_fx");
					}
				}
			}
			else
			{
				AddMobileIconComponents(PartyBase, ref clearBannerComponentCache, ref clearBannerComponentCache);
			}
		}
		if (flag)
		{
			_cachedBannerComponent = (null, null);
		}
		if (clearBannerComponentCache)
		{
			_cachedBannerEntity = (null, null);
		}
		StrategicEntity.CheckResources(addToQueue: true, checkFaceResources: false);
	}

	private void RemoveSiege()
	{
		foreach (var siegeRangedMachineEntity in _siegeRangedMachineEntities)
		{
			StrategicEntity.RemoveChild(siegeRangedMachineEntity.Item1, keepPhysics: false, keepScenePointer: false, callScriptCallbacks: true, 36);
		}
		foreach (var siegeMissileEntity in _siegeMissileEntities)
		{
			StrategicEntity.RemoveChild(siegeMissileEntity.Item1, keepPhysics: false, keepScenePointer: false, callScriptCallbacks: true, 37);
		}
		foreach (var siegeMeleeMachineEntity in _siegeMeleeMachineEntities)
		{
			StrategicEntity.RemoveChild(siegeMeleeMachineEntity.Item1, keepPhysics: false, keepScenePointer: false, callScriptCallbacks: true, 38);
		}
		_siegeRangedMachineEntities.Clear();
		_siegeMeleeMachineEntities.Clear();
		_siegeMissileEntities.Clear();
	}

	private void RefreshSiegePreparations(PartyBase party)
	{
		List<GameEntity> children = new List<GameEntity>();
		StrategicEntity.GetChildrenRecursive(ref children);
		List<GameEntity> list = children.FindAll((GameEntity x) => x.HasTag("siege_preparation"));
		bool flag = false;
		if (party.Settlement != null && party.Settlement.IsUnderSiege)
		{
			SiegeEvent.SiegeEngineConstructionProgress siegePreparations = party.Settlement.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.SiegePreparations;
			if (siegePreparations != null && siegePreparations.Progress >= 1f)
			{
				flag = true;
				foreach (GameEntity item in list)
				{
					item.SetVisibilityExcludeParents(visible: true);
				}
			}
		}
		if (flag)
		{
			return;
		}
		foreach (GameEntity item2 in list)
		{
			item2.SetVisibilityExcludeParents(visible: false);
		}
	}

	private void AddSiegeIconComponents(PartyBase party)
	{
		if (!party.Settlement.IsUnderSiege)
		{
			return;
		}
		int wallLevel = -1;
		if (party.Settlement.SiegeEvent.BesiegedSettlement.IsTown || party.Settlement.SiegeEvent.BesiegedSettlement.IsCastle)
		{
			wallLevel = party.Settlement.SiegeEvent.BesiegedSettlement.Town.GetWallLevel();
		}
		SiegeEvent.SiegeEngineConstructionProgress[] deployedRangedSiegeEngines = party.Settlement.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.DeployedRangedSiegeEngines;
		for (int i = 0; i < deployedRangedSiegeEngines.Length; i++)
		{
			SiegeEvent.SiegeEngineConstructionProgress obj = deployedRangedSiegeEngines[i];
			if (obj != null && obj.IsActive && i < _attackerRangedEngineSpawnEntities.Length)
			{
				MatrixFrame globalFrame = _attackerRangedEngineSpawnEntities[i].GetGlobalFrame();
				globalFrame.rotation.MakeUnit();
				AddSiegeMachine(deployedRangedSiegeEngines[i].SiegeEngine, globalFrame, BattleSideEnum.Attacker, wallLevel, i);
			}
		}
		SiegeEvent.SiegeEngineConstructionProgress[] deployedMeleeSiegeEngines = party.Settlement.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.DeployedMeleeSiegeEngines;
		for (int j = 0; j < deployedMeleeSiegeEngines.Length; j++)
		{
			SiegeEvent.SiegeEngineConstructionProgress obj2 = deployedMeleeSiegeEngines[j];
			if (obj2 == null || !obj2.IsActive)
			{
				continue;
			}
			if (deployedMeleeSiegeEngines[j].SiegeEngine == DefaultSiegeEngineTypes.SiegeTower)
			{
				int num = j - _attackerBatteringRamSpawnEntities.Length;
				if (num >= 0)
				{
					MatrixFrame globalFrame2 = _attackerSiegeTowerSpawnEntities[num].GetGlobalFrame();
					globalFrame2.rotation.MakeUnit();
					AddSiegeMachine(deployedMeleeSiegeEngines[j].SiegeEngine, globalFrame2, BattleSideEnum.Attacker, wallLevel, j);
				}
			}
			else if (deployedMeleeSiegeEngines[j].SiegeEngine == DefaultSiegeEngineTypes.Ram || deployedMeleeSiegeEngines[j].SiegeEngine == DefaultSiegeEngineTypes.ImprovedRam)
			{
				int num2 = j;
				if (num2 >= 0)
				{
					MatrixFrame globalFrame3 = _attackerBatteringRamSpawnEntities[num2].GetGlobalFrame();
					globalFrame3.rotation.MakeUnit();
					AddSiegeMachine(deployedMeleeSiegeEngines[j].SiegeEngine, globalFrame3, BattleSideEnum.Attacker, wallLevel, j);
				}
			}
		}
		SiegeEvent.SiegeEngineConstructionProgress[] deployedRangedSiegeEngines2 = party.Settlement.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Defender).SiegeEngines.DeployedRangedSiegeEngines;
		for (int k = 0; k < deployedRangedSiegeEngines2.Length; k++)
		{
			SiegeEvent.SiegeEngineConstructionProgress obj3 = deployedRangedSiegeEngines2[k];
			if (obj3 != null && obj3.IsActive && k < _defenderBreachableWallEntitiesCacheForCurrentLevel.Length)
			{
				MatrixFrame globalFrame4 = _defenderBreachableWallEntitiesCacheForCurrentLevel[k].GetGlobalFrame();
				globalFrame4.rotation.MakeUnit();
				AddSiegeMachine(deployedRangedSiegeEngines2[k].SiegeEngine, globalFrame4, BattleSideEnum.Defender, wallLevel, k);
			}
		}
		for (int l = 0; l < 2; l++)
		{
			BattleSideEnum side = ((l == 0) ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
			MBReadOnlyList<SiegeEvent.SiegeEngineMissile> siegeEngineMissiles = party.Settlement.SiegeEvent.GetSiegeEventSide(side).SiegeEngineMissiles;
			for (int m = 0; m < siegeEngineMissiles.Count; m++)
			{
				AddSiegeMissile(siegeEngineMissiles[m].ShooterSiegeEngineType, StrategicEntity.GetGlobalFrame(), side, m);
			}
		}
	}

	private void AddSiegeMachine(SiegeEngineType type, MatrixFrame globalFrame, BattleSideEnum side, int wallLevel, int slotIndex)
	{
		string siegeEngineMapPrefabName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapPrefabName(type, wallLevel, side);
		GameEntity gameEntity = GameEntity.Instantiate(MapScene, siegeEngineMapPrefabName, callScriptCallbacks: true);
		if (!(gameEntity != null))
		{
			return;
		}
		StrategicEntity.AddChild(gameEntity);
		gameEntity.GetFrame(out var frame);
		MatrixFrame frame2 = globalFrame.TransformToParent(frame);
		gameEntity.SetGlobalFrame(in frame2);
		List<GameEntity> children = new List<GameEntity>();
		gameEntity.GetChildrenRecursive(ref children);
		GameEntity gameEntity2 = null;
		if (children.Any((GameEntity entity) => entity.HasTag("siege_machine_mapicon_skeleton")))
		{
			GameEntity gameEntity3 = children.Find((GameEntity entity) => entity.HasTag("siege_machine_mapicon_skeleton"));
			if (gameEntity3.Skeleton != null)
			{
				gameEntity2 = gameEntity3;
				string siegeEngineMapFireAnimationName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapFireAnimationName(type, side);
				gameEntity2.Skeleton.SetAnimationAtChannel(siegeEngineMapFireAnimationName, 0, 1f, 0f, 1f);
			}
		}
		if (type.IsRanged)
		{
			_siegeRangedMachineEntities.Add(ValueTuple.Create(gameEntity, side, slotIndex, globalFrame, gameEntity2));
		}
		else
		{
			_siegeMeleeMachineEntities.Add(ValueTuple.Create(gameEntity, side, slotIndex, globalFrame, gameEntity2));
		}
	}

	private void AddSiegeMissile(SiegeEngineType type, MatrixFrame globalFrame, BattleSideEnum side, int missileIndex)
	{
		string siegeEngineMapProjectilePrefabName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapProjectilePrefabName(type);
		GameEntity gameEntity = GameEntity.Instantiate(MapScene, siegeEngineMapProjectilePrefabName, callScriptCallbacks: true);
		if (gameEntity != null)
		{
			_siegeMissileEntities.Add(ValueTuple.Create(gameEntity, side, missileIndex));
			StrategicEntity.AddChild(gameEntity);
			StrategicEntity.EntityFlags &= ~EntityFlags.DoNotTick;
			gameEntity.GetFrame(out var frame);
			MatrixFrame frame2 = globalFrame.TransformToParent(frame);
			gameEntity.SetGlobalFrame(in frame2);
			gameEntity.SetVisibilityExcludeParents(visible: false);
		}
	}

	private void AddMobileIconComponents(PartyBase party, ref bool clearBannerComponentCache, ref bool clearBannerEntityCache)
	{
		uint contourColor = (FactionManager.IsAtWarAgainstFaction(party.MapFaction, Hero.MainHero.MapFaction) ? 4294905856u : 4278206719u);
		if (party.MobileParty.BesiegedSettlement?.SiegeEvent != null && party.MobileParty.BesiegedSettlement.SiegeEvent.BesiegerCamp.HasInvolvedPartyForEventType(party))
		{
			GameEntity gameEntity = GameEntity.CreateEmpty(StrategicEntity.Scene);
			gameEntity.AddMultiMesh(MetaMesh.GetCopy("map_icon_siege_camp_tent"));
			MatrixFrame frame = MatrixFrame.Identity;
			frame.rotation.ApplyScaleLocal(1.2f);
			gameEntity.SetFrame(ref frame);
			string text = null;
			if (party.LeaderHero?.ClanBanner != null)
			{
				text = party.LeaderHero.ClanBanner.Serialize();
			}
			bool flag = party.MobileParty.Army != null && party.MobileParty.Army.LeaderParty == party.MobileParty;
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin.z += (flag ? 0.2f : 0.15f);
			identity.rotation.RotateAboutUp((float)Math.PI / 2f);
			float scaleAmount = MBMath.Map(party.TotalStrength / 500f * ((party.MobileParty.Army != null && flag) ? 1f : 0.8f), 0f, 1f, 0.15f, 0.5f);
			identity.rotation.ApplyScaleLocal(scaleAmount);
			if (!string.IsNullOrEmpty(text))
			{
				clearBannerComponentCache = false;
				string text2 = "campaign_flag";
				if (_cachedBannerComponent.Item1 == text + text2)
				{
					_cachedBannerComponent.Item2.GetFirstMetaMesh().Frame = identity;
					StrategicEntity.AddComponent(_cachedBannerComponent.Item2);
				}
				else
				{
					MetaMesh bannerOfCharacter = GetBannerOfCharacter(new Banner(text), text2);
					bannerOfCharacter.Frame = identity;
					int componentCount = gameEntity.GetComponentCount(GameEntity.ComponentType.ClothSimulator);
					gameEntity.AddMultiMesh(bannerOfCharacter);
					if (gameEntity.GetComponentCount(GameEntity.ComponentType.ClothSimulator) > componentCount)
					{
						_cachedBannerComponent.Item1 = text + text2;
						_cachedBannerComponent.Item2 = gameEntity.GetComponentAtIndex(componentCount, GameEntity.ComponentType.ClothSimulator);
					}
				}
			}
			StrategicEntity.AddChild(gameEntity);
		}
		else
		{
			if (PartyBaseHelper.GetVisualPartyLeader(party) == null)
			{
				return;
			}
			string bannerKey = null;
			if (party.LeaderHero?.ClanBanner != null)
			{
				bannerKey = party.LeaderHero.ClanBanner.Serialize();
			}
			ActionIndexCache leaderAction = ActionIndexCache.act_none;
			ActionIndexCache mountAction = ActionIndexCache.act_none;
			MapEvent mapEvent = ((party.MobileParty.Army != null && party.MobileParty.Army.DoesLeaderPartyAndAttachedPartiesContain(party.MobileParty)) ? party.MobileParty.Army.LeaderParty.MapEvent : party.MapEvent);
			GetMeleeWeaponToWield(party, out var wieldedItemIndex);
			if (mapEvent != null && (mapEvent.EventType == MapEvent.BattleTypes.FieldBattle || (mapEvent.EventType == MapEvent.BattleTypes.Raid && party.MapEventSide == mapEvent.AttackerSide) || mapEvent.EventType == MapEvent.BattleTypes.SiegeOutside || mapEvent.EventType == MapEvent.BattleTypes.SallyOut))
			{
				GetPartyBattleAnimation(party, wieldedItemIndex, out leaderAction, out mountAction);
			}
			uint teamColor = (uint)(((int?)party.MapFaction?.Color) ?? (-3357781));
			uint teamColor2 = (uint)(((int?)party.MapFaction?.Color2) ?? (-3357781));
			AddCharacterToPartyIcon(PartyBaseHelper.GetVisualPartyLeader(party), contourColor, bannerKey, wieldedItemIndex, teamColor, teamColor2, leaderAction, mountAction, MBRandom.NondeterministicRandomFloat * 0.7f, ref clearBannerEntityCache);
			if (party.IsMobile)
			{
				party.MobileParty.GetMountAndHarnessVisualIdsForPartyIcon(out var mountStringId, out var harnessStringId);
				if (!string.IsNullOrEmpty(mountStringId))
				{
					AddMountToPartyIcon(new Vec3(0.3f, -0.25f), mountStringId, harnessStringId, contourColor, PartyBaseHelper.GetVisualPartyLeader(party));
				}
			}
		}
	}

	private void GetMeleeWeaponToWield(PartyBase party, out int wieldedItemIndex)
	{
		wieldedItemIndex = -1;
		CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(party);
		if (visualPartyLeader == null)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			if (visualPartyLeader.Equipment[i].Item != null && visualPartyLeader.Equipment[i].Item.PrimaryWeapon.IsMeleeWeapon)
			{
				wieldedItemIndex = i;
				break;
			}
		}
	}

	private static void GetPartyBattleAnimation(PartyBase party, int wieldedItemIndex, out ActionIndexCache leaderAction, out ActionIndexCache mountAction)
	{
		leaderAction = ActionIndexCache.act_none;
		mountAction = ActionIndexCache.act_none;
		if (party.MobileParty.Army == null || !party.MobileParty.Army.DoesLeaderPartyAndAttachedPartiesContain(party.MobileParty))
		{
			_ = party.MapEvent;
		}
		else
		{
			_ = party.MobileParty.Army.LeaderParty.MapEvent;
		}
		CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(party);
		if (party.MapEvent?.MapEventSettlement != null && visualPartyLeader != null && !visualPartyLeader.HasMount())
		{
			leaderAction = _raidOnFoot;
			return;
		}
		if (wieldedItemIndex > -1 && visualPartyLeader?.Equipment[wieldedItemIndex].Item != null)
		{
			WeaponComponent weaponComponent = visualPartyLeader.Equipment[wieldedItemIndex].Item.WeaponComponent;
			if (weaponComponent != null && weaponComponent.PrimaryWeapon.IsMeleeWeapon)
			{
				if (visualPartyLeader.HasMount())
				{
					if (visualPartyLeader.Equipment[10].Item.HorseComponent.Monster.MonsterUsage == "camel")
					{
						if (weaponComponent.GetItemType() == ItemObject.ItemTypeEnum.OneHandedWeapon || weaponComponent.GetItemType() == ItemObject.ItemTypeEnum.TwoHandedWeapon)
						{
							leaderAction = _camelSwordAttack;
							mountAction = _swordAttackMount;
						}
						else if (weaponComponent.GetItemType() == ItemObject.ItemTypeEnum.Polearm)
						{
							if (weaponComponent.PrimaryWeapon.SwingDamageType == DamageTypes.Invalid)
							{
								leaderAction = _camelSpearAttack;
								mountAction = _spearAttackMount;
							}
							else if (weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedPolearm)
							{
								leaderAction = _camel1HandedSwingAttack;
								mountAction = _swingAttackMount;
							}
							else
							{
								leaderAction = _camel2HandedSwingAttack;
								mountAction = _swingAttackMount;
							}
						}
					}
					else if (weaponComponent.GetItemType() == ItemObject.ItemTypeEnum.OneHandedWeapon || weaponComponent.GetItemType() == ItemObject.ItemTypeEnum.TwoHandedWeapon)
					{
						leaderAction = _horseSwordAttack;
						mountAction = _swordAttackMount;
					}
					else if (weaponComponent.GetItemType() == ItemObject.ItemTypeEnum.Polearm)
					{
						if (weaponComponent.PrimaryWeapon.SwingDamageType == DamageTypes.Invalid)
						{
							leaderAction = _horseSpearAttack;
							mountAction = _spearAttackMount;
						}
						else if (weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedPolearm)
						{
							leaderAction = _horse1HandedSwingAttack;
							mountAction = _swingAttackMount;
						}
						else
						{
							leaderAction = _horse2HandedSwingAttack;
							mountAction = _swingAttackMount;
						}
					}
				}
				else if (weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.OneHandedAxe || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.Mace || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.OneHandedSword)
				{
					leaderAction = _attack1H;
				}
				else if (weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedAxe || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedMace || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedSword)
				{
					leaderAction = _attack2H;
				}
				else if (weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.OneHandedPolearm || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedPolearm)
				{
					leaderAction = _attackSpear1HOr2H;
				}
			}
		}
		if (!(leaderAction == ActionIndexCache.act_none))
		{
			return;
		}
		if (visualPartyLeader.HasMount())
		{
			if (visualPartyLeader.Equipment[10].Item.HorseComponent.Monster.MonsterUsage == "camel")
			{
				leaderAction = _camelUnarmedAttack;
			}
			else
			{
				leaderAction = _horseUnarmedAttack;
			}
			mountAction = _unarmedAttackMount;
		}
		else
		{
			leaderAction = _attackUnarmed;
		}
	}

	public void RefreshWallState()
	{
		if (_defenderBreachableWallEntitiesForAllLevels == null)
		{
			return;
		}
		MBReadOnlyList<float> mBReadOnlyList = ((PartyBase?.Settlement != null && (PartyBase.Settlement == null || PartyBase.Settlement.IsFortification)) ? PartyBase.Settlement.SettlementWallSectionHitPointsRatioList : null);
		if (mBReadOnlyList == null)
		{
			return;
		}
		if (mBReadOnlyList.Count == 0)
		{
			Debug.FailedAssert("Town (" + PartyBase.Settlement.Name.ToString() + ") doesn't have wall entities defined for it's current level(" + PartyBase.Settlement.Town.GetWallLevel() + ")", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\PartyVisual.cs", "RefreshWallState", 1658);
			return;
		}
		for (int i = 0; i < _defenderBreachableWallEntitiesForAllLevels.Length; i++)
		{
			bool flag = mBReadOnlyList[i % mBReadOnlyList.Count] <= 0f;
			foreach (GameEntity child in _defenderBreachableWallEntitiesForAllLevels[i].GetChildren())
			{
				if (child.HasTag("map_solid_wall"))
				{
					child.SetVisibilityExcludeParents(!flag);
				}
				else if (child.HasTag("map_broken_wall"))
				{
					child.SetVisibilityExcludeParents(flag);
				}
			}
		}
	}

	public void RefreshTownPhysicalEntitiesState(PartyBase party)
	{
		if (party?.Settlement == null || !party.Settlement.IsFortification || TownPhysicalEntities == null)
		{
			return;
		}
		if (PlayerSiege.PlayerSiegeEvent != null && PlayerSiege.PlayerSiegeEvent.BesiegedSettlement == party.Settlement)
		{
			TownPhysicalEntities.ForEach(delegate(GameEntity p)
			{
				p.AddBodyFlags(BodyFlags.Disabled);
			});
		}
		else
		{
			TownPhysicalEntities.ForEach(delegate(GameEntity p)
			{
				p.RemoveBodyFlags(BodyFlags.Disabled);
			});
		}
	}

	public void SetLevelMask(uint newMask)
	{
		_currentLevelMask = newMask;
		PartyBase.SetVisualAsDirty();
	}

	public void RefreshLevelMask()
	{
		if (PartyBase.IsSettlement)
		{
			uint num = 0u;
			if (PartyBase.Settlement.IsVillage)
			{
				num = ((PartyBase.Settlement.Village.VillageState != Village.VillageStates.Looted) ? (num | Campaign.Current.MapSceneWrapper.GetSceneLevel("civilian")) : (num | Campaign.Current.MapSceneWrapper.GetSceneLevel("looted")));
				num |= GetLevelOfProduction(PartyBase.Settlement);
			}
			else if (PartyBase.Settlement.IsTown || PartyBase.Settlement.IsCastle)
			{
				if (PartyBase.Settlement.Town.GetWallLevel() == 1)
				{
					num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("level_1");
				}
				else if (PartyBase.Settlement.Town.GetWallLevel() == 2)
				{
					num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("level_2");
				}
				else if (PartyBase.Settlement.Town.GetWallLevel() == 3)
				{
					num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("level_3");
				}
				num = ((PartyBase.Settlement.SiegeEvent == null) ? (num | Campaign.Current.MapSceneWrapper.GetSceneLevel("civilian")) : (num | Campaign.Current.MapSceneWrapper.GetSceneLevel("siege")));
			}
			else if (PartyBase.Settlement.IsHideout)
			{
				num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("level_1");
			}
			if (_currentLevelMask != num)
			{
				SetLevelMask(num);
			}
		}
		PartyBase.OnLevelMaskUpdated();
	}

	private static uint GetLevelOfProduction(Settlement settlement)
	{
		uint num = 0u;
		if (settlement.IsVillage)
		{
			num = ((settlement.Village.Hearth < 200f) ? (num | Campaign.Current.MapSceneWrapper.GetSceneLevel("level_1")) : ((!(settlement.Village.Hearth < 600f)) ? (num | Campaign.Current.MapSceneWrapper.GetSceneLevel("level_3")) : (num | Campaign.Current.MapSceneWrapper.GetSceneLevel("level_2"))));
		}
		return num;
	}

	private void SetSettlementLevelVisibility()
	{
		List<GameEntity> children = new List<GameEntity>();
		StrategicEntity.GetChildrenRecursive(ref children);
		foreach (GameEntity item in children)
		{
			if (((uint)item.GetUpgradeLevelMask() & _currentLevelMask) == _currentLevelMask)
			{
				item.SetVisibilityExcludeParents(visible: true);
				item.SetPhysicsState(isEnabled: true, setChildren: true);
			}
			else
			{
				item.SetVisibilityExcludeParents(visible: false);
				item.SetPhysicsState(isEnabled: false, setChildren: true);
			}
		}
	}

	private void InitializePartyCollider(PartyBase party)
	{
		if (StrategicEntity != null && party.IsMobile)
		{
			StrategicEntity.AddSphereAsBody(new Vec3(0f, 0f, 0f, -1f), 0.5f, BodyFlags.Moveable | BodyFlags.OnlyCollideWithRaycast);
		}
	}

	public void OnPartyRemoved()
	{
		if (!(StrategicEntity != null))
		{
			return;
		}
		MapScreen.VisualsOfEntities.Remove(StrategicEntity.Pointer);
		foreach (GameEntity child in StrategicEntity.GetChildren())
		{
			MapScreen.VisualsOfEntities.Remove(child.Pointer);
		}
		ReleaseResources();
		StrategicEntity.Remove(111);
	}

	internal void OnMapHoverSiegeEngine(MatrixFrame engineFrame)
	{
		if (PlayerSiege.PlayerSiegeEvent == null)
		{
			return;
		}
		for (int i = 0; i < _attackerBatteringRamSpawnEntities.Length; i++)
		{
			MatrixFrame globalFrame = _attackerBatteringRamSpawnEntities[i].GetGlobalFrame();
			if (globalFrame.NearlyEquals(engineFrame))
			{
				if (_hoveredSiegeEntityFrame != globalFrame)
				{
					SiegeEvent.SiegeEngineConstructionProgress engineInProgress = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.DeployedMeleeSiegeEngines[i];
					InformationManager.ShowTooltip(typeof(List<TooltipProperty>), SandBoxUIHelper.GetSiegeEngineInProgressTooltip(engineInProgress));
				}
				return;
			}
		}
		for (int j = 0; j < _attackerSiegeTowerSpawnEntities.Length; j++)
		{
			MatrixFrame globalFrame2 = _attackerSiegeTowerSpawnEntities[j].GetGlobalFrame();
			if (globalFrame2.NearlyEquals(engineFrame))
			{
				if (_hoveredSiegeEntityFrame != globalFrame2)
				{
					SiegeEvent.SiegeEngineConstructionProgress engineInProgress2 = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.DeployedMeleeSiegeEngines[_attackerBatteringRamSpawnEntities.Length + j];
					InformationManager.ShowTooltip(typeof(List<TooltipProperty>), SandBoxUIHelper.GetSiegeEngineInProgressTooltip(engineInProgress2));
				}
				return;
			}
		}
		for (int k = 0; k < _attackerRangedEngineSpawnEntities.Length; k++)
		{
			MatrixFrame globalFrame3 = _attackerRangedEngineSpawnEntities[k].GetGlobalFrame();
			if (globalFrame3.NearlyEquals(engineFrame))
			{
				if (_hoveredSiegeEntityFrame != globalFrame3)
				{
					SiegeEvent.SiegeEngineConstructionProgress engineInProgress3 = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.DeployedRangedSiegeEngines[k];
					InformationManager.ShowTooltip(typeof(List<TooltipProperty>), SandBoxUIHelper.GetSiegeEngineInProgressTooltip(engineInProgress3));
				}
				return;
			}
		}
		for (int l = 0; l < _defenderRangedEngineSpawnEntitiesCacheForCurrentLevel.Length; l++)
		{
			MatrixFrame globalFrame4 = _defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[l].GetGlobalFrame();
			if (globalFrame4.NearlyEquals(engineFrame))
			{
				if (_hoveredSiegeEntityFrame != globalFrame4)
				{
					SiegeEvent.SiegeEngineConstructionProgress engineInProgress4 = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Defender).SiegeEngines.DeployedRangedSiegeEngines[l];
					InformationManager.ShowTooltip(typeof(List<TooltipProperty>), SandBoxUIHelper.GetSiegeEngineInProgressTooltip(engineInProgress4));
				}
				return;
			}
		}
		for (int m = 0; m < _defenderBreachableWallEntitiesCacheForCurrentLevel.Length; m++)
		{
			MatrixFrame globalFrame5 = _defenderBreachableWallEntitiesCacheForCurrentLevel[m].GetGlobalFrame();
			if (globalFrame5.NearlyEquals(engineFrame))
			{
				if (_hoveredSiegeEntityFrame != globalFrame5 && PartyBase.IsSettlement)
				{
					InformationManager.ShowTooltip(typeof(List<TooltipProperty>), SandBoxUIHelper.GetWallSectionTooltip(PartyBase.Settlement, m));
				}
				return;
			}
		}
		_hoveredSiegeEntityFrame = MatrixFrame.Identity;
	}

	internal void OnMapHoverSiegeEngineEnd()
	{
		_hoveredSiegeEntityFrame = MatrixFrame.Identity;
		MBInformationManager.HideInformations();
	}

	public void OnStartup()
	{
		bool flag = false;
		if (PartyBase.IsMobile)
		{
			StrategicEntity = GameEntity.CreateEmpty(MapScene);
			if (!PartyBase.IsVisible)
			{
				StrategicEntity.EntityFlags |= EntityFlags.DoNotTick;
			}
		}
		else if (PartyBase.IsSettlement)
		{
			StrategicEntity = MapScene.GetCampaignEntityWithName(PartyBase.Id);
			if (StrategicEntity == null)
			{
				Campaign.Current.MapSceneWrapper.AddNewEntityToMapScene(PartyBase.Settlement.StringId, PartyBase.Settlement.Position2D);
				StrategicEntity = MapScene.GetCampaignEntityWithName(PartyBase.Id);
			}
			bool flag2 = false;
			if (PartyBase.Settlement.IsFortification)
			{
				List<GameEntity> children = new List<GameEntity>();
				StrategicEntity.GetChildrenRecursive(ref children);
				PopulateSiegeEngineFrameListsFromChildren(children);
				UpdateDefenderSiegeEntitiesCache();
				TownPhysicalEntities = children.FindAll((GameEntity x) => x.HasTag("bo_town"));
				List<GameEntity> list = new List<GameEntity>();
				Dictionary<int, List<GameEntity>> dictionary = new Dictionary<int, List<GameEntity>>();
				dictionary.Add(1, new List<GameEntity>());
				dictionary.Add(2, new List<GameEntity>());
				dictionary.Add(3, new List<GameEntity>());
				List<MatrixFrame> list2 = new List<MatrixFrame>();
				List<MatrixFrame> list3 = new List<MatrixFrame>();
				foreach (GameEntity item in children)
				{
					if (item.HasTag("main_map_city_gate"))
					{
						PartyBase.IsPositionOkForTraveling(item.GetGlobalFrame().origin.AsVec2);
						flag2 = true;
						list.Add(item);
					}
					if (item.HasTag("map_settlement_circle"))
					{
						CircleLocalFrame = item.GetGlobalFrame();
						flag = true;
						item.SetVisibilityExcludeParents(visible: false);
						list.Add(item);
					}
					if (item.HasTag("map_banner_placeholder"))
					{
						int upgradeLevelOfEntity = item.Parent.GetUpgradeLevelOfEntity();
						if (upgradeLevelOfEntity == 0)
						{
							dictionary[1].Add(item);
							dictionary[2].Add(item);
							dictionary[3].Add(item);
						}
						else
						{
							dictionary[upgradeLevelOfEntity].Add(item);
						}
						list.Add(item);
					}
					if (item.HasTag("map_camp_area_1"))
					{
						list2.Add(item.GetGlobalFrame());
						list.Add(item);
					}
					else if (item.HasTag("map_camp_area_2"))
					{
						list3.Add(item.GetGlobalFrame());
						list.Add(item);
					}
				}
				_gateBannerEntitiesWithLevels = dictionary;
				if (PartyBase.Settlement.IsFortification)
				{
					PartyBase.Settlement.Town.BesiegerCampPositions1 = list2.ToArray();
					PartyBase.Settlement.Town.BesiegerCampPositions2 = list3.ToArray();
				}
				foreach (GameEntity item2 in list)
				{
					item2.Remove(112);
				}
			}
			if (!flag2)
			{
				if (!PartyBase.Settlement.IsTown)
				{
					_ = PartyBase.Settlement.IsCastle;
				}
				if (!PartyBase.IsPositionOkForTraveling(PartyBase.Settlement.GatePosition))
				{
					_ = PartyBase.Settlement.GatePosition;
				}
			}
		}
		CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(PartyBase);
		if (!flag)
		{
			CircleLocalFrame = MatrixFrame.Identity;
			if (PartyBase.IsSettlement)
			{
				MatrixFrame circleLocalFrame = CircleLocalFrame;
				Mat3 rotation = circleLocalFrame.rotation;
				if (PartyBase.Settlement.IsVillage)
				{
					rotation.ApplyScaleLocal(1.75f);
				}
				else if (PartyBase.Settlement.IsTown)
				{
					rotation.ApplyScaleLocal(5.75f);
				}
				else if (PartyBase.Settlement.IsCastle)
				{
					rotation.ApplyScaleLocal(2.75f);
				}
				else
				{
					rotation.ApplyScaleLocal(1.75f);
				}
				circleLocalFrame.rotation = rotation;
				CircleLocalFrame = circleLocalFrame;
			}
			else if ((visualPartyLeader != null && visualPartyLeader.HasMount()) || PartyBase.MobileParty.IsCaravan)
			{
				MatrixFrame circleLocalFrame2 = CircleLocalFrame;
				Mat3 rotation2 = circleLocalFrame2.rotation;
				rotation2.ApplyScaleLocal(0.4625f);
				circleLocalFrame2.rotation = rotation2;
				CircleLocalFrame = circleLocalFrame2;
			}
			else
			{
				MatrixFrame circleLocalFrame3 = CircleLocalFrame;
				Mat3 rotation3 = circleLocalFrame3.rotation;
				rotation3.ApplyScaleLocal(0.3725f);
				circleLocalFrame3.rotation = rotation3;
				CircleLocalFrame = circleLocalFrame3;
			}
		}
		StrategicEntity.SetVisibilityExcludeParents(PartyBase.IsVisible);
		AgentVisuals humanAgentVisuals = HumanAgentVisuals;
		if (humanAgentVisuals != null)
		{
			humanAgentVisuals.GetEntity()?.SetVisibilityExcludeParents(PartyBase.IsVisible);
		}
		AgentVisuals mountAgentVisuals = MountAgentVisuals;
		if (mountAgentVisuals != null)
		{
			mountAgentVisuals.GetEntity()?.SetVisibilityExcludeParents(PartyBase.IsVisible);
		}
		AgentVisuals caravanMountAgentVisuals = CaravanMountAgentVisuals;
		if (caravanMountAgentVisuals != null)
		{
			caravanMountAgentVisuals.GetEntity()?.SetVisibilityExcludeParents(PartyBase.IsVisible);
		}
		StrategicEntity.SetReadyToRender(ready: true);
		StrategicEntity.SetEntityEnvMapVisibility(value: false);
		_entityAlpha = (PartyBase.IsVisible ? 1f : 0f);
		InitializePartyCollider(PartyBase);
		List<GameEntity> children2 = new List<GameEntity>();
		StrategicEntity.GetChildrenRecursive(ref children2);
		if (!MapScreen.VisualsOfEntities.ContainsKey(StrategicEntity.Pointer))
		{
			MapScreen.VisualsOfEntities.Add(StrategicEntity.Pointer, this);
		}
		foreach (GameEntity item3 in children2)
		{
			if (!MapScreen.VisualsOfEntities.ContainsKey(item3.Pointer) && !MapScreen.FrameAndVisualOfEngines.ContainsKey(item3.Pointer))
			{
				MapScreen.VisualsOfEntities.Add(item3.Pointer, this);
			}
		}
		if (PartyBase.IsSettlement)
		{
			StrategicEntity.SetAsPredisplayEntity();
		}
	}

	private void PopulateSiegeEngineFrameListsFromChildren(List<GameEntity> children)
	{
		_attackerRangedEngineSpawnEntities = (from e in children.FindAll((GameEntity x) => x.Tags.Any((string t) => t.Contains("map_siege_engine")))
			orderby e.Tags.First((string s) => s.Contains("map_siege_engine"))
			select e).ToArray();
		GameEntity[] attackerRangedEngineSpawnEntities = _attackerRangedEngineSpawnEntities;
		foreach (GameEntity gameEntity in attackerRangedEngineSpawnEntities)
		{
			if (gameEntity.ChildCount > 0 && !MapScreen.FrameAndVisualOfEngines.ContainsKey(gameEntity.GetChild(0).Pointer))
			{
				MapScreen.FrameAndVisualOfEngines.Add(gameEntity.GetChild(0).Pointer, new Tuple<MatrixFrame, PartyVisual>(gameEntity.GetGlobalFrame(), this));
			}
		}
		_defenderRangedEngineSpawnEntitiesForAllLevels = (from e in children.FindAll((GameEntity x) => x.Tags.Any((string t) => t.Contains("map_defensive_engine")))
			orderby e.Tags.First((string s) => s.Contains("map_defensive_engine"))
			select e).ToArray();
		attackerRangedEngineSpawnEntities = _defenderRangedEngineSpawnEntitiesForAllLevels;
		foreach (GameEntity gameEntity2 in attackerRangedEngineSpawnEntities)
		{
			if (gameEntity2.ChildCount > 0 && !MapScreen.FrameAndVisualOfEngines.ContainsKey(gameEntity2.GetChild(0).Pointer))
			{
				MapScreen.FrameAndVisualOfEngines.Add(gameEntity2.GetChild(0).Pointer, new Tuple<MatrixFrame, PartyVisual>(gameEntity2.GetGlobalFrame(), this));
			}
		}
		_attackerBatteringRamSpawnEntities = children.FindAll((GameEntity x) => x.HasTag("map_siege_ram")).ToArray();
		attackerRangedEngineSpawnEntities = _attackerBatteringRamSpawnEntities;
		foreach (GameEntity gameEntity3 in attackerRangedEngineSpawnEntities)
		{
			if (gameEntity3.ChildCount > 0 && !MapScreen.FrameAndVisualOfEngines.ContainsKey(gameEntity3.GetChild(0).Pointer))
			{
				MapScreen.FrameAndVisualOfEngines.Add(gameEntity3.GetChild(0).Pointer, new Tuple<MatrixFrame, PartyVisual>(gameEntity3.GetGlobalFrame(), this));
			}
		}
		_attackerSiegeTowerSpawnEntities = children.FindAll((GameEntity x) => x.HasTag("map_siege_tower")).ToArray();
		attackerRangedEngineSpawnEntities = _attackerSiegeTowerSpawnEntities;
		foreach (GameEntity gameEntity4 in attackerRangedEngineSpawnEntities)
		{
			if (gameEntity4.ChildCount > 0 && !MapScreen.FrameAndVisualOfEngines.ContainsKey(gameEntity4.GetChild(0).Pointer))
			{
				MapScreen.FrameAndVisualOfEngines.Add(gameEntity4.GetChild(0).Pointer, new Tuple<MatrixFrame, PartyVisual>(gameEntity4.GetGlobalFrame(), this));
			}
		}
		_defenderBreachableWallEntitiesForAllLevels = children.FindAll((GameEntity x) => x.HasTag("map_breachable_wall")).ToArray();
		attackerRangedEngineSpawnEntities = _defenderBreachableWallEntitiesForAllLevels;
		foreach (GameEntity gameEntity5 in attackerRangedEngineSpawnEntities)
		{
			if (gameEntity5.ChildCount > 0 && !MapScreen.FrameAndVisualOfEngines.ContainsKey(gameEntity5.GetChild(0).Pointer))
			{
				MapScreen.FrameAndVisualOfEngines.Add(gameEntity5.GetChild(0).Pointer, new Tuple<MatrixFrame, PartyVisual>(gameEntity5.GetGlobalFrame(), this));
			}
		}
	}

	private MatrixFrame GetFrame()
	{
		return StrategicEntity.GetFrame();
	}

	private void SetFrame(ref MatrixFrame frame)
	{
		if (StrategicEntity != null && !StrategicEntity.GetFrame().NearlyEquals(frame))
		{
			StrategicEntity.SetFrame(ref frame);
			if (HumanAgentVisuals != null)
			{
				MatrixFrame frame2 = frame;
				frame2.rotation.ApplyScaleLocal(HumanAgentVisuals.GetScale());
				HumanAgentVisuals.GetEntity().SetFrame(ref frame2);
			}
			if (MountAgentVisuals != null)
			{
				MatrixFrame frame3 = frame;
				frame3.rotation.ApplyScaleLocal(MountAgentVisuals.GetScale());
				MountAgentVisuals.GetEntity().SetFrame(ref frame3);
			}
			if (CaravanMountAgentVisuals != null)
			{
				MatrixFrame frame4 = frame.TransformToParent(CaravanMountAgentVisuals.GetFrame());
				frame4.rotation.ApplyScaleLocal(CaravanMountAgentVisuals.GetScale());
				CaravanMountAgentVisuals.GetEntity().SetFrame(ref frame4);
			}
		}
	}

	private void UpdateDefenderSiegeEntitiesCache()
	{
		GameEntity.UpgradeLevelMask currentSettlementUpgradeLevelMask = GameEntity.UpgradeLevelMask.None;
		if (PartyBase.IsSettlement && PartyBase.Settlement.IsFortification)
		{
			if (PartyBase.Settlement.Town.GetWallLevel() == 1)
			{
				currentSettlementUpgradeLevelMask = GameEntity.UpgradeLevelMask.Level1;
			}
			else if (PartyBase.Settlement.Town.GetWallLevel() == 2)
			{
				currentSettlementUpgradeLevelMask = GameEntity.UpgradeLevelMask.Level2;
			}
			else if (PartyBase.Settlement.Town.GetWallLevel() == 3)
			{
				currentSettlementUpgradeLevelMask = GameEntity.UpgradeLevelMask.Level3;
			}
		}
		_currentSettlementUpgradeLevelMask = currentSettlementUpgradeLevelMask;
		_defenderRangedEngineSpawnEntitiesCacheForCurrentLevel = _defenderRangedEngineSpawnEntitiesForAllLevels.Where((GameEntity e) => (e.GetUpgradeLevelMask() & _currentSettlementUpgradeLevelMask) == _currentSettlementUpgradeLevelMask).ToArray();
		_defenderBreachableWallEntitiesCacheForCurrentLevel = _defenderBreachableWallEntitiesForAllLevels.Where((GameEntity e) => (e.GetUpgradeLevelMask() & _currentSettlementUpgradeLevelMask) == _currentSettlementUpgradeLevelMask).ToArray();
	}

	public MatrixFrame[] GetAttackerTowerSiegeEngineFrames()
	{
		MatrixFrame[] array = new MatrixFrame[_attackerSiegeTowerSpawnEntities.Length];
		for (int i = 0; i < _attackerSiegeTowerSpawnEntities.Length; i++)
		{
			array[i] = _attackerSiegeTowerSpawnEntities[i].GetGlobalFrame();
		}
		return array;
	}

	public MatrixFrame[] GetAttackerBatteringRamSiegeEngineFrames()
	{
		MatrixFrame[] array = new MatrixFrame[_attackerBatteringRamSpawnEntities.Length];
		for (int i = 0; i < _attackerBatteringRamSpawnEntities.Length; i++)
		{
			array[i] = _attackerBatteringRamSpawnEntities[i].GetGlobalFrame();
		}
		return array;
	}

	public MatrixFrame[] GetAttackerRangedSiegeEngineFrames()
	{
		MatrixFrame[] array = new MatrixFrame[_attackerRangedEngineSpawnEntities.Length];
		for (int i = 0; i < _attackerRangedEngineSpawnEntities.Length; i++)
		{
			array[i] = _attackerRangedEngineSpawnEntities[i].GetGlobalFrame();
		}
		return array;
	}

	public MatrixFrame[] GetDefenderRangedSiegeEngineFrames()
	{
		MatrixFrame[] array = new MatrixFrame[_defenderRangedEngineSpawnEntitiesCacheForCurrentLevel.Length];
		for (int i = 0; i < _defenderRangedEngineSpawnEntitiesCacheForCurrentLevel.Length; i++)
		{
			array[i] = _defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[i].GetGlobalFrame();
		}
		return array;
	}

	public MatrixFrame[] GetBreachableWallFrames()
	{
		MatrixFrame[] array = new MatrixFrame[_defenderBreachableWallEntitiesCacheForCurrentLevel.Length];
		for (int i = 0; i < _defenderBreachableWallEntitiesCacheForCurrentLevel.Length; i++)
		{
			array[i] = _defenderBreachableWallEntitiesCacheForCurrentLevel[i].GetGlobalFrame();
		}
		return array;
	}

	public bool IsVisibleOrFadingOut()
	{
		return _entityAlpha > 0f;
	}
}
