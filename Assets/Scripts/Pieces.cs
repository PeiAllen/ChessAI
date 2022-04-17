using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pieces : MonoBehaviour {
    public Sprite[] pieceSprites;
    public Sprite[,] pieceSprite;
    public int pieceCount;
    public int[] pieceTypes;
    Game game;
    Grid gridScript;
    TMPro.TextMeshPro result;
    Turn turn;
    GameObject promotion;

    void Start() {
        game = GameObject.Find("Game").GetComponent<Game>();
        gridScript = GameObject.Find("Grid").GetComponent<Grid>();
        result = GameObject.Find("Result").GetComponent<TMPro.TextMeshPro>();
        promotion = GameObject.Find("Promotion");
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        pieceSprite = new Sprite[(pieceSprites.Length + pieceCount - 1) / pieceCount, pieceCount];
        for (int i = 0; i < pieceSprites.Length; i++) {
            pieceSprite[i / pieceCount, i % pieceCount] = pieceSprites[i];
        }
        for (int y = 0; y < gridScript.height; y++) {
            for (int x = 0; x < gridScript.width; x++) {
                gridScript.grid[y, x].piece.pieceSprite = pieceSprite;
            }
        }
        (int x, int y)[] kingLocation = new (int x, int y)[pieceSprite.GetLength(0) - 1];
        for (int i = 0; i < pieceSprite.GetLength(0) - 1; i++) {
            kingLocation[i] = (-1, -1);
        }
        game.kingLocation = kingLocation;
        reset(true);
    }
    public void reset(bool ignorePromotion) {
        if ((promotion.activeSelf && !ignorePromotion) || gridScript.selectedX != -1) return;
        result.text = "";
        for (int y = 0; y < gridScript.height; y++) {
            for (int x = 0; x < gridScript.width; x++) {
                gridScript.grid[y, x].piece.updatePiece(0, 2, true);
            }
        }
        for (int x = 0; x < gridScript.width; x++) {
            gridScript.grid[0, x].piece.updatePiece(pieceTypes[x], 1, true, false);
            gridScript.grid[gridScript.height - 1, x].piece.updatePiece(pieceTypes[x], 0, true, false);
            gridScript.grid[1, x].piece.updatePiece(5, 1, true, false);
            gridScript.grid[gridScript.height - 2, x].piece.updatePiece(5, 0, true, false);
        }
        turn.makePlayer(0);
        game.reset();
    }
}
