using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    [System.Serializable]
    public class Moves {
        public int[] xc;
        public int[] yc;
        public List<(int x, int y)> change;
        public int maxMovePerTurn;
    }

    Tile[,] grid;
    GameObject promotion;
    Turn turn;
    int width;
    int height;
    public int promotionType;
    public Moves[] pieceMoves;
    public int[] xCheck;
    public int[] yCheck;
    public int[] moveAmount;
    public (int x, int y)[] kingLocation;
    public (int x, int y) previousPiece;
    bool legalResult = false;
    List<(int x, int y, int type, int player, bool moved, bool pawnTwo)> processedMoves;

    // Start is called before the first frame update
    void Start() {
        Grid gridScript = GameObject.Find("Grid").GetComponent<Grid>();
        promotion = GameObject.Find("Promotion");
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        grid = gridScript.grid;
        width = gridScript.width;
        height = gridScript.height;
        previousPiece = (-1, -1);
        for (int i = 0; i < pieceMoves.Length; i++) {
            pieceMoves[i].change = new List<(int x, int y)>();
            for (int j = 0; j < pieceMoves[i].xc.Length; j++) {
                pieceMoves[i].change.Add((pieceMoves[i].xc[j], pieceMoves[i].yc[j]));
            }
        }
    }

    public void generateValidMoves(int x, int y) {
        Piece cur = grid[y, x].piece;
        if (cur.player == 2) return;
        foreach ((int x, int y) c in pieceMoves[cur.type].change) {
            int nx = x;
            int ny = y;
            int dy = c.y * (cur.player == 1 ? -1 : 1);
            for (int moves = 1; moves <= pieceMoves[cur.type].maxMovePerTurn; moves++) {
                nx += c.x;
                ny += dy;
                if (nx < 0 || ny < 0 || nx >= width || ny >= height) break;
                if (grid[ny, nx].piece.player < 2) {
                    if (cur.type != 5 && grid[ny, nx].piece.player != cur.player) {
                        cur.validMoves.Add((nx, ny, -1, -1, -1, -1));
                        grid[ny, nx].attacked[cur.player]++;
                    }
                    break;
                }
                cur.validMoves.Add((nx, ny, -1, -1, -1, -1));
                grid[ny, nx].attacked[cur.player]++;
            }
        }
        if (cur.type == 0) {
            if (!cur.moved) {
                for (int dir = -1; dir <= 1; dir += 2) {
                    bool work = false;
                    int loc = x + dir;
                    for (; loc >= 0 && loc < width; loc += dir) {
                        if (grid[y, loc].piece.player < 2) {
                            if (grid[y, loc].piece.player == cur.player && grid[y, loc].piece.type == 4 && !grid[y, loc].piece.moved) work = true;
                            break;
                        }
                    }
                    if (work == false || Math.Abs(loc - x) < 3) continue;
                    cur.validMoves.Add((x + dir * 2, y, loc, y, x + dir, y));
                    grid[y, x + dir * 2].attacked[cur.player]++;
                }
            }
        } else if (cur.type == 5) {
            int dy = pieceMoves[cur.type].change[0].y * (cur.player == 1 ? -1 : 1);
            if (!cur.moved) {
                bool work = true;
                int ny = y;
                for (int moves = 1; moves <= 2; moves++) {
                    ny += dy;
                    if (ny < 0 || ny >= height || grid[ny, x].piece.player < 2) {
                        work = false;
                        break;
                    }
                }
                if (work) {
                    cur.validMoves.Add((x, ny, -1, -1, -1, -1));
                    grid[ny, x].attacked[cur.player]++;
                }
            }
            for (int dx = -1; dx <= 1; dx += 2) {
                int nx = x + dx, ny = y + dy;
                if (nx < 0 || nx >= width || ny < 0 || ny >= height) continue;
                if (grid[ny, nx].piece.player < 2) {
                    if (grid[ny, nx].piece.player != cur.player) {
                        cur.validMoves.Add((nx, ny, -1, -1, -1, -1));
                        grid[ny, nx].attacked[cur.player]++;
                    }
                } else {
                    if (grid[y, nx].piece.type == 5 && grid[y, nx].piece.player != cur.player && grid[y, nx].piece.pawnTwo) {
                        cur.validMoves.Add((nx, ny, -(2 + 1), 0, nx, y));
                        grid[ny, nx].attacked[cur.player]++;
                    }
                }
            }
        }
    }

    public void removeMoves(int x, int y) {
        foreach ((int x, int y, int x2, int y2, int x3, int y3) a in grid[y, x].piece.validMoves) {
            grid[a.y, a.x].attacked[grid[y, x].piece.player]--;
        }
        grid[y, x].piece.validMoves.Clear();
    }

    public void reset() {
        previousPiece = (-1, -1);
        for (int i = 0; i < kingLocation.Length; i++) {
            kingLocation[i] = (-1, -1);
        }
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (grid[y, x].piece.pawnTwo) {
                    previousPiece = (x, y);
                }
                if (grid[y, x].piece.player < 2 && grid[y, x].piece.type == 0) {
                    kingLocation[grid[y, x].piece.player] = (x, y);
                }
            }
        }
        generateAllValidMoves();
    }

    public void generateAllValidMoves() {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                grid[y, x].attacked[0] = 0;
                grid[y, x].attacked[1] = 0;
                grid[y, x].piece.validMoves.Clear();
            }
        }
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (grid[y, x].piece.player < 2) {
                    generateValidMoves(x, y);
                }
            }
        }
    }

    public IEnumerator processMove(List<(int x, int y, int x2, int y2)> moves, int type, bool askPromotion, Renderer render, Color color) {//type = 1 -> checkLegality, type = 2 -> making move, type = 3 -> getting move
        processedMoves = new List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>();
        foreach ((int x, int y, int x2, int y2) a in moves) {
            if (a.x2 < 0) continue;
            if (a.x >= 0) {
                if (askPromotion && grid[a.y, a.x].piece.type == 5 && (a.y2 + pieceMoves[5].change[0].y * (grid[a.y, a.x].piece.player == 1 ? -1 : 1) < 0 || a.y2 + pieceMoves[5].change[0].y * (grid[a.y, a.x].piece.player == 1 ? -1 : 1) >= height)) {
                    promotionType = grid[a.y, a.x].piece.player;
                    promotion.SetActive(true);
                    yield return new WaitWhile(() => promotion.activeSelf);
                    processedMoves.Add((a.x2, a.y2, promotionType, grid[a.y, a.x].piece.player, true, false));
                } else {
                    processedMoves.Add((a.x2, a.y2, grid[a.y, a.x].piece.type, grid[a.y, a.x].piece.player, true, (grid[a.y, a.x].piece.type == 5 && Math.Abs(a.y2 - a.y) == 2 && !grid[a.y, a.x].piece.moved)));
                }
                processedMoves.Add((a.x, a.y, 0, 2, true, false));
            } else {
                processedMoves.Add((a.x2, a.y2, a.y, -(a.x + 1), true, false));
            }
        }
        if (type == 1) {
            if (checkLegality(processedMoves, grid[moves[0].y, moves[0].x].piece.player)) {
                if (render) render.material.color = color;
                legalResult = true;
            } else legalResult = false;
        } else if (type == 2) {
            makeMoves(processedMoves, true, true);
            render.material.color = color;
        }
    }

    public void makeMoves(List<(int x, int y, int type, int player, bool moved, bool pawnTwo)> moves, bool moveSprite, bool setPrevious) {

        HashSet<(int x, int y)> effectedCells = new HashSet<(int x, int y)>();
        Action<(int x, int y)> processAround = cur => {
            effectedCells.Add((cur.x, cur.y));
            for (int i = 0; i < xCheck.Length; i++) {
                int nx = cur.x;
                int ny = cur.y;
                for (int moves = 1; moves <= moveAmount[i]; moves++) {
                    nx += xCheck[i];
                    ny += yCheck[i];
                    if (nx < 0 || ny < 0 || nx >= width || ny >= height) break;
                    if (grid[ny, nx].piece.player < 2) {
                        effectedCells.Add((nx, ny));
                        break;
                    }
                }
            }
        };

        if (setPrevious && previousPiece.x >= 0 && grid[previousPiece.y, previousPiece.x].piece.type == 5 && grid[previousPiece.y, previousPiece.x].piece.pawnTwo) {
            grid[previousPiece.y, previousPiece.x].piece.pawnTwo = false;
            processAround(previousPiece);
        }

        foreach ((int x, int y, int type, int player, bool moved, bool pawnTwo) a in moves) {
            processAround((a.x, a.y));
        }

        foreach ((int x, int y) a in effectedCells) {
            removeMoves(a.x, a.y);
        }

        foreach ((int x, int y, int type, int player, bool moved, bool pawnTwo) a in moves) {
            if (a.player < 2 && grid[a.y, a.x].piece.player < 2 && grid[a.y, a.x].piece.type == 0) {
                kingLocation[grid[a.y, a.x].piece.player] = (-1, -1);
            }
            grid[a.y, a.x].piece.updatePiece(a.type, a.player, moveSprite, a.moved, a.pawnTwo);
            if (a.type == 0 && a.player < 2) {
                kingLocation[a.player] = (a.x, a.y);
            }
        }

        foreach ((int x, int y) a in effectedCells) {
            generateValidMoves(a.x, a.y);
        }

        if (setPrevious && moves.Count > 0) {
            previousPiece = (moves[0].x, moves[0].y);
        }

    }

    public bool checkLegality(List<(int x, int y, int type, int player, bool moved, bool pawnTwo)> moves, int player) {
        if (moves.Count > 2 && moves[0].type == 0 && moves[2].type == 4) {
            for (int i = Math.Min(moves[0].x, moves[1].x); i <= Math.Max(moves[0].x, moves[1].x); i++) {
                if (moves[0].y + (player == 0 ? -1 : 1) >= 0 && moves[0].y + (player == 0 ? -1 : 1) < height) {
                    for (int offset = -1; offset <= 1; offset++) {
                        if (i + offset >= 0 && i + offset < width) {
                            if (grid[moves[0].y + (player == 0 ? -1 : 1), i + offset].piece.type == 5 && grid[moves[0].y + (player == 0 ? -1 : 1), i + offset].piece.player != player) {
                                return false;
                            }
                        }
                    }
                }
                if (grid[moves[0].y, i].attacked[(player == 1 ? 0 : 1)] > 0) {
                    return false;
                }
            }
        }
        List<(int x, int y, int type, int player, bool moved, bool pawnTwo)> fixMoves = new List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>();
        foreach ((int x, int y, int type, int player, bool moved, bool pawnTwo) a in moves) {
            fixMoves.Add((a.x, a.y, grid[a.y, a.x].piece.type, grid[a.y, a.x].piece.player, grid[a.y, a.x].piece.moved, grid[a.y, a.x].piece.pawnTwo));
        }
        makeMoves(moves, false, false);
        bool legal = (kingLocation[player].x == -1 || grid[kingLocation[player].y, kingLocation[player].x].attacked[(player == 1 ? 0 : 1)] == 0);
        makeMoves(fixMoves, false, false);
        return legal;
    }

    public bool checkLegalMoves(int player) {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (grid[y, x].piece.player == player) {
                    for (int i = 0; i < grid[y, x].piece.validMoves.Count; i++) {
                        (int x, int y, int x2, int y2, int x3, int y3) a = grid[y, x].piece.validMoves[i];
                        List<(int x, int y, int x2, int y2)> toPass = new List<(int x, int y, int x2, int y2)>();
                        toPass.Add((x, y, a.x, a.y));
                        toPass.Add((a.x2, a.y2, a.x3, a.y3));
                        StartCoroutine(processMove(toPass, 1, false, null, Color.white));
                        if (legalResult) return true;
                    }
                }
            }
        }
        return false;
    }

    public bool inCheck(int player) {
        return (kingLocation[player].x != -1) && (grid[kingLocation[player].y, kingLocation[player].x].attacked[(player == 1 ? 0 : 1)] > 0);
    }

    public List<List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>> getValidMoves(int player) {
        List<List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>> moves = new List<List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>>();
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (grid[y, x].piece.player == player) {
                    foreach ((int x, int y, int x2, int y2, int x3, int y3) a in grid[y, x].piece.validMoves) {
                        List<(int x, int y, int x2, int y2)> toPass = new List<(int x, int y, int x2, int y2)>();
                        toPass.Add((x, y, a.x, a.y));
                        toPass.Add((a.x2, a.y2, a.x3, a.y3));
                        StartCoroutine(processMove(toPass, 3, false, null, Color.white));
                        if (grid[y, x].piece.type == 5 && (a.y + pieceMoves[5].change[0].y * (grid[y, x].piece.player == 1 ? -1 : 1) < 0 || a.y + pieceMoves[5].change[0].y * (grid[y, x].piece.player == 1 ? -1 : 1) >= height)) {
                            for (int i = 1; i < pieceMoves.Length - 1; i++) {
                                (int x, int y, int type, int player, bool moved, bool pawnTwo) curMove = processedMoves[0];
                                curMove.type = i;
                                processedMoves[0] = curMove;
                                moves.Add(new List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>(processedMoves));
                            }
                        } else moves.Add(new List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>(processedMoves));
                    }
                }
            }
        }
        return moves;
    }

    public List<List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>> getLegalMoves(int player) {
        List<List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>> moves = new List<List<(int x, int y, int type, int player, bool moved, bool pawnTwo)>>();
        foreach (List<(int x, int y, int type, int player, bool moved, bool pawnTwo)> move in getValidMoves(player)) {
            if (checkLegality(move, player)) {
                moves.Add(move);
            }
        }
        return moves;
    }

}
