using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.View.Map;

public class MapCameraView : MapView
{
	public enum CameraFollowMode
	{
		Free,
		FollowParty,
		MoveToPosition
	}

	public struct InputInformation
	{
		public bool IsMainPartyValid;

		public bool IsMapReady;

		public bool IsControlDown;

		public bool IsMouseActive;

		public bool CheatModeEnabled;

		public bool LeftMouseButtonPressed;

		public bool LeftMouseButtonDown;

		public bool LeftMouseButtonReleased;

		public bool MiddleMouseButtonDown;

		public bool RightMouseButtonDown;

		public bool RotateLeftKeyDown;

		public bool RotateRightKeyDown;

		public bool PartyMoveUpKey;

		public bool PartyMoveDownKey;

		public bool PartyMoveLeftKey;

		public bool PartyMoveRightKey;

		public bool CameraFollowModeKeyPressed;

		public bool LeftButtonDraggingMode;

		public bool IsInMenu;

		public bool RayCastForClosestEntityOrTerrainCondition;

		public float MapZoomIn;

		public float MapZoomOut;

		public float DeltaMouseScroll;

		public float MouseSensitivity;

		public float MouseMoveX;

		public float MouseMoveY;

		public float HorizontalCameraInput;

		public float RX;

		public float RY;

		public float RS;

		public float Dt;

		public Vec2 MousePositionPixel;

		public Vec2 ClickedPositionPixel;

		public Vec3 ClickedPosition;

		public Vec3 ProjectedPosition;

		public Vec3 WorldMouseNear;

		public Vec3 WorldMouseFar;
	}

	private const float VerticalHalfViewAngle = 0.34906584f;

	private Vec3 _cameraTarget;

	private bool _doFastCameraMovementToTarget;

	private float _cameraElevation;

	private Vec2 _lastUsedIdealCameraTarget;

	private Vec2 _cameraAnimationTarget;

	private float _cameraAnimationStopDuration;

	private readonly Scene _mapScene;

	protected float _customMaximumCameraHeight;

	private MatrixFrame _cameraFrame;

	protected virtual CameraFollowMode CurrentCameraFollowMode { get; set; }

	public virtual float CameraFastMoveMultiplier { get; protected set; }

	protected virtual float CameraBearing { get; set; }

	protected virtual float MaximumCameraHeight => Math.Max(_customMaximumCameraHeight, Campaign.MapMaximumHeight);

	protected virtual float CameraBearingVelocity { get; set; }

	public virtual float CameraDistance { get; protected set; }

	protected virtual float TargetCameraDistance { get; set; }

	protected virtual float AdditionalElevation { get; set; }

	public virtual bool CameraAnimationInProgress { get; protected set; }

	public virtual bool ProcessCameraInput { get; protected set; }

	public virtual Camera Camera { get; protected set; }

	public virtual MatrixFrame CameraFrame
	{
		get
		{
			return _cameraFrame;
		}
		protected set
		{
			_cameraFrame = value;
		}
	}

	protected virtual Vec3 IdealCameraTarget { get; set; }

	private static MapCameraView Instance { get; set; }

	public MapCameraView()
	{
		Camera = Camera.CreateCamera();
		Camera.SetViewVolume(perspective: true, -0.1f, 0.1f, -0.07f, 0.07f, 0.2f, 300f);
		Camera.Position = new Vec3(0f, 0f, 10f);
		CameraBearing = 0f;
		_cameraElevation = 1f;
		CameraDistance = 2.5f;
		ProcessCameraInput = true;
		CameraFastMoveMultiplier = 4f;
		_cameraFrame = MatrixFrame.Identity;
		CurrentCameraFollowMode = CameraFollowMode.FollowParty;
		_mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
		Instance = this;
	}

	public virtual void OnActivate(bool leftButtonDraggingMode, Vec3 clickedPosition)
	{
		SetCameraMode(CameraFollowMode.FollowParty);
		CameraBearingVelocity = 0f;
		UpdateMapCamera(leftButtonDraggingMode, clickedPosition);
	}

