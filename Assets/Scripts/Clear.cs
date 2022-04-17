using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear : MonoBehaviour {
    Game game;
    Grid gridScript;

    // Start is called before the first frame update
    void Start() {
        game = GameObject.Find("Game").GetComponent<Game>();
        gridScript = GameObject.Find("Grid").GetComponent<Grid>();
    }

    public void clearClick() {
        for (int y = 0; y < gridScript.height; y++) {
            for (int x = 0; x < gridScript.width; x++) {
                gridScript.grid[y, x].piece.updatePiece(0, 2, true);
            }
        }
        game.reset();
    }
}
