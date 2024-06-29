using System.Collections.Generic;
using System.Linq;
using SandBox.BoardGames.MissionLogics;
using SandBox.BoardGames.Objects;
using SandBox.BoardGames.Pawns;
using SandBox.BoardGames.Tiles;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.BoardGames;

public class BoardGameTablut : BoardGameBase
{
	public struct PawnInformation
	{
		public int X;

		public int Y;

		public bool IsCaptured;

		public PawnInformation(int x, int y, bool captured)
		{
			X = x;
			Y = y;
			IsCaptured = captured;
		}
	}

	public struct BoardInformation
	{
		public readonly PawnInformation[] PawnInformation;

		public readonly PlayerTurn Turn;

		public BoardInformation(ref PawnInformation[] pawns, PlayerTurn turn)
		{
			PawnInformation = pawns;
			Turn = turn;
		}
	}

	public enum State
	{
		InProgress,
		Aborted,
		PlayerWon,
		AIWon
	}

	public const int BoardWidth = 9;

	public const int BoardHeight = 9;

	public const int AttackerPawnCount = 16;

	public const int DefenderPawnCount = 9;

	private BoardInformation _startState;

	public override int TileCount => 81;

	protected override bool RotateBoard => false;

	protected override bool PreMovementStagePresent => false;

	protected override bool DiceRollRequired => false;

	private PawnTablut King { get; set; }

	public BoardGameTablut(MissionBoardGameLogic mission, PlayerTurn startingPlayer)
		: base(mission, new TextObject("{=qeKskdiY}Tablut"), startingPlayer)
	{
		SelectedUnit = null;
		PawnUnselectedFactor = 4287395960u;
	}

	public static bool IsCitadelTile(int tileX, int tileY)
	{
		if (tileX == 4)
		{
			return tileY == 4;
		}
		return false;
	}

	public override void InitializeUnits()
	{
		base.PlayerOneUnits.Clear();
		base.PlayerTwoUnits.Clear();
		List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
		for (int i = 0; i < 16; i++)
		{
			GameEntity entity = Mission.Current.Scene.FindEntityWithTag("player_one_unit_" + i);
			list.Add(InitializeUnit(new PawnTablut(entity, base.PlayerWhoStarted == PlayerTurn.PlayerOne)));
		}
		List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
		for (int j = 0; j < 9; j++)
		{
			GameEntity entity2 = Mission.Current.Scene.FindEntityWithTag("player_two_unit_" + j);
			list2.Add(InitializeUnit(new PawnTablut(entity2, base.PlayerWhoStarted != PlayerTurn.PlayerOne)));
		}
		King = list2[0] as PawnTablut;
	}

	public override void InitializeTiles()
	{
		IEnumerable<GameEntity> source = from x in BoardEntity.GetChildren()
			where x.Tags.Any((string t) => t.Contains("tile_"))
			select x;
		IEnumerable<GameEntity> source2 = from x in BoardEntity.GetChildren()
			where x.Tags.Any((string t) => t.Contains("decal_"))
			select x;
		if (base.Tiles == null)
		{
			base.Tiles = new TileBase[TileCount];
		}
		int x2 = 0;
		while (x2 < 9)
		{
			int y;
			int num;
			for (y = 0; y < 9; y = num)
			{
				GameEntity entity = source.Single((GameEntity e) => e.HasTag("tile_" + x2 + "_" + y));
				BoardGameDecal firstScriptOfType = source2.Single((GameEntity e) => e.HasTag("decal_" + x2 + "_" + y)).GetFirstScriptOfType<BoardGameDecal>();
				Tile2D tile = new Tile2D(entity, firstScriptOfType, x2, y);
				SetTile(tile, x2, y);
				num = y + 1;
			}
			num = x2 + 1;
			x2 = num;
		}
	}

	public override void InitializeSound()
	{
		PawnBase.PawnMoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/move_stone");
		PawnBase.PawnSelectSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/pick_stone");
		PawnBase.PawnTapSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/drop_stone");
		PawnBase.PawnRemoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/out_stone");
	}

	public override void Reset()
	{
		base.Reset();
		if (_startState.PawnInformation == null)
		{
			PreplaceUnits();
		}
		else
		{
			RestoreStartingBoard();
		}
	}