	public virtual void Initialize()
	{
		if (MobileParty.MainParty != null && PartyBase.MainParty.IsValid)
		{
			float height = 0f;
			_mapScene.GetHeightAtPoint(MobileParty.MainParty.Position2D, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref height);
			IdealCameraTarget = new Vec3(MobileParty.MainParty.Position2D, height + 1f);
		}
		_cameraTarget = IdealCameraTarget;
	}

	protected internal override void OnFinalize()
	{
		base.OnFinalize();
		Instance = null;
	}

	public virtual void SetCameraMode(CameraFollowMode cameraMode)
	{
		CurrentCameraFollowMode = cameraMode;
	}

	public virtual void ResetCamera(bool resetDistance, bool teleportToMainParty)
	{
		if (teleportToMainParty)
		{
			TeleportCameraToMainParty();
		}
		if (resetDistance)
		{
			TargetCameraDistance = 15f;
			CameraDistance = 15f;
		}
		CameraBearing = 0f;
		_cameraElevation = 1f;
	}

	public virtual void TeleportCameraToMainParty()
	{
		CurrentCameraFollowMode = CameraFollowMode.FollowParty;
		Campaign.Current.CameraFollowParty = MobileParty.MainParty.Party;
		IdealCameraTarget = GetCameraTargetForParty(Campaign.Current.CameraFollowParty);
		_lastUsedIdealCameraTarget = IdealCameraTarget.AsVec2;
		_cameraTarget = IdealCameraTarget;
	}

	public virtual void FastMoveCameraToMainParty()
	{
		CurrentCameraFollowMode = CameraFollowMode.FollowParty;
		Campaign.Current.CameraFollowParty = MobileParty.MainParty.Party;
		IdealCameraTarget = GetCameraTargetForParty(Campaign.Current.CameraFollowParty);
		_doFastCameraMovementToTarget = true;
		TargetCameraDistance = 15f;
	}

	public virtual void FastMoveCameraToPosition(Vec2 target, bool isInMenu)
	{
		if (!isInMenu)
		{
			CurrentCameraFollowMode = CameraFollowMode.MoveToPosition;
			IdealCameraTarget = GetCameraTargetForPosition(target);
			_doFastCameraMovementToTarget = true;
			TargetCameraDistance = 15f;
		}
	}

	public virtual bool IsCameraLockedToPlayerParty()
	{
		if (CurrentCameraFollowMode == CameraFollowMode.FollowParty)
		{
			return Campaign.Current.CameraFollowParty == MobileParty.MainParty.Party;
		}
		return false;
	}

	public virtual void StartCameraAnimation(Vec2 targetPosition, float animationStopDuration)
	{
		CameraAnimationInProgress = true;
		_cameraAnimationTarget = targetPosition;
		_cameraAnimationStopDuration = animationStopDuration;
		Campaign.Current.SetTimeSpeed(0);
		Campaign.Current.SetTimeControlModeLock(isLocked: true);
	}

	public virtual void SiegeEngineClick(MatrixFrame siegeEngineFrame)
	{
		if (TargetCameraDistance > 18f)
		{
			TargetCameraDistance = 18f;
		}
	}

	public virtual void OnExit()
	{
		ProcessCameraInput = true;
	}

	public virtual void OnEscapeMenuToggled(bool isOpened)
	{
		ProcessCameraInput = !isOpened;
	}

	public virtual void HandleMouse(bool rightMouseButtonPressed, float verticalCameraInput, float mouseMoveY, float dt)
	{
		float num = 0.3f / 700f;
		float num2 = (0f - (700f - TaleWorlds.Library.MathF.Min(700f, TaleWorlds.Library.MathF.Max(50f, CameraDistance)))) * num;
		float maxValue = TaleWorlds.Library.MathF.Max(num2 + 1E-05f, (float)Math.PI * 99f / 200f - CalculateCameraElevation(CameraDistance));
		if (rightMouseButtonPressed)
		{
			AdditionalElevation = MBMath.ClampFloat(AdditionalElevation + mouseMoveY * 0.0015f, num2, maxValue);
		}
		if (verticalCameraInput != 0f)
		{
			AdditionalElevation = MBMath.ClampFloat(AdditionalElevation - verticalCameraInput * dt, num2, maxValue);
		}
	}

