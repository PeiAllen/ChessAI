using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    // Start is called before the first frame update
    public GameObject tileSquare;
    public GameObject pieceSquare;
    public int width;
    public int height;
    public Tile[,] grid;
    public Color[] gridColor;
    public Color[] selectColor;
    public int selectedX;
    public int selectedY;

    void Start() {
        selectedX = -1;
        selectedY = -1;
        GameObject pieces = GameObject.Find("Pieces");
        Pieces piecesScript = pieces.GetComponent<Pieces>();
        grid = new Tile[height, width];
        double tlx = -(width / 2 - 0.5);
        double tly = height / 2 - 0.5;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                grid[y, x] = new Tile();
                grid[y, x].self = Instantiate(tileSquare, new Vector3((float)tlx + x, (float)tly - y, 0), Quaternion.identity);
                grid[y, x].self.GetComponent<Renderer>().material.color = gridColor[(x + y) % 2];
                grid[y, x].piece = new Piece(Instantiate(pieceSquare, new Vector3((float)tlx + x, (float)tly - y, 0), Quaternion.identity), 0, 2, piecesScript.pieceSprites[piecesScript.pieceCount * 2]);
                grid[y, x].self.AddComponent<TileClick>();
                grid[y, x].self.GetComponent<TileClick>().setup(y, x, gameObject.GetComponent<Grid>());
            }
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
