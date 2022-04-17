using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mode : MonoBehaviour {

    public GameObject pieceSquare;
    public (int type, int player) chosen;
    public int topOffset;
    public int mode;
    public int[] xloc;
    GameObject[,] pieces;
    GameObject clear;
    GameObject delete;
    GameObject fen;
    GameObject fenText;
    GameObject perft;
    GameObject perftDepth;
    GameObject perftResult;
    Sprite[,] pieceSprite;
    TMPro.TextMeshPro result;
    GameObject promotion;
    Grid grid;


    // Start is called before the first frame update
    void Start() {
        result = GameObject.Find("Result").GetComponent<TMPro.TextMeshPro>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        promotion = GameObject.Find("Promotion");
        clear = GameObject.Find("Clear");
        delete = GameObject.Find("Delete");
        fen = GameObject.Find("Fen");
        fenText = GameObject.Find("FenText");
        perft = GameObject.Find("Perft");
        perftDepth = GameObject.Find("PerftDepth");
        perftResult = GameObject.Find("PerftResult");
        delete.GetComponent<Delete>().setup(2, 0);
        clear.SetActive(false);
        delete.SetActive(false);
        fen.SetActive(false);
        fenText.SetActive(false);
        perft.SetActive(false);
        perftDepth.SetActive(false);
        perftResult.SetActive(false);
        pieceSprite = GameObject.Find("Pieces").GetComponent<Pieces>().pieceSprite;
        pieces = new GameObject[pieceSprite.GetLength(0) - 1, pieceSprite.GetLength(1)];
        for (int i = 0; i < pieceSprite.GetLength(0) - 1; i++) {
            for (int j = 0; j < pieceSprite.GetLength(1); j++) {
                pieces[i, j] = Instantiate(pieceSquare, new Vector3((float)xloc[i] / 100, (float)(topOffset - 100 * j) / 100, 0), Quaternion.identity);
                pieces[i, j].GetComponent<SpriteRenderer>().sprite = pieceSprite[i, j];
                pieces[i, j].AddComponent<EditClick>();
                pieces[i, j].GetComponent<EditClick>().setup(i, j);
                pieces[i, j].SetActive(false);
            }
        }
        mode = 0;
    }

    public void modeDrop(int newMode) {
        if (promotion.activeSelf || grid.selectedX != -1) return;
        result.text = "";
        mode = newMode;
        clear.SetActive(newMode == 2);
        delete.SetActive(newMode == 2);
        fen.SetActive(newMode == 3);
        fenText.SetActive(newMode == 3);
        perft.SetActive(newMode == 4);
        perftDepth.SetActive(newMode == 4);
        perftResult.SetActive(newMode == 4);
        if (newMode == 4) {
            perftResult.GetComponent<Text>().text = "";
        }
        for (int i = 0; i < pieceSprite.GetLength(0) - 1; i++) {
            for (int j = 0; j < pieceSprite.GetLength(1); j++) {
                pieces[i, j].SetActive(newMode == 2);
            }
        }
        if (newMode == 2) {
            chosen = (0, 2);
        }
    }
}
