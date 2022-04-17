using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece {

    public int type, player;
    public bool pawnTwo, moved;
    public GameObject self;
    public Sprite[,] pieceSprite;
    public List<(int x, int y, int x2, int y2, int x3, int y3)> validMoves;
    //  x2, y2 should be moved to x3 y3
    //  if movement, all >= 0
    //  if insertion, x2 y2 will be -(piececolor+1) piecenumber and x3,y3 >= 0
    //  if deletion, insertino with piececolor=2, piecenumber=0
    //  if nothing, all -1

    public Piece(GameObject a, int pieceType, int playerNumber, Sprite sprite) {
        self = a;
        type = pieceType;
        player = playerNumber;
        moved = false;
        pawnTwo = false;
        self.GetComponent<SpriteRenderer>().sprite = sprite;
        validMoves = new List<(int x, int y, int x2, int y2, int x3, int y3)>();
    }
    public void updatePiece(int pieceType, int playerNumber, bool changeSprite, bool move = true, bool pawnTwoed = false) {
        type = pieceType;
        player = playerNumber;
        moved = move;
        pawnTwo = pawnTwoed;
        if (changeSprite) self.GetComponent<SpriteRenderer>().sprite = pieceSprite[playerNumber, pieceType];
    }
}