	public override List<Move> CalculateValidMoves(PawnBase pawn)
	{
		List<Move> list = new List<Move>(16);
		if (pawn.IsPlaced && !pawn.Captured)
		{
			PawnTablut pawnTablut = pawn as PawnTablut;
			int num = pawnTablut.X;
			int num2 = pawnTablut.Y;
			while (num > 0)
			{
				num--;
				if (!AddValidMove(list, pawn, num, num2))
				{
					break;
				}
			}
			num = pawnTablut.X;
			while (num < 8)
			{
				num++;
				if (!AddValidMove(list, pawn, num, num2))
				{
					break;
				}
			}
			num = pawnTablut.X;
			while (num2 < 8)
			{
				num2++;
				if (!AddValidMove(list, pawn, num, num2))
				{
					break;
				}
			}
			num2 = pawnTablut.Y;
			while (num2 > 0)
			{
				num2--;
				if (!AddValidMove(list, pawn, num, num2))
				{
					break;
				}
			}
			num2 = pawnTablut.Y;
		}
		return list;
	}

	public override void SetPawnCaptured(PawnBase pawn, bool fake = false)
	{
		base.SetPawnCaptured(pawn, fake);
		PawnTablut pawnTablut = pawn as PawnTablut;
		GetTile(pawnTablut.X, pawnTablut.Y).PawnOnTile = null;
		pawnTablut.X = -1;
		pawnTablut.Y = -1;
		if (!fake)
		{
			RemovePawnFromBoard(pawnTablut, 0.6f);
		}
	}

	protected override void OnAfterBoardSetUp()
	{
		if (_startState.PawnInformation == null)
		{
			_startState = TakeBoardSnapshot();
		}
		ReadyToPlay = true;
	}

	protected override PawnBase SelectPawn(PawnBase pawn)
	{
		if (pawn.PlayerOne == (base.PlayerTurn == PlayerTurn.PlayerOne))
		{
			SelectedUnit = pawn;
		}
		return pawn;
	}

	protected override void MovePawnToTileDelayed(PawnBase pawn, TileBase tile, bool instantMove, bool displayMessage, float delay)
	{
		base.MovePawnToTileDelayed(pawn, tile, instantMove, displayMessage, delay);
		Tile2D tile2D = tile as Tile2D;
		PawnTablut pawnTablut = pawn as PawnTablut;
		if (tile2D.PawnOnTile != null)
		{
			return;
		}
		if (displayMessage)
		{
			if (base.PlayerTurn == PlayerTurn.PlayerOne)
			{
				InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_boardgame_move_piece_player").ToString()));
			}
			else
			{
				InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_boardgame_move_piece_opponent").ToString()));
			}
		}
		Vec3 globalPosition = pawnTablut.Entity.GlobalPosition;
		Vec3 globalPosition2 = tile2D.Entity.GlobalPosition;
		if (pawnTablut.X != -1 && pawnTablut.Y != -1)
		{
			GetTile(pawnTablut.X, pawnTablut.Y).PawnOnTile = null;
		}
		pawnTablut.MovingToDifferentTile = pawnTablut.X != tile2D.X || pawnTablut.Y != tile2D.Y;
		pawnTablut.X = tile2D.X;
		pawnTablut.Y = tile2D.Y;
		tile2D.PawnOnTile = pawnTablut;
		if (SettingUpBoard && globalPosition2.z > globalPosition.z)
		{
			Vec3 goal = globalPosition;
			goal.z += 2f * (globalPosition2.z - globalPosition.z);
			pawnTablut.AddGoalPosition(goal);
			pawnTablut.MovePawnToGoalPositionsDelayed(instantMove, 0.5f, JustStoppedDraggingUnit, delay);
		}
		pawnTablut.AddGoalPosition(globalPosition2);
		pawnTablut.MovePawnToGoalPositionsDelayed(instantMove, 0.5f, JustStoppedDraggingUnit, delay);
		if (instantMove)
		{
			CheckIfPawnCaptures(SelectedUnit as PawnTablut);
		}
		if (pawnTablut == SelectedUnit && instantMove)
		{
			SelectedUnit = null;
		}
	}

	protected override void SwitchPlayerTurn()
	{
		if ((base.PlayerTurn == PlayerTurn.PlayerOneWaiting || base.PlayerTurn == PlayerTurn.PlayerTwoWaiting) && SelectedUnit != null)
		{
			CheckIfPawnCaptures(SelectedUnit as PawnTablut);
		}
		SelectedUnit = null;
		if (base.PlayerTurn == PlayerTurn.PlayerOneWaiting)
		{
			base.PlayerTurn = PlayerTurn.PlayerTwo;
		}
		else if (base.PlayerTurn == PlayerTurn.PlayerTwoWaiting)
		{
			base.PlayerTurn = PlayerTurn.PlayerOne;
		}
		CheckGameEnded();
		base.SwitchPlayerTurn();
	}

