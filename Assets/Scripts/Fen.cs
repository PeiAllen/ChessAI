using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fen : MonoBehaviour {

    Game game;
    InputField fenText;
    Grid grid;
    Turn turn;
    Dictionary<char, int> convert = new Dictionary<char, int>();

    void Start() {
        fenText = GameObject.Find("FenText").GetComponent<InputField>();
        game = GameObject.Find("Game").GetComponent<Game>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        convert['p'] = 5;
        convert['n'] = 3;
        convert['b'] = 2;
        convert['r'] = 4;
        convert['q'] = 1;
        convert['k'] = 0;
    }


    public void processFen(string fen) {
        int ptr = 0;
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; ptr++) {
                if (Char.IsDigit(fen[ptr])) {
                    for (int cur = fen[ptr] - '0'; cur > 0; cur--, j++) {
                        grid.grid[i, j].piece.updatePiece(0, 2, true);
                    }
                } else {
                    grid.grid[i, j++].piece.updatePiece(convert[Char.ToLower(fen[ptr])], (Char.IsUpper(fen[ptr]) ? 0 : 1), true, !(Char.ToLower(fen[ptr]) == 'p' && i == (Char.IsUpper(fen[ptr]) ? 6 : 1)));
                }
            }
            ptr++;
        }
        Debug.Log(fen[ptr]);
        turn.makePlayer(fen[ptr] == 'w' ? 0 : 1);
        ptr += 2;
        if (fen[ptr] != '-') {
            for (; fen[ptr] != ' '; ptr++) {
                grid.grid[(Char.IsUpper(fen[ptr]) ? 7 : 0), 4].piece.moved = false;
                grid.grid[(Char.IsUpper(fen[ptr]) ? 7 : 0), (Char.ToLower(fen[ptr]) == 'k' ? 7 : 0)].piece.moved = false;
            }
            ptr++;
        } else ptr += 2;
        if (fen[ptr] != '-') {
            grid.grid[(fen[ptr + 1] == 3 ? 4 : 3), fen[ptr] - 'a'].piece.pawnTwo = true;
            ptr += 3;
        } else ptr += 2;
        game.reset();
    }

    public void fenClick() {
        processFen(fenText.text);
    }

}