	public virtual void HandleLeftMouseButtonClick(bool isMouseActive)
	{
		if (isMouseActive)
		{
			CurrentCameraFollowMode = CameraFollowMode.FollowParty;
			Campaign.Current.CameraFollowParty = PartyBase.MainParty;
		}
	}

	public virtual void OnSetMapSiegeOverlayState(bool isActive, bool isMapSiegeOverlayViewNull)
	{
		if (isActive && isMapSiegeOverlayViewNull && PlayerSiege.PlayerSiegeEvent != null)
		{
			TargetCameraDistance = 13f;
		}
	}

	public virtual void OnRefreshMapSiegeOverlayRequired(bool isMapSiegeOverlayViewNull)
	{
		if (PlayerSiege.PlayerSiegeEvent != null && isMapSiegeOverlayViewNull)
		{
			TargetCameraDistance = 13f;
		}
	}

	public virtual void OnBeforeTick(in InputInformation inputInformation)
	{
		float num = TaleWorlds.Library.MathF.Min(1f, TaleWorlds.Library.MathF.Max(0f, 1f - CameraFrame.rotation.f.z)) + 0.15f;
		_mapScene.SetDepthOfFieldParameters(0.05f, num * 1000f, isVignetteOn: true);
		_mapScene.SetDepthOfFieldFocus(0.05f);
		MobileParty mainParty = MobileParty.MainParty;
		if (inputInformation.IsMainPartyValid && CameraAnimationInProgress)
		{
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			if (_cameraAnimationStopDuration > 0f)
			{
				if (_cameraAnimationTarget.DistanceSquared(_cameraTarget.AsVec2) < 0.0001f)
				{
					_cameraAnimationStopDuration = TaleWorlds.Library.MathF.Max(_cameraAnimationStopDuration - inputInformation.Dt, 0f);
				}
				else
				{
					float terrainHeight = _mapScene.GetTerrainHeight(_cameraAnimationTarget);
					IdealCameraTarget = _cameraAnimationTarget.ToVec3(terrainHeight + 1f);
				}
			}
			else if (MobileParty.MainParty.Position2D.DistanceSquared(_cameraTarget.AsVec2) < 0.0001f)
			{
				CameraAnimationInProgress = false;
				Campaign.Current.SetTimeControlModeLock(isLocked: false);
			}
			else
			{
				IdealCameraTarget = MobileParty.MainParty.GetPosition() + Vec3.Up;
			}
		}
		bool flag = CameraAnimationInProgress;
		if (ProcessCameraInput && !CameraAnimationInProgress && inputInformation.IsMapReady)
		{
			flag = GetMapCameraInput(inputInformation);
		}
		if (flag)
		{
			Vec3 vec = IdealCameraTarget - _cameraTarget;
			Vec3 vec2 = 10f * vec * inputInformation.Dt;
			float num2 = TaleWorlds.Library.MathF.Sqrt(TaleWorlds.Library.MathF.Max(CameraDistance, 20f)) * 0.15f;
			float num3 = (_doFastCameraMovementToTarget ? (num2 * 5f) : num2);
			if (vec2.LengthSquared > num3 * num3)
			{
				vec2 = vec2.NormalizedCopy() * num3;
			}
			if (vec2.LengthSquared < num2 * num2)
			{
				_doFastCameraMovementToTarget = false;
			}
			_cameraTarget += vec2;
		}
		else
		{
			_cameraTarget = IdealCameraTarget;
			_doFastCameraMovementToTarget = false;
		}
		if (inputInformation.IsMainPartyValid)
		{
			if (inputInformation.CameraFollowModeKeyPressed)
			{
				CurrentCameraFollowMode = CameraFollowMode.FollowParty;
			}
			if (!inputInformation.IsInMenu && !inputInformation.MiddleMouseButtonDown && (MobileParty.MainParty == null || MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty) && (inputInformation.PartyMoveRightKey || inputInformation.PartyMoveLeftKey || inputInformation.PartyMoveUpKey || inputInformation.PartyMoveDownKey))
			{
				float num4 = 0f;
				float num5 = 0f;
				TaleWorlds.Library.MathF.SinCos(CameraBearing, out var sa, out var ca);
				TaleWorlds.Library.MathF.SinCos(CameraBearing + (float)Math.PI / 2f, out var sa2, out var ca2);
				float num6 = 0.5f;
				if (inputInformation.PartyMoveUpKey)
				{
					num5 += ca * num6;
					num4 += sa * num6;
					mainParty.Ai.ForceAiNoPathMode = true;
				}
				if (inputInformation.PartyMoveDownKey)
				{
					num5 -= ca * num6;
					num4 -= sa * num6;
					mainParty.Ai.ForceAiNoPathMode = true;
				}
				if (inputInformation.PartyMoveLeftKey)
				{
					num5 -= ca2 * num6;
					num4 -= sa2 * num6;
					mainParty.Ai.ForceAiNoPathMode = true;
				}
				if (inputInformation.PartyMoveRightKey)
				{
					num5 += ca2 * num6;
					num4 += sa2 * num6;
					mainParty.Ai.ForceAiNoPathMode = true;
				}
				CurrentCameraFollowMode = CameraFollowMode.FollowParty;
				mainParty.Ai.SetMoveGoToPoint(mainParty.Position2D + new Vec2(num4, num5));
				Campaign.Current.TimeControlMode = CampaignTimeControlMode.StoppablePlay;
			}
			else if (mainParty.Ai.ForceAiNoPathMode)
			{
				mainParty.Ai.SetMoveGoToPoint(mainParty.Position2D);
			}
		}
		UpdateMapCamera(inputInformation.LeftButtonDraggingMode, inputInformation.ClickedPosition);
	}