	protected override bool CheckGameEnded()
	{
		State state = CheckGameState();
		bool result = true;
		switch (state)
		{
		case State.InProgress:
			result = false;
			break;
		case State.PlayerWon:
			OnVictory();
			ReadyToPlay = false;
			break;
		case State.AIWon:
			OnDefeat();
			ReadyToPlay = false;
			break;
		}
		return result;
	}

	public bool AIMakeMove(Move move)
	{
		Tile2D tile2D = move.GoalTile as Tile2D;
		PawnTablut pawnTablut = move.Unit as PawnTablut;
		if (tile2D.PawnOnTile == null)
		{
			if (pawnTablut.X != -1 && pawnTablut.Y != -1)
			{
				GetTile(pawnTablut.X, pawnTablut.Y).PawnOnTile = null;
			}
			pawnTablut.X = tile2D.X;
			pawnTablut.Y = tile2D.Y;
			tile2D.PawnOnTile = pawnTablut;
			CheckIfPawnCaptures(pawnTablut, fake: true);
			return true;
		}
		return false;
	}

	public bool HasAvailableMoves(PawnTablut pawn)
	{
		bool result = false;
		if (pawn.IsPlaced && !pawn.Captured)
		{
			int x = pawn.X;
			int y = pawn.Y;
			result = (x > 0 && GetTile(x - 1, y).PawnOnTile == null && !IsCitadelTile(x - 1, y)) || (x < 8 && GetTile(x + 1, y).PawnOnTile == null && !IsCitadelTile(x + 1, y)) || (y > 0 && GetTile(x, y - 1).PawnOnTile == null && !IsCitadelTile(x, y - 1)) || (y < 8 && GetTile(x, y + 1).PawnOnTile == null && !IsCitadelTile(x, y + 1));
		}
		return result;
	}

	public Move GetRandomAvailableMove(PawnTablut pawn)
	{
		List<Move> list = CalculateValidMoves(pawn);
		return list[MBRandom.RandomInt(list.Count)];
	}

	public BoardInformation TakeBoardSnapshot()
	{
		List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
		List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
		PawnInformation[] pawns = new PawnInformation[25];
		PawnInformation pawnInformation = default(PawnInformation);
		for (int i = 0; i < 25; i++)
		{
			PawnTablut pawnTablut = ((i >= 16) ? (list2[i - 16] as PawnTablut) : (list[i] as PawnTablut));
			pawnInformation.X = pawnTablut.X;
			pawnInformation.Y = pawnTablut.Y;
			pawnInformation.IsCaptured = pawnTablut.Captured;
			pawns[i] = pawnInformation;
		}
		PlayerTurn playerTurn = base.PlayerTurn;
		return new BoardInformation(ref pawns, playerTurn);
	}

	public void UndoMove(ref BoardInformation board)
	{
		for (int i = 0; i < TileCount; i++)
		{
			base.Tiles[i].PawnOnTile = null;
		}
		List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
		List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
		for (int j = 0; j < 25; j++)
		{
			PawnInformation pawnInformation = board.PawnInformation[j];
			PawnTablut pawnTablut = ((j >= 16) ? (list2[j - 16] as PawnTablut) : (list[j] as PawnTablut));
			pawnTablut.X = pawnInformation.X;
			pawnTablut.Y = pawnInformation.Y;
			pawnTablut.Captured = pawnInformation.IsCaptured;
			if (pawnTablut.IsPlaced)
			{
				GetTile(pawnTablut.X, pawnTablut.Y).PawnOnTile = pawnTablut;
			}
		}
		base.PlayerTurn = board.Turn;
	}

	public State CheckGameState()
	{
		State result;
		if (!base.AIOpponent.AbortRequested)
		{
			result = State.InProgress;
			if (base.PlayerTurn == PlayerTurn.PlayerOne || base.PlayerTurn == PlayerTurn.PlayerTwo)
			{
				bool flag = base.PlayerWhoStarted == PlayerTurn.PlayerOne;
				if (King.Captured)
				{
					result = (flag ? State.PlayerWon : State.AIWon);
				}
				else if (King.X == 0 || King.X == 8 || King.Y == 0 || King.Y == 8)
				{
					result = (flag ? State.AIWon : State.PlayerWon);
				}
				else
				{
					bool flag2 = false;
					bool flag3 = base.PlayerTurn == PlayerTurn.PlayerOne;
					List<PawnBase> list = (flag3 ? base.PlayerOneUnits : base.PlayerTwoUnits);
					int count = list.Count;
					for (int i = 0; i < count; i++)
					{
						PawnBase pawnBase = list[i];
						if (pawnBase.IsPlaced && !pawnBase.Captured && HasAvailableMoves(pawnBase as PawnTablut))
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						result = (flag3 ? State.AIWon : State.PlayerWon);
					}
				}
			}
		}
		else
		{
			result = State.Aborted;
		}
		return result;
	}

