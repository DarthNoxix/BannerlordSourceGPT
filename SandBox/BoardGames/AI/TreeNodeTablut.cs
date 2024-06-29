using System.Collections.Generic;
using System.Linq;
using SandBox.BoardGames.Pawns;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.BoardGames.AI;

public class TreeNodeTablut
{
	private enum ExpandResult
	{
		NeedsToBeSimulated,
		AIWon,
		PlayerWon
	}

	private const float UCTConstant = 1.5f;

	private static int MaxDepth;

	private readonly int _depth;

	private BoardGameTablut.BoardInformation _boardState;

	private TreeNodeTablut _parent;

	private List<TreeNodeTablut> _children;

	private BoardGameSide _side;

	private int _visits;

	private int _wins;

	public Move OpeningMove { get; private set; }

	private bool IsLeaf => _children == null;

	public TreeNodeTablut(BoardGameSide side, int depth)
	{
		_side = side;
		_depth = depth;
	}

	public static TreeNodeTablut CreateTreeAndReturnRootNode(BoardGameTablut.BoardInformation initialBoardState, int maxDepth)
	{
		MaxDepth = maxDepth;
		return new TreeNodeTablut(BoardGameSide.Player, 0)
		{
			_boardState = initialBoardState
		};
	}

	public TreeNodeTablut GetChildWithBestScore()
	{
		TreeNodeTablut result = null;
		if (!IsLeaf)
		{
			float num = float.MinValue;
			{
				foreach (TreeNodeTablut child in _children)
				{
					if (child._visits > 0)
					{
						float num2 = (float)child._wins / (float)child._visits;
						if (num2 > num)
						{
							result = child;
							num = num2;
						}
					}
				}
				return result;
			}
		}
		return result;
	}

	public void SelectAction()
	{
		TreeNodeTablut tn = this;
		while (!tn.IsLeaf)
		{
			tn = tn.Select();
		}
		int num;
		switch (tn.Expand())
		{
		case ExpandResult.NeedsToBeSimulated:
		{
			if (tn._children != null)
			{
				tn = tn.Select();
			}
			int num2;
			switch (Simulate(ref tn))
			{
			default:
				num2 = 0;
				break;
			case BoardGameTablut.State.AIWon:
				num2 = 1;
				break;
			case BoardGameTablut.State.Aborted:
				return;
			}
			BoardGameSide winner = (BoardGameSide)num2;
			tn.BackPropagate(winner);
			return;
		}
		default:
			num = 0;
			break;
		case ExpandResult.AIWon:
			num = 1;
			break;
		}
		BoardGameSide winner2 = (BoardGameSide)num;
		tn.BackPropagate(winner2);
	}

	private TreeNodeTablut Select()
	{
		TreeNodeTablut treeNodeTablut = null;
		if (!IsLeaf)
		{
			double num = double.MinValue;
			foreach (TreeNodeTablut child in _children)
			{
				if (child._visits == 0)
				{
					treeNodeTablut = child;
					break;
				}
				double num2 = (double)child._wins / (double)child._visits + (double)(1.5f * MathF.Sqrt(MathF.Log(_visits) / (float)child._visits));
				if (num2 > num)
				{
					treeNodeTablut = child;
					num = num2;
				}
			}
			if (treeNodeTablut != null && treeNodeTablut._boardState.PawnInformation == null)
			{
				BoardGameAITablut.Board.UndoMove(ref treeNodeTablut._parent._boardState);
				BoardGameAITablut.Board.AIMakeMove(treeNodeTablut.OpeningMove);
				treeNodeTablut._boardState = BoardGameAITablut.Board.TakeBoardSnapshot();
			}
		}
		return treeNodeTablut;
	}

	private ExpandResult Expand()
	{
		ExpandResult result = ExpandResult.NeedsToBeSimulated;
		if (_depth < MaxDepth)
		{
			BoardGameAITablut.Board.UndoMove(ref _boardState);
			switch (BoardGameAITablut.Board.CheckGameState())
			{
			case BoardGameTablut.State.InProgress:
			{
				BoardGameSide side = ((_side == BoardGameSide.Player) ? BoardGameSide.AI : BoardGameSide.Player);
				List<List<Move>> moves = BoardGameAITablut.Board.CalculateAllValidMoves(side);
				int totalMovesAvailable = BoardGameAITablut.Board.GetTotalMovesAvailable(ref moves);
				if (totalMovesAvailable > 0)
				{
					_children = new List<TreeNodeTablut>(totalMovesAvailable);
					{
						foreach (List<Move> item in moves)
						{
							foreach (Move item2 in item)
							{
								TreeNodeTablut treeNodeTablut = new TreeNodeTablut(side, _depth + 1);
								treeNodeTablut.OpeningMove = item2;
								treeNodeTablut._parent = this;
								_children.Add(treeNodeTablut);
							}
						}
						return result;
					}
				}
				Debug.FailedAssert("No available moves left but the game is in progress", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\AI\\TreeNodeTablut.cs", "Expand", 203);
				break;
			}
			case BoardGameTablut.State.AIWon:
				result = ExpandResult.AIWon;
				break;
			default:
				result = ExpandResult.PlayerWon;
				break;
			case BoardGameTablut.State.Aborted:
				break;
			}
		}
		return result;
	}

	private BoardGameTablut.State Simulate(ref TreeNodeTablut tn)
	{
		BoardGameAITablut.Board.UndoMove(ref tn._boardState);
		BoardGameTablut.State state = BoardGameAITablut.Board.CheckGameState();
		BoardGameSide side = tn._side;
		while (state == BoardGameTablut.State.InProgress)
		{
			List<PawnBase> list = ((tn._side == BoardGameSide.Player) ? BoardGameAITablut.Board.PlayerOneUnits : BoardGameAITablut.Board.PlayerTwoUnits);
			int count = list.Count;
			PawnBase pawnBase = null;
			bool flag = false;
			int num = 3;
			do
			{
				pawnBase = list[MBRandom.RandomInt(count)];
				flag = BoardGameAITablut.Board.HasAvailableMoves(pawnBase as PawnTablut);
				num--;
			}
			while (!flag && num > 0);
			if (!flag)
			{
				pawnBase = list.OrderBy((PawnBase x) => MBRandom.RandomInt()).FirstOrDefault((PawnBase x) => BoardGameAITablut.Board.HasAvailableMoves(x as PawnTablut));
				flag = pawnBase != null;
			}
			if (flag)
			{
				Move randomAvailableMove = BoardGameAITablut.Board.GetRandomAvailableMove(pawnBase as PawnTablut);
				BoardGameAITablut.Board.AIMakeMove(randomAvailableMove);
				state = BoardGameAITablut.Board.CheckGameState();
			}
			else
			{
				state = ((tn._side != 0) ? BoardGameTablut.State.PlayerWon : BoardGameTablut.State.AIWon);
			}
			tn._side = ((tn._side == BoardGameSide.Player) ? BoardGameSide.AI : BoardGameSide.Player);
		}
		tn._side = side;
		return state;
	}

	private void BackPropagate(BoardGameSide winner)
	{
		for (TreeNodeTablut treeNodeTablut = this; treeNodeTablut != null; treeNodeTablut = treeNodeTablut._parent)
		{
			treeNodeTablut._visits++;
			if (winner == treeNodeTablut._side)
			{
				treeNodeTablut._wins++;
			}
		}
	}
}