	protected virtual void UpdateMapCamera(bool _leftButtonDraggingMode, Vec3 _clickedPosition)
	{
		_lastUsedIdealCameraTarget = IdealCameraTarget.AsVec2;
		MatrixFrame cameraFrame = ComputeMapCamera(ref _cameraTarget, CameraBearing, _cameraElevation, CameraDistance, ref _lastUsedIdealCameraTarget);
		bool flag = !cameraFrame.origin.NearlyEquals(_cameraFrame.origin);
		bool flag2 = !cameraFrame.rotation.NearlyEquals(_cameraFrame.rotation);
		if (flag2 || flag)
		{
			Game.Current.EventManager.TriggerEvent(new MainMapCameraMoveEvent(flag2, flag));
		}
		_cameraFrame = cameraFrame;
		float height = 0f;
		_mapScene.GetHeightAtPoint(_cameraFrame.origin.AsVec2, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref height);
		height += 0.5f;
		if (_cameraFrame.origin.z < height)
		{
			if (_leftButtonDraggingMode)
			{
				Vec3 vec = _clickedPosition;
				vec -= Vec3.DotProduct(vec - _cameraFrame.origin, _cameraFrame.rotation.s) * _cameraFrame.rotation.s;
				Vec3 vec2 = Vec3.CrossProduct((vec - _cameraFrame.origin).NormalizedCopy(), (vec - (_cameraFrame.origin + new Vec3(0f, 0f, height - _cameraFrame.origin.z))).NormalizedCopy());
				float a = vec2.Normalize();
				_cameraFrame.origin.z = height;
				_cameraFrame.rotation.u = _cameraFrame.rotation.u.RotateAboutAnArbitraryVector(vec2, a);
				_cameraFrame.rotation.f = Vec3.CrossProduct(_cameraFrame.rotation.u, _cameraFrame.rotation.s).NormalizedCopy();
				_cameraFrame.rotation.s = Vec3.CrossProduct(_cameraFrame.rotation.f, _cameraFrame.rotation.u);
				Vec3 planeNormal = -Vec3.Up;
				Vec3 rayDirection = -_cameraFrame.rotation.u;
				Vec3 planeCenter = IdealCameraTarget;
				if (MBMath.GetRayPlaneIntersectionPoint(in planeNormal, in planeCenter, in _cameraFrame.origin, in rayDirection, out var t))
				{
					IdealCameraTarget = _cameraFrame.origin + rayDirection * t;
					_cameraTarget = IdealCameraTarget;
				}
				_cameraElevation = 0f - new Vec2(_cameraFrame.rotation.f.AsVec2.Length, _cameraFrame.rotation.f.z).RotationInRadians;
				CameraDistance = (_cameraFrame.origin - IdealCameraTarget).Length - 2f;
				TargetCameraDistance = CameraDistance;
				AdditionalElevation = _cameraElevation - CalculateCameraElevation(CameraDistance);
				_lastUsedIdealCameraTarget = IdealCameraTarget.AsVec2;
				ComputeMapCamera(ref _cameraTarget, CameraBearing, _cameraElevation, CameraDistance, ref _lastUsedIdealCameraTarget);
			}
			else
			{
				float num = 0.47123894f;
				int num2 = 0;
				do
				{
					_cameraElevation += ((_cameraFrame.origin.z < height) ? num : (0f - num));
					AdditionalElevation = _cameraElevation - CalculateCameraElevation(CameraDistance);
					float num3 = 700f;
					float num4 = 0.3f / num3;
					float a2 = 50f;
					float num5 = (0f - (num3 - TaleWorlds.Library.MathF.Min(num3, TaleWorlds.Library.MathF.Max(a2, CameraDistance)))) * num4;
					float maxValue = TaleWorlds.Library.MathF.Max(num5 + 1E-05f, (float)Math.PI * 99f / 200f - CalculateCameraElevation(CameraDistance));
					AdditionalElevation = MBMath.ClampFloat(AdditionalElevation, num5, maxValue);
					_cameraElevation = AdditionalElevation + CalculateCameraElevation(CameraDistance);
					Vec2 lastUsedIdealCameraTarget = Vec2.Zero;
					_cameraFrame = ComputeMapCamera(ref _cameraTarget, CameraBearing, _cameraElevation, CameraDistance, ref lastUsedIdealCameraTarget);
					_mapScene.GetHeightAtPoint(_cameraFrame.origin.AsVec2, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref height);
					height += 0.5f;
					if (num > 0.0001f)
					{
						num *= 0.5f;
					}
					else
					{
						num2++;
					}
				}
				while (num > 0.0001f || (_cameraFrame.origin.z < height && num2 < 5));
				if (_cameraFrame.origin.z < height)
				{
					_cameraFrame.origin.z = height;
					Vec3 planeNormal2 = -Vec3.Up;
					Vec3 rayDirection2 = -_cameraFrame.rotation.u;
					Vec3 planeCenter2 = IdealCameraTarget;
					if (MBMath.GetRayPlaneIntersectionPoint(in planeNormal2, in planeCenter2, in _cameraFrame.origin, in rayDirection2, out var t2))
					{
						IdealCameraTarget = _cameraFrame.origin + rayDirection2 * t2;
						_cameraTarget = IdealCameraTarget;
					}
					CameraDistance = (_cameraFrame.origin - IdealCameraTarget).Length - 2f;
					_lastUsedIdealCameraTarget = IdealCameraTarget.AsVec2;
					ComputeMapCamera(ref _cameraTarget, CameraBearing, _cameraElevation, CameraDistance, ref _lastUsedIdealCameraTarget);
					TargetCameraDistance = TaleWorlds.Library.MathF.Max(TargetCameraDistance, CameraDistance);
				}
			}
		}
		Camera.Frame = _cameraFrame;
		Camera.SetFovVertical(0.6981317f, Screen.AspectRatio, 0.01f, MaximumCameraHeight * 4f);
		_mapScene.SetDepthOfFieldFocus(0f);
		_mapScene.SetDepthOfFieldParameters(0f, 0f, isVignetteOn: false);
		MatrixFrame identity = MatrixFrame.Identity;
		identity.rotation = _cameraFrame.rotation;
		identity.origin = _cameraTarget;
		_mapScene.GetHeightAtPoint(identity.origin.AsVec2, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref identity.origin.z);
		identity.origin = MBMath.Lerp(identity.origin, _cameraFrame.origin, 0.075f, 1E-05f);
		PathFaceRecord faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(identity.origin.AsVec2);
		if (faceIndex.IsValid())
		{
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(faceIndex);
			MBMapScene.TickAmbientSounds(_mapScene, (int)faceTerrainType);
		}
		SoundManager.SetListenerFrame(identity);
	}

