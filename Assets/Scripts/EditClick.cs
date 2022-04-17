using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditClick : MonoBehaviour {
    int type;
    int player;
    Mode mode;

    public void setup(int p, int t) {
        type = t;
        player = p;
        mode = GameObject.Find("Mode").GetComponent<Mode>();
    }
    void OnMouseUpAsButton() {
        mode.chosen = (type, player);
    }
}