	private void SetTile(TileBase tile, int x, int y)
	{
		base.Tiles[y * 9 + x] = tile;
	}

	private TileBase GetTile(int x, int y)
	{
		return base.Tiles[y * 9 + x];
	}

	private void PreplaceUnits()
	{
		int[] array = new int[32]
		{
			3, 0, 4, 0, 5, 0, 4, 1, 0, 3,
			0, 4, 0, 5, 1, 4, 8, 3, 8, 4,
			8, 5, 7, 4, 3, 8, 4, 8, 5, 8,
			4, 7
		};
		int[] array2 = new int[18]
		{
			4, 4, 4, 3, 4, 2, 5, 4, 6, 4,
			3, 4, 2, 4, 4, 5, 4, 6
		};
		List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			int x = array[i * 2];
			int y = array[i * 2 + 1];
			MovePawnToTileDelayed(list[i], GetTile(x, y), instantMove: false, displayMessage: false, 0.15f * (float)(i + 1) + 0.25f);
		}
		List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
		int count2 = list2.Count;
		for (int j = 0; j < count2; j++)
		{
			int x2 = array2[j * 2];
			int y2 = array2[j * 2 + 1];
			MovePawnToTileDelayed(list2[j], GetTile(x2, y2), instantMove: false, displayMessage: false, 0.15f * (float)(j + 1) + 0.25f);
		}
	}

	private void RestoreStartingBoard()
	{
		List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
		List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
		for (int i = 0; i < 25; i++)
		{
			PawnBase pawnBase = ((i >= 16) ? list2[i - 16] : list[i]);
			PawnInformation pawnInformation = _startState.PawnInformation[i];
			TileBase tile = GetTile(pawnInformation.X, pawnInformation.Y);
			pawnBase.Reset();
			MovePawnToTile(pawnBase, tile, instantMove: false, displayMessage: false);
		}
	}

	private bool AddValidMove(List<Move> moves, PawnBase pawn, int x, int y)
	{
		bool result = false;
		TileBase tile = GetTile(x, y);
		if (tile.PawnOnTile == null && !IsCitadelTile(x, y))
		{
			Move item = default(Move);
			item.Unit = pawn;
			item.GoalTile = tile;
			moves.Add(item);
			result = true;
		}
		return result;
	}

	private void CheckIfPawnCapturedEnemyPawn(PawnTablut pawn, bool fake, TileBase victimTile, Tile2D helperTile)
	{
		PawnBase pawnOnTile = victimTile.PawnOnTile;
		if (pawnOnTile == null || pawnOnTile.PlayerOne == pawn.PlayerOne)
		{
			return;
		}
		PawnBase pawnOnTile2 = helperTile.PawnOnTile;
		if (pawnOnTile2 != null)
		{
			if (pawnOnTile2.PlayerOne == pawn.PlayerOne)
			{
				SetPawnCaptured(pawnOnTile, fake);
			}
		}
		else if (IsCitadelTile(helperTile.X, helperTile.Y))
		{
			SetPawnCaptured(pawnOnTile, fake);
		}
	}

	private void CheckIfPawnCaptures(PawnTablut pawn, bool fake = false)
	{
		int x = pawn.X;
		int y = pawn.Y;
		if (x > 1)
		{
			CheckIfPawnCapturedEnemyPawn(pawn, fake, GetTile(x - 1, y), GetTile(x - 2, y) as Tile2D);
		}
		if (x < 7)
		{
			CheckIfPawnCapturedEnemyPawn(pawn, fake, GetTile(x + 1, y), GetTile(x + 2, y) as Tile2D);
		}
		if (y > 1)
		{
			CheckIfPawnCapturedEnemyPawn(pawn, fake, GetTile(x, y - 1), GetTile(x, y - 2) as Tile2D);
		}
		if (y < 7)
		{
			CheckIfPawnCapturedEnemyPawn(pawn, fake, GetTile(x, y + 1), GetTile(x, y + 2) as Tile2D);
		}
	}
}
