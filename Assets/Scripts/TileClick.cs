using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClick : MonoBehaviour {
    public int x;
    public int y;
    Grid grid;
    Piece self;
    Game game;
    Mode mode;
    TMPro.TextMeshPro result;
    Turn turn;
    public void setup(int a, int b, Grid g) {
        y = a;
        x = b;
        grid = g;
        self = grid.grid[y, x].piece;
        game = GameObject.Find("Game").GetComponent<Game>();
        mode = GameObject.Find("Mode").GetComponent<Mode>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        result = GameObject.Find("Result").GetComponent<TMPro.TextMeshPro>();
    }
    void OnMouseUpAsButton() {
        if (result.text != "") return;
        if (mode.mode == 0) {
            if (grid.selectedX == -1 && grid.selectedY == -1) {
                if (grid.grid[y, x].piece.player != turn.player) return;
                gameObject.GetComponent<Renderer>().material.color = grid.selectColor[0];
                for (int i = 0; i < grid.grid[y, x].piece.validMoves.Count; i++) {
                    (int x, int y, int x2, int y2, int x3, int y3) a = grid.grid[y, x].piece.validMoves[i];
                    List<(int x, int y, int x2, int y2)> toPass = new List<(int x, int y, int x2, int y2)>();
                    toPass.Add((x, y, a.x, a.y));
                    toPass.Add((a.x2, a.y2, a.x3, a.y3));
                    StartCoroutine(game.processMove(toPass, 1, false, grid.grid[a.y, a.x].self.GetComponent<Renderer>(), grid.selectColor[1]));
                }
                grid.selectedX = x;
                grid.selectedY = y;
            } else if (gameObject.GetComponent<Renderer>().material.color == grid.selectColor[1]) {
                (int x, int y, int x2, int y2, int x3, int y3) selectedMove = (-1, -1, -1, -1, -1, -1);
                foreach ((int x, int y, int x2, int y2, int x3, int y3) a in grid.grid[grid.selectedY, grid.selectedX].piece.validMoves) {
                    grid.grid[a.y, a.x].self.GetComponent<Renderer>().material.color = grid.gridColor[(a.y + a.x) % 2];
                    if (a.x == x && a.y == y) selectedMove = a;
                }
                List<(int x, int y, int x2, int y2)> toPass = new List<(int x, int y, int x2, int y2)>();
                toPass.Add((grid.selectedX, grid.selectedY, x, y));
                toPass.Add((selectedMove.x2, selectedMove.y2, selectedMove.x3, selectedMove.y3));
                StartCoroutine(game.processMove(toPass, 2, true, grid.grid[grid.selectedY, grid.selectedX].self.GetComponent<Renderer>(), grid.gridColor[(grid.selectedX + grid.selectedY) % 2]));
                grid.selectedX = -1;
                grid.selectedY = -1;
                turn.makePlayer(turn.player == 1 ? 0 : 1);
                if (!game.checkLegalMoves(turn.player)) {
                    if (game.inCheck(turn.player)) {
                        result.text = (turn.player == 1 ? "White" : "Black") + " Wins";
                    } else {
                        result.text = "Stalemate";
                    }
                }
            } else {
                grid.grid[grid.selectedY, grid.selectedX].self.GetComponent<Renderer>().material.color = grid.gridColor[(grid.selectedX + grid.selectedY) % 2];
                foreach ((int x, int y, int x2, int y2, int x3, int y3) a in grid.grid[grid.selectedY, grid.selectedX].piece.validMoves) {
                    grid.grid[a.y, a.x].self.GetComponent<Renderer>().material.color = grid.gridColor[(a.y + a.x) % 2];
                }
                grid.selectedX = -1;
                grid.selectedY = -1;
            }
        } else if (mode.mode == 2) {
            List<(int x, int y, int type, int player, bool moved, bool pawnTwo)> moves = new List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>();
            moves.Add((x, y, mode.chosen.type, mode.chosen.player, true, false));
            game.makeMoves(moves, true, true);
            return;
        }
    }
}
