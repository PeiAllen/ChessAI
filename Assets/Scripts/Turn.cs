using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour {

    UnityEngine.UI.Text turnText;
    Grid grid;
    GameObject promotion;
    public int player;


    // Start is called before the first frame update
    void Start() {
        promotion = GameObject.Find("Promotion");
        turnText = GameObject.Find("TurnText").GetComponent<UnityEngine.UI.Text>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        makePlayer(0);
    }

    // Update is called once per frame
    public void makePlayer(int p) {
        Debug.Log(p);
        player = p;
        turnText.text = (player == 0 ? "White's" : "Black's") + " Turn";
    }
    public void turnClick() {
        if (promotion.activeSelf || grid.selectedX != -1) return;
        makePlayer((player == 1 ? 0 : 1));
    }
}
