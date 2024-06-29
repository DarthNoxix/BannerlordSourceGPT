using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox;

public class SettlementPositionScript : ScriptComponentBehavior
{
	private struct SettlementRecord
	{
		public readonly string SettlementName;

		public readonly string SettlementId;

		public readonly XmlNode Node;

		public readonly Vec2 Position;

		public readonly Vec2 GatePosition;

		public readonly bool HasGate;

		public SettlementRecord(string settlementName, string settlementId, Vec2 position, Vec2 gatePosition, XmlNode node, bool hasGate)
		{
			SettlementName = settlementName;
			SettlementId = settlementId;
			Position = position;
			GatePosition = gatePosition;
			Node = node;
			HasGate = hasGate;
		}
	}

	public SimpleButton CheckPositions;

	public SimpleButton SavePositions;

	public SimpleButton ComputeAndSaveSettlementDistanceCache;

	private string SettlementsXmlPath
	{
		get
		{
			string modulePath = base.Scene.GetModulePath();
			modulePath = modulePath.Remove(0, 6);
			return BasePath.Name + modulePath + "ModuleData/settlements.xml";
		}
	}

	private string SettlementsDistanceCacheFilePath
	{
		get
		{
			string modulePath = base.Scene.GetModulePath();
			modulePath = modulePath.Remove(0, 6);
			return BasePath.Name + modulePath + "ModuleData/settlements_distance_cache.bin";
		}
	}

	protected override void OnEditorVariableChanged(string variableName)
	{
		base.OnEditorVariableChanged(variableName);
		if (variableName == "SavePositions")
		{
			SaveSettlementPositions();
		}
		if (variableName == "ComputeAndSaveSettlementDistanceCache")
		{
			SaveSettlementDistanceCache();
		}
		if (variableName == "CheckPositions")
		{
			CheckSettlementPositions();
		}
	}

	protected override void OnSceneSave(string saveFolder)
	{
		base.OnSceneSave(saveFolder);
		SaveSettlementPositions();
	}

	private void CheckSettlementPositions()
	{
		XmlDocument xmlDocument = LoadXmlFile(SettlementsXmlPath);
		base.GameEntity.RemoveAllChildren();
		foreach (XmlNode item in xmlDocument.DocumentElement.SelectNodes("Settlement"))
		{
			string value = item.Attributes["id"].Value;
			GameEntity campaignEntityWithName = base.Scene.GetCampaignEntityWithName(value);
			Vec3 origin = campaignEntityWithName.GetGlobalFrame().origin;
			Vec3 vec = default(Vec3);
			List<GameEntity> children = new List<GameEntity>();
			campaignEntityWithName.GetChildrenRecursive(ref children);
			bool flag = false;
			foreach (GameEntity item2 in children)
			{
				if (item2.HasTag("main_map_city_gate"))
				{
					vec = item2.GetGlobalFrame().origin;
					flag = true;
					break;
				}
			}
			Vec3 pos = origin;
			if (flag)
			{
				pos = vec;
			}
			PathFaceRecord record = new PathFaceRecord(-1, -1, -1);
			base.GameEntity.Scene.GetNavMeshFaceIndex(ref record, pos.AsVec2, checkIfDisabled: true);
			int num = 0;
			if (record.IsValid())
			{
				num = record.FaceGroupIndex;
			}
			if (num == 0 || num == 7 || num == 8 || num == 10 || num == 11 || num == 13 || num == 14)
			{
				MBEditor.ZoomToPosition(pos);
				break;
			}
		}
	}

