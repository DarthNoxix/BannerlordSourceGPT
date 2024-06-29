using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace SandBox;

public class SettlementVisualizer : ScriptComponentBehavior
{
	private class SettlementInstance
	{
		public GameEntity ChildEntity;

		public string SettlementName;

		public XmlNode Node;

		public Vec2 OriginalPosition;

		public SettlementInstance(GameEntity childEntity, XmlNode node, string settlementName, Vec2 originalPosition)
		{
			ChildEntity = childEntity;
			Node = node;
			SettlementName = settlementName;
			OriginalPosition = originalPosition;
		}
	}

	public SimpleButton ReloadXML;

	public SimpleButton SaveXML;

	public SimpleButton SnapToTerrain;

	public SimpleButton CheckNavMesh;

	public bool renderSettlementName;

	public float translateScale = 1f;

	private XmlDocument _doc;

	private List<SettlementInstance> _settlementDatas;

	private const string settlemensXmlPath = "/Modules/SandBox/ModuleData/settlements.xml";

	private void CheckNavMeshAux()
	{
		if (_settlementDatas == null)
		{
			return;
		}
		foreach (SettlementInstance settlementData in _settlementDatas)
		{
			MatrixFrame globalFrame = settlementData.ChildEntity.GetGlobalFrame();
			PathFaceRecord record = PathFaceRecord.NullFaceRecord;
			base.GameEntity.Scene.GetNavMeshFaceIndex(ref record, globalFrame.origin.AsVec2, checkIfDisabled: false);
			if (record.FaceIndex == -1)
			{
				Debug.Print("Settlement(" + settlementData.SettlementName + ") has no nav mesh under");
			}
		}
	}

	private void SnapToTerrainAux()
	{
		foreach (SettlementInstance settlementData in _settlementDatas)
		{
			MatrixFrame frame = settlementData.ChildEntity.GetGlobalFrame();
			float height = 0f;
			settlementData.ChildEntity.Scene.GetHeightAtPoint(frame.origin.AsVec2, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref height);
			frame.origin.z = height;
			settlementData.ChildEntity.SetGlobalFrame(in frame);
			settlementData.ChildEntity.UpdateTriadFrameForEditor();
		}
	}

	protected override void OnEditorVariableChanged(string variableName)
	{
		base.OnEditorVariableChanged(variableName);
		switch (variableName)
		{
		case "ReloadXML":
			LoadFromXml();
			break;
		case "SnapToTerrain":
			SnapToTerrainAux();
			break;
		case "translateScale":
			RepositionAfterScale();
			break;
		case "CheckNavMesh":
			CheckNavMeshAux();
			break;
		}
	}

	private void RepositionAfterScale()
	{
		foreach (SettlementInstance settlementData in _settlementDatas)
		{
			MatrixFrame frame = settlementData.ChildEntity.GetGlobalFrame();
			Vec2 vec = settlementData.OriginalPosition * translateScale;
			frame.origin.x = vec.x;
			frame.origin.y = vec.y;
			float height = 0f;
			settlementData.ChildEntity.Scene.GetHeightAtPoint(frame.origin.AsVec2, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref height);
			frame.origin.z = height;
			settlementData.ChildEntity.SetGlobalFrame(in frame);
			settlementData.ChildEntity.UpdateTriadFrameForEditor();
		}
	}

	private void LoadFromXml()
	{
		_settlementDatas = new List<SettlementInstance>();
		_doc = LoadXmlFile(BasePath.Name + "/Modules/SandBox/ModuleData/settlements.xml");
		base.GameEntity.RemoveAllChildren();
		foreach (XmlNode item in _doc.DocumentElement.SelectNodes("Settlement"))
		{
			if (item.Attributes["posX"] == null || item.Attributes["posY"] == null)
			{
				continue;
			}
			GameEntity gameEntity = GameEntity.CreateEmpty(base.GameEntity.Scene);
			MatrixFrame frame = gameEntity.GetGlobalFrame();
			Vec2 vec = new Vec2((float)Convert.ToDouble(item.Attributes["posX"].Value), (float)Convert.ToDouble(item.Attributes["posY"].Value));
			string settlementName = (gameEntity.Name = item.Attributes["name"].Value);
			float height = 0f;
			base.GameEntity.Scene.GetHeightAtPoint(vec, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref height);
			frame.origin = new Vec3(vec, height);
			if (item.Attributes["culture"] != null)
			{
				string value2 = item.Attributes["culture"].Value;
				value2.Substring(value2.IndexOf('.') + 1);
				MetaMesh metaMesh = null;
				gameEntity.SetGlobalFrame(in frame);
				gameEntity.EntityFlags |= EntityFlags.DontSaveToScene;
				base.GameEntity.AddChild(gameEntity, autoLocalizeFrame: true);
				gameEntity.GetGlobalFrame();
				gameEntity.UpdateTriadFrameForEditor();
				_settlementDatas.Add(new SettlementInstance(gameEntity, item, settlementName, vec));
				if (metaMesh != null)
				{
					gameEntity.AddMultiMesh(metaMesh);
				}
				else
				{
					gameEntity.AddMultiMesh(MetaMesh.GetCopy("map_icon_bandit_hideout_a"));
				}
			}
			else
			{
				gameEntity.AddMultiMesh(MetaMesh.GetCopy("map_icon_bandit_hideout_a"));
			}
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

	protected override void OnEditorTick(float dt)
	{
		if (Input.IsKeyDown(InputKey.LeftAlt) && Input.IsKeyDown(InputKey.LeftControl) && Input.IsKeyPressed(InputKey.A))
		{
			SnapToTerrainAux();
		}
		if (!renderSettlementName || _settlementDatas == null)
		{
			return;
		}
		foreach (SettlementInstance settlementData in _settlementDatas)
		{
			MatrixFrame globalFrame = settlementData.ChildEntity.GetGlobalFrame();
			globalFrame.origin.z += 1.5f;
		}
	}

	protected override bool IsOnlyVisual()
	{
		return true;
	}
}
