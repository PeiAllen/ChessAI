using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
    public GameObject self;
    public Piece piece;
    public int[] attacked;
    public Tile() {
        attacked = new int[2];
    }
}