	protected override void OnInit()
	{
		try
		{
			Debug.Print("SettlementsDistanceCacheFilePath: " + SettlementsDistanceCacheFilePath);
			System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(File.Open(SettlementsDistanceCacheFilePath, FileMode.Open, FileAccess.Read));
			if (Campaign.Current.Models.MapDistanceModel is DefaultMapDistanceModel)
			{
				((DefaultMapDistanceModel)Campaign.Current.Models.MapDistanceModel).LoadCacheFromFile(binaryReader);
			}
			binaryReader.Close();
		}
		catch
		{
			Debug.FailedAssert("SettlementsDistanceCacheFilePath could not be read!. Cache will be created right now, This may take few minutes!", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\SettlementPositionScript.cs", "OnInit", 165);
			Debug.Print("SettlementsDistanceCacheFilePath could not be read!. Cache will be created right now, This may take few minutes!");
			SaveSettlementDistanceCache();
		}
	}

	private List<SettlementRecord> LoadSettlementData(XmlDocument settlementDocument)
	{
		List<SettlementRecord> list = new List<SettlementRecord>();
		base.GameEntity.RemoveAllChildren();
		foreach (XmlNode item in settlementDocument.DocumentElement.SelectNodes("Settlement"))
		{
			string value = item.Attributes["name"].Value;
			string value2 = item.Attributes["id"].Value;
			GameEntity campaignEntityWithName = base.Scene.GetCampaignEntityWithName(value2);
			if (campaignEntityWithName == null)
			{
				continue;
			}
			Vec2 asVec = campaignEntityWithName.GetGlobalFrame().origin.AsVec2;
			Vec2 vec = default(Vec2);
			List<GameEntity> children = new List<GameEntity>();
			campaignEntityWithName.GetChildrenRecursive(ref children);
			bool flag = false;
			foreach (GameEntity item2 in children)
			{
				if (item2.HasTag("main_map_city_gate"))
				{
					vec = item2.GetGlobalFrame().origin.AsVec2;
					flag = true;
				}
			}
			list.Add(new SettlementRecord(value, value2, asVec, flag ? vec : asVec, item, flag));
		}
		return list;
	}

	private void SaveSettlementPositions()
	{
		XmlDocument xmlDocument = LoadXmlFile(SettlementsXmlPath);
		foreach (SettlementRecord item in LoadSettlementData(xmlDocument))
		{
			if (item.Node.Attributes["posX"] == null)
			{
				XmlAttribute node = xmlDocument.CreateAttribute("posX");
				item.Node.Attributes.Append(node);
			}
			item.Node.Attributes["posX"].Value = item.Position.X.ToString();
			if (item.Node.Attributes["posY"] == null)
			{
				XmlAttribute node2 = xmlDocument.CreateAttribute("posY");
				item.Node.Attributes.Append(node2);
			}
			item.Node.Attributes["posY"].Value = item.Position.Y.ToString();
			if (item.HasGate)
			{
				if (item.Node.Attributes["gate_posX"] == null)
				{
					XmlAttribute node3 = xmlDocument.CreateAttribute("gate_posX");
					item.Node.Attributes.Append(node3);
				}
				item.Node.Attributes["gate_posX"].Value = item.GatePosition.X.ToString();
				if (item.Node.Attributes["gate_posY"] == null)
				{
					XmlAttribute node4 = xmlDocument.CreateAttribute("gate_posY");
					item.Node.Attributes.Append(node4);
				}
				item.Node.Attributes["gate_posY"].Value = item.GatePosition.Y.ToString();
			}
		}
		xmlDocument.Save(SettlementsXmlPath);
	}

	private void SaveSettlementDistanceCache()
	{
		System.IO.BinaryWriter binaryWriter = null;
		try
		{
			XmlDocument settlementDocument = LoadXmlFile(SettlementsXmlPath);
			List<SettlementRecord> list = LoadSettlementData(settlementDocument);
			int navigationMeshIndexOfTerrainType = MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.Mountain);
			int navigationMeshIndexOfTerrainType2 = MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.Lake);
			int navigationMeshIndexOfTerrainType3 = MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.Water);
			int navigationMeshIndexOfTerrainType4 = MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.River);
			int navigationMeshIndexOfTerrainType5 = MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.Canyon);
			int navigationMeshIndexOfTerrainType6 = MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.RuralArea);
			base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType, isEnabled: false);
			base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType2, isEnabled: false);
			base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType3, isEnabled: false);
			base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType4, isEnabled: false);
			base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType5, isEnabled: false);
			base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType6, isEnabled: false);
			binaryWriter = new System.IO.BinaryWriter(File.Open(SettlementsDistanceCacheFilePath, FileMode.Create));
			binaryWriter.Write(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				binaryWriter.Write(list[i].SettlementId);
				Vec2 gatePosition = list[i].GatePosition;
				PathFaceRecord record = new PathFaceRecord(-1, -1, -1);
				base.Scene.GetNavMeshFaceIndex(ref record, gatePosition, checkIfDisabled: false);
				for (int j = i + 1; j < list.Count; j++)
				{
					binaryWriter.Write(list[j].SettlementId);
					Vec2 gatePosition2 = list[j].GatePosition;
					PathFaceRecord record2 = new PathFaceRecord(-1, -1, -1);
					base.Scene.GetNavMeshFaceIndex(ref record2, gatePosition2, checkIfDisabled: false);
					base.Scene.GetPathDistanceBetweenAIFaces(record.FaceIndex, record2.FaceIndex, gatePosition, gatePosition2, 0.1f, float.MaxValue, out var distance);
					binaryWriter.Write(distance);
				}
			}
			int navMeshFaceCount = base.Scene.GetNavMeshFaceCount();
			for (int k = 0; k < navMeshFaceCount; k++)
			{
				int idOfNavMeshFace = base.Scene.GetIdOfNavMeshFace(k);
				if (idOfNavMeshFace == navigationMeshIndexOfTerrainType || idOfNavMeshFace == navigationMeshIndexOfTerrainType2 || idOfNavMeshFace == navigationMeshIndexOfTerrainType3 || idOfNavMeshFace == navigationMeshIndexOfTerrainType4 || idOfNavMeshFace == navigationMeshIndexOfTerrainType5 || idOfNavMeshFace == navigationMeshIndexOfTerrainType6)
				{
					continue;
				}
				Vec3 centerPosition = Vec3.Zero;
				base.Scene.GetNavMeshCenterPosition(k, ref centerPosition);
				Vec2 asVec = centerPosition.AsVec2;
				float num = float.MaxValue;
				string value = "";
				for (int l = 0; l < list.Count; l++)
				{
					Vec2 gatePosition3 = list[l].GatePosition;
					PathFaceRecord record3 = new PathFaceRecord(-1, -1, -1);
					base.Scene.GetNavMeshFaceIndex(ref record3, gatePosition3, checkIfDisabled: false);
					if ((num == float.MaxValue || asVec.DistanceSquared(gatePosition3) < num * num) && base.Scene.GetPathDistanceBetweenAIFaces(k, record3.FaceIndex, asVec, gatePosition3, 0.1f, num, out var distance2) && distance2 < num)
					{
						num = distance2;
						value = list[l].SettlementId;
					}
				}
				if (!string.IsNullOrEmpty(value))
				{
					binaryWriter.Write(k);
					binaryWriter.Write(value);
				}
			}
			binaryWriter.Write(-1);
		}
		catch
		{
		}
		finally
		{
			base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.Mountain), isEnabled: true);
			base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.Lake), isEnabled: true);
			base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.Water), isEnabled: true);
			base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.River), isEnabled: true);
			base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.Canyon), isEnabled: true);
			base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(TerrainType.RuralArea), isEnabled: true);
			binaryWriter?.Close();
		}
	}

	private XmlDocument LoadXmlFile(string path)
	{
		Debug.Print("opening " + path);
		XmlDocument xmlDocument = new XmlDocument();
		StreamReader streamReader = new StreamReader(path);
		string xml = streamReader.ReadToEnd();
		xmlDocument.LoadXml(xml);
		streamReader.Close();
		return xmlDocument;
	}

	protected override bool IsOnlyVisual()
	{
		return true;
	}
}
