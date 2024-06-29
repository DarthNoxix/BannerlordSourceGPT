using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.View.Map;

public class MapTracksVisual : CampaignEntityVisualComponent
{
	private const string TrackPrefabName = "map_track_arrow";

	private const int DefaultObjectPoolCount = 5;

	private readonly List<GameEntity> _trackEntityPool;

	private SphereData _trackSphere;

	private Scene _mapScene;

	private bool _tracksDirty = true;

	private readonly TWParallel.ParallelForAuxPredicate _parallelUpdateTrackColorsPredicate;

	private readonly TWParallel.ParallelForAuxPredicate _parallelMakeTrackPoolElementsInvisiblePredicate;

	private readonly TWParallel.ParallelForAuxPredicate _parallelUpdateTrackPoolPositionsPredicate;

	private readonly TWParallel.ParallelForAuxPredicate _parallelUpdateVisibleTracksPredicate;

	public Scene MapScene
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

	public MapTracksVisual()
	{
		_trackEntityPool = new List<GameEntity>();
		_parallelUpdateTrackColorsPredicate = ParallelUpdateTrackColors;
		_parallelMakeTrackPoolElementsInvisiblePredicate = ParallelMakeTrackPoolElementsInvisible;
		_parallelUpdateTrackPoolPositionsPredicate = ParallelUpdateTrackPoolPositions;
		_parallelUpdateVisibleTracksPredicate = ParallelUpdateVisibleTracks;
	}

	protected override void OnInitialize()
	{
		base.OnInitialize();
		CampaignEvents.TrackDetectedEvent.AddNonSerializedListener(this, OnTrackDetected);
		CampaignEvents.TrackLostEvent.AddNonSerializedListener(this, OnTrackLost);
		InitializeObjectPoolWithDefaultCount();
	}

	private void OnTrackDetected(Track track)
	{
		_tracksDirty = true;
	}

	private void OnTrackLost(Track track)
	{
		_tracksDirty = true;
	}

	private void ParallelUpdateTrackColors(int startInclusive, int endExclusive)
	{
		for (int i = startInclusive; i < endExclusive; i++)
		{
			(_trackEntityPool[i].GetComponentAtIndex(0, GameEntity.ComponentType.Decal) as Decal).SetFactor1(Campaign.Current.Models.MapTrackModel.GetTrackColor(MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks[i]));
		}
	}

	private void UpdateTrackMesh()
	{
		int num = _trackEntityPool.Count - MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count;
		if (num > 0)
		{
			int count = MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count;
			TWParallel.For(count, count + num, _parallelMakeTrackPoolElementsInvisiblePredicate);
		}
		else
		{
			CreateNewTrackPoolElements(-num);
		}
		TWParallel.For(0, MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count, _parallelUpdateVisibleTracksPredicate);
		TWParallel.For(0, MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count, _parallelUpdateTrackPoolPositionsPredicate);
	}

	private void ParallelUpdateTrackPoolPositions(int startInclusive, int endExclusive)
	{
		for (int i = startInclusive; i < endExclusive; i++)
		{
			Track track = MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks[i];
			MatrixFrame frame = CalculateTrackFrame(track);
			_trackEntityPool[i].SetFrame(ref frame);
		}
	}

	private void ParallelUpdateVisibleTracks(int startInclusive, int endExclusive)
	{
		for (int i = startInclusive; i < endExclusive; i++)
		{
			_trackEntityPool[i].SetVisibilityExcludeParents(visible: true);
		}
	}

	private void ParallelMakeTrackPoolElementsInvisible(int startInclusive, int endExclusive)
	{
		for (int i = startInclusive; i < endExclusive; i++)
		{
			_trackEntityPool[i].SetVisibilityExcludeParents(visible: false);
		}
	}

	private void InitializeObjectPoolWithDefaultCount()
	{
		CreateNewTrackPoolElements(5);
		foreach (GameEntity item in _trackEntityPool)
		{
			item.SetVisibilityExcludeParents(visible: false);
		}
	}

	private void CreateNewTrackPoolElements(int delta)
	{
		for (int i = 0; i < delta; i++)
		{
			GameEntity gameEntity = GameEntity.Instantiate(MapScene, "map_track_arrow", MatrixFrame.Identity);
			gameEntity.SetVisibilityExcludeParents(visible: true);
			_trackEntityPool.Add(gameEntity);
		}
	}

	public override void OnVisualTick(MapScreen screen, float realDt, float dt)
	{
		if (_tracksDirty)
		{
			UpdateTrackMesh();
			_tracksDirty = false;
		}
		TWParallel.For(0, MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count, _parallelUpdateTrackColorsPredicate);
	}

	public bool RaySphereIntersection(Ray ray, SphereData sphere, ref Vec3 intersectionPoint)
	{
		Vec3 origin = sphere.Origin;
		float radius = sphere.Radius;
		Vec3 v = origin - ray.Origin;
		float num = Vec3.DotProduct(ray.Direction, v);
		if (num > 0f)
		{
			float num2 = radius * radius - (ray.Origin + ray.Direction * num - origin).LengthSquared;
			if (num2 >= 0f)
			{
				float num3 = MathF.Sqrt(num2);
				float num4 = num - num3;
				if (num4 >= 0f && num4 <= ray.MaxDistance)
				{
					intersectionPoint = ray.Origin + ray.Direction * num4;
					return true;
				}
				if (num4 < 0f)
				{
					intersectionPoint = ray.Origin;
					return true;
				}
			}
		}
		else if ((ray.Origin - origin).LengthSquared < radius * radius)
		{
			intersectionPoint = ray.Origin;
			return true;
		}
		return false;
	}

	public Track GetTrackOnMouse(Ray mouseRay, Vec3 mouseIntersectionPoint)
	{
		Track result = null;
		for (int i = 0; i < MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count; i++)
		{
			Track track = MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks[i];
			float trackScale = Campaign.Current.Models.MapTrackModel.GetTrackScale(track);
			MatrixFrame matrixFrame = CalculateTrackFrame(track);
			float lengthSquared = (matrixFrame.origin - mouseIntersectionPoint).LengthSquared;
			if (lengthSquared < 0.1f)
			{
				float num = MathF.Sqrt(lengthSquared);
				_trackSphere.Origin = matrixFrame.origin;
				_trackSphere.Radius = 0.05f + num * 0.01f + trackScale;
				Vec3 intersectionPoint = default(Vec3);
				if (RaySphereIntersection(mouseRay, _trackSphere, ref intersectionPoint))
				{
					result = track;
				}
			}
		}
		return result;
	}

	private MatrixFrame CalculateTrackFrame(Track track)
	{
		Vec3 origin = track.Position.ToVec3();
		float scale = track.Scale;
		MatrixFrame identity = MatrixFrame.Identity;
		identity.origin = origin;
		Campaign.Current.MapSceneWrapper.GetTerrainHeightAndNormal(identity.origin.AsVec2, out var height, out var normal);
		identity.origin.z = height + 0.01f;
		identity.rotation.u = normal;
		Vec2 asVec = identity.rotation.f.AsVec2;
		asVec.RotateCCW(track.Direction);
		identity.rotation.f = new Vec3(asVec.x, asVec.y, identity.rotation.f.z);
		identity.rotation.s = Vec3.CrossProduct(identity.rotation.f, identity.rotation.u);
		identity.rotation.s.Normalize();
		identity.rotation.f = Vec3.CrossProduct(identity.rotation.u, identity.rotation.s);
		identity.rotation.f.Normalize();
		float num = scale;
		identity.rotation.s *= num;
		identity.rotation.f *= num;
		identity.rotation.u *= num;
		return identity;
	}
}