	protected virtual Vec3 GetCameraTargetForPosition(Vec2 targetPosition)
	{
		float terrainHeight = _mapScene.GetTerrainHeight(targetPosition);
		return new Vec3(targetPosition, terrainHeight + 1f);
	}

	protected virtual Vec3 GetCameraTargetForParty(PartyBase party)
	{
		Vec2 targetPosition;
		if (party.IsMobile && party.MobileParty.CurrentSettlement != null)
		{
			targetPosition = party.MobileParty.CurrentSettlement.Position2D;
		}
		else if (party.IsMobile && party.MobileParty.BesiegedSettlement != null)
		{
			if (PlayerSiege.PlayerSiegeEvent != null)
			{
				Vec2 asVec = party.MobileParty.BesiegedSettlement.Town.BesiegerCampPositions1.First().origin.AsVec2;
				targetPosition = Vec2.Lerp(party.MobileParty.BesiegedSettlement.GatePosition, asVec, 0.75f);
			}
			else
			{
				targetPosition = party.MobileParty.BesiegedSettlement.GatePosition;
			}
		}
		else
		{
			targetPosition = party.Position2D;
		}
		return GetCameraTargetForPosition(targetPosition);
	}

	protected virtual bool GetMapCameraInput(InputInformation inputInformation)
	{
		bool flag = false;
		bool result = !inputInformation.LeftButtonDraggingMode;
		if (inputInformation.IsControlDown && inputInformation.CheatModeEnabled)
		{
			flag = true;
			if (inputInformation.DeltaMouseScroll > 0.01f)
			{
				CameraFastMoveMultiplier *= 1.25f;
			}
			else if (inputInformation.DeltaMouseScroll < -0.01f)
			{
				CameraFastMoveMultiplier *= 0.8f;
			}
			CameraFastMoveMultiplier = MBMath.ClampFloat(CameraFastMoveMultiplier, 1f, 37.252903f);
		}
		Vec2 vec = Vec2.Zero;
		if (!inputInformation.LeftMouseButtonPressed && inputInformation.LeftMouseButtonDown && !inputInformation.LeftMouseButtonReleased && inputInformation.MousePositionPixel.DistanceSquared(inputInformation.ClickedPositionPixel) > 300f && !inputInformation.IsInMenu)
		{
			if (!inputInformation.LeftButtonDraggingMode)
			{
				IdealCameraTarget = _cameraTarget;
				_lastUsedIdealCameraTarget = IdealCameraTarget.AsVec2;
			}
			Vec3 rayDirection = (inputInformation.WorldMouseFar - inputInformation.WorldMouseNear).NormalizedCopy();
			Vec3 planeNormal = -Vec3.Up;
			if (MBMath.GetRayPlaneIntersectionPoint(in planeNormal, in inputInformation.ClickedPosition, in inputInformation.WorldMouseNear, in rayDirection, out var t))
			{
				CurrentCameraFollowMode = CameraFollowMode.Free;
				Vec3 vec2 = inputInformation.WorldMouseNear + rayDirection * t;
				vec = inputInformation.ClickedPosition.AsVec2 - vec2.AsVec2;
			}
		}
		if (inputInformation.MiddleMouseButtonDown)
		{
			TargetCameraDistance += 0.01f * (CameraDistance + 20f) * inputInformation.MouseSensitivity * inputInformation.MouseMoveY;
		}
		if (inputInformation.RotateLeftKeyDown)
		{
			CameraBearingVelocity = inputInformation.Dt * 2f;
		}
		else if (inputInformation.RotateRightKeyDown)
		{
			CameraBearingVelocity = inputInformation.Dt * -2f;
		}
		CameraBearingVelocity += inputInformation.HorizontalCameraInput * 1.75f * inputInformation.Dt;
		if (inputInformation.RightMouseButtonDown)
		{
			CameraBearingVelocity += 0.01f * inputInformation.MouseSensitivity * inputInformation.MouseMoveX;
		}
		float num = 0.1f;
		if (!inputInformation.IsMouseActive)
		{
			num *= inputInformation.Dt * 10f;
		}
		if (!flag)
		{
			TargetCameraDistance -= inputInformation.MapZoomIn * num * (CameraDistance + 20f);
			TargetCameraDistance += inputInformation.MapZoomOut * num * (CameraDistance + 20f);
		}
		PartyBase cameraFollowParty = Campaign.Current.CameraFollowParty;
		TargetCameraDistance = MBMath.ClampFloat(TargetCameraDistance, 2.5f, (cameraFollowParty != null && cameraFollowParty.IsMobile && (cameraFollowParty.MobileParty.BesiegedSettlement != null || (cameraFollowParty.MobileParty.CurrentSettlement != null && cameraFollowParty.MobileParty.CurrentSettlement.IsUnderSiege))) ? 30f : MaximumCameraHeight);
		float num2 = TargetCameraDistance - CameraDistance;
		float num3 = TaleWorlds.Library.MathF.Abs(num2);
		float cameraDistance = ((num3 > 0.001f) ? (CameraDistance + num2 * inputInformation.Dt * 8f) : TargetCameraDistance);
		if (CurrentCameraFollowMode == CameraFollowMode.Free && !inputInformation.RightMouseButtonDown && !inputInformation.LeftMouseButtonDown && num3 >= 0.001f && (inputInformation.WorldMouseFar - CameraFrame.origin).NormalizedCopy().z < -0.2f && inputInformation.RayCastForClosestEntityOrTerrainCondition)
		{
			MatrixFrame matrixFrame = ComputeMapCamera(ref _cameraTarget, CameraBearing + CameraBearingVelocity, TaleWorlds.Library.MathF.Min(CalculateCameraElevation(cameraDistance) + AdditionalElevation, (float)Math.PI * 99f / 200f), cameraDistance, ref _lastUsedIdealCameraTarget);
			Vec3 planeNormal2 = -Vec3.Up;
			Vec3 v = (inputInformation.WorldMouseFar - CameraFrame.origin).NormalizedCopy();
			Vec3 rayDirection2 = matrixFrame.rotation.TransformToParent(CameraFrame.rotation.TransformToLocal(v));
			if (MBMath.GetRayPlaneIntersectionPoint(in planeNormal2, in inputInformation.ProjectedPosition, in matrixFrame.origin, in rayDirection2, out var t2))
			{
				vec = inputInformation.ProjectedPosition.AsVec2 - (matrixFrame.origin + rayDirection2 * t2).AsVec2;
				result = false;
			}
		}
		if (inputInformation.RX != 0f || inputInformation.RY != 0f || vec.IsNonZero())
		{
			float num4 = 0.001f * (CameraDistance * 0.55f + 15f);
			Vec2 vec3 = Vec2.FromRotation(0f - CameraBearing);
			if ((IdealCameraTarget.AsVec2 - _lastUsedIdealCameraTarget).LengthSquared > 0.010000001f)
			{
				IdealCameraTarget = new Vec3(_lastUsedIdealCameraTarget.x, _lastUsedIdealCameraTarget.y, IdealCameraTarget.z);
				_cameraTarget = IdealCameraTarget;
			}
			if (!vec.IsNonZero())
			{
				IdealCameraTarget = _cameraTarget;
			}
			Vec2 vec4 = inputInformation.Dt * 500f * inputInformation.RX * vec3.RightVec() * num4 + inputInformation.Dt * 500f * inputInformation.RY * vec3 * num4;
			IdealCameraTarget = new Vec3(IdealCameraTarget.x + vec.x + vec4.x, IdealCameraTarget.y + vec.y + vec4.y, IdealCameraTarget.z);
			if (vec.IsNonZero())
			{
				_cameraTarget = IdealCameraTarget;
			}
			_cameraTarget.AsVec2 += vec4;
			if (inputInformation.RX != 0f || inputInformation.RY != 0f)
			{
				CurrentCameraFollowMode = CameraFollowMode.Free;
			}
		}
		CameraBearing += CameraBearingVelocity;
		CameraBearingVelocity = 0f;
		CameraDistance = cameraDistance;
		_cameraElevation = TaleWorlds.Library.MathF.Min(CalculateCameraElevation(cameraDistance) + AdditionalElevation, (float)Math.PI * 99f / 200f);
		if (CurrentCameraFollowMode == CameraFollowMode.FollowParty && cameraFollowParty != null && cameraFollowParty.IsValid)
		{
			Vec2 position;
			if (cameraFollowParty.IsMobile && cameraFollowParty.MobileParty.CurrentSettlement != null)
			{
				position = ((cameraFollowParty.MobileParty.CurrentSettlement.SiegeEvent != null) ? cameraFollowParty.MobileParty.CurrentSettlement.GatePosition : cameraFollowParty.MobileParty.CurrentSettlement.Position2D);
			}
			else if (cameraFollowParty.IsMobile && cameraFollowParty.MobileParty.BesiegedSettlement != null)
			{
				if (PlayerSiege.PlayerSiegeEvent != null)
				{
					Vec2 asVec = cameraFollowParty.MobileParty.BesiegedSettlement.Town.BesiegerCampPositions1.First().origin.AsVec2;
					position = Vec2.Lerp(cameraFollowParty.MobileParty.BesiegedSettlement.GatePosition, asVec, 0.75f);
				}
				else
				{
					position = cameraFollowParty.MobileParty.BesiegedSettlement.GatePosition;
				}
			}
			else
			{
				position = cameraFollowParty.Position2D;
			}
			float terrainHeight = _mapScene.GetTerrainHeight(position);
			IdealCameraTarget = new Vec3(position.x, position.y, terrainHeight + 1f);
		}
		return result;
	}

