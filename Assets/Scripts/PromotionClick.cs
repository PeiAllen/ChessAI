using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionClick : MonoBehaviour {

    GameObject promotion;
    Promotion promotionScript;
    Game game;
    int type;

    // Start is called before the first frame update
    public void setup(int t) {
        type = t;
        promotion = GameObject.Find("Promotion");
        promotionScript = promotion.GetComponent<Promotion>();
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    void OnMouseUpAsButton() {
        game.promotionType = type;
        for (int i = 0; i < promotionScript.pieces.Count; i++) {
            if (i + 1 != type) {
                promotionScript.pieces[i].SetActive(false);
            }
        }
        promotion.SetActive(false);
        gameObject.SetActive(false);
    }

}
