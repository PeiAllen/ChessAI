using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Promotion : MonoBehaviour {
    public GameObject promotionSquare;
    public double spacing;
    bool started = false;
    Game game;
    Sprite[,] pieceSprite;
    public List<GameObject> pieces;
    // Start is called before the first frame update
    void Start() {
        double width = GameObject.Find("Grid").GetComponent<Grid>().width;
        gameObject.transform.localScale = new Vector3((float)width, GameObject.Find("Grid").GetComponent<Grid>().height, 1);
        game = GameObject.Find("Game").GetComponent<Game>();
        pieceSprite = GameObject.Find("Pieces").GetComponent<Pieces>().pieceSprite;
        pieces = new List<GameObject>();
        double size = (width - spacing * (pieceSprite.GetLength(1) - 1)) / (pieceSprite.GetLength(1) - 2);
        double stx = -(width / 2 - size / 2) + spacing;
        for (int i = 1; i < pieceSprite.GetLength(1) - 1; i++) {
            pieces.Add(Instantiate(promotionSquare, new Vector3((float)stx, 0, -2), Quaternion.identity));
            stx += size + spacing;
            pieces[i - 1].transform.localScale = new Vector3((float)size, (float)size, 1);
            pieces[i - 1].AddComponent<PromotionClick>();
            pieces[i - 1].GetComponent<PromotionClick>().setup(i);
        }
        started = true;
        for (int i = 0; i < pieces.Count; i++) {
            pieces[i].SetActive(false);
        }
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void OnEnable() {
        if (started) {
            for (int i = 0; i < pieces.Count; i++) {
                pieces[i].SetActive(true);
                pieces[i].GetComponent<SpriteRenderer>().sprite = pieceSprite[game.promotionType, i + 1];
            }
        }
    }

}