	protected virtual MatrixFrame ComputeMapCamera(ref Vec3 cameraTarget, float cameraBearing, float cameraElevation, float cameraDistance, ref Vec2 lastUsedIdealCameraTarget)
	{
		Vec2 asVec = cameraTarget.AsVec2;
		MatrixFrame identity = MatrixFrame.Identity;
		identity.origin = cameraTarget;
		identity.rotation.RotateAboutSide((float)Math.PI / 2f);
		identity.rotation.RotateAboutForward(0f - cameraBearing);
		identity.rotation.RotateAboutSide(0f - cameraElevation);
		identity.origin += identity.rotation.u * (cameraDistance + 2f);
		Vec2 vec = (Campaign.MapMinimumPosition + Campaign.MapMaximumPosition) * 0.5f;
		float num = Campaign.MapMaximumPosition.y - vec.y;
		float num2 = Campaign.MapMaximumPosition.x - vec.x;
		asVec.x = MBMath.ClampFloat(asVec.x, vec.x - num2, vec.x + num2);
		asVec.y = MBMath.ClampFloat(asVec.y, vec.y - num, vec.y + num);
		lastUsedIdealCameraTarget.x = MBMath.ClampFloat(lastUsedIdealCameraTarget.x, vec.x - num2, vec.x + num2);
		lastUsedIdealCameraTarget.y = MBMath.ClampFloat(lastUsedIdealCameraTarget.y, vec.y - num, vec.y + num);
		identity.origin.x += asVec.x - cameraTarget.x;
		identity.origin.y += asVec.y - cameraTarget.y;
		return identity;
	}

	protected virtual float CalculateCameraElevation(float cameraDistance)
	{
		return cameraDistance * 0.5f * 0.015f + 0.35f;
	}
}
