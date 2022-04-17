using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Perft : MonoBehaviour {
    Game game;
    InputField perftDepth;
    Text perftResult;
    Turn turn;
    Tile[,] grid;
    void Start() {
        perftDepth = GameObject.Find("PerftDepth").GetComponent<InputField>();
        perftResult = GameObject.Find("PerftResult").GetComponent<Text>();
        game = GameObject.Find("Game").GetComponent<Game>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        grid = GameObject.Find("Grid").GetComponent<Grid>().grid;
    }

    public long getPerft(int depth, int player) {
        if (depth == 0) {
            return 1;
        }
        long nodes = 0;
        foreach (List<(int x, int y, int type, int player, bool moved, bool pawnTwo)> moves in game.getValidMoves(player)) {
            bool legalMove = true;
            if (moves.Count > 2 && moves[0].type == 0 && moves[2].type == 4) {
                for (int i = Math.Min(moves[0].x, moves[1].x); i <= Math.Max(moves[0].x, moves[1].x) && legalMove; i++) {
                    if (moves[0].y + (player == 0 ? -1 : 1) >= 0 && moves[0].y + (player == 0 ? -1 : 1) < grid.GetLength(0)) {
                        for (int offset = -1; offset <= 1; offset++) {
                            if (i + offset >= 0 && i + offset < grid.GetLength(1)) {
                                if (grid[moves[0].y + (player == 0 ? -1 : 1), i + offset].piece.type == 5 && grid[moves[0].y + (player == 0 ? -1 : 1), i + offset].piece.player != player) {
                                    legalMove = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (grid[moves[0].y, i].attacked[(player == 1 ? 0 : 1)] > 0) {
                        legalMove = false;
                    }
                }
            }
            if (!legalMove) {
                continue;
            }
            (int x, int y) previousPiece = game.previousPiece;
            List<(int x, int y, int type, int player, bool moved, bool pawnTwo)> fixMoves = new List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>();
            foreach ((int x, int y, int type, int player, bool moved, bool pawnTwo) a in moves) {
                fixMoves.Add((a.x, a.y, grid[a.y, a.x].piece.type, grid[a.y, a.x].piece.player, grid[a.y, a.x].piece.moved, grid[a.y, a.x].piece.pawnTwo));
            }
            if (previousPiece.x >= 0 && grid[previousPiece.y, previousPiece.x].piece.type == 5 && grid[previousPiece.y, previousPiece.x].piece.pawnTwo) {
                fixMoves.Add((previousPiece.x, previousPiece.y, grid[previousPiece.y, previousPiece.x].piece.type, grid[previousPiece.y, previousPiece.x].piece.player, grid[previousPiece.y, previousPiece.x].piece.moved, grid[previousPiece.y, previousPiece.x].piece.pawnTwo));
            }
            game.makeMoves(moves, false, true);
            if (game.kingLocation[player].x == -1 || grid[game.kingLocation[player].y, game.kingLocation[player].x].attacked[(player == 1 ? 0 : 1)] == 0) {
                long te = getPerft(depth - 1, (player == 1 ? 0 : 1));
                if (depth == Int32.Parse(perftDepth.text)) {
                    Debug.Log(((char)(moves[1].x + 'a')).ToString() + (8 - moves[1].y).ToString() + " " + ((char)(moves[0].x + 'a')).ToString() + (8 - moves[0].y).ToString() + " " + te);
                }
                nodes += te;
            }
            game.makeMoves(fixMoves, false, false);
            game.previousPiece = previousPiece;
        }
        return nodes;
    }
    public void perftClick() {
        perftResult.text = getPerft(Int32.Parse(perftDepth.text), turn.player).ToString();
    }
}
