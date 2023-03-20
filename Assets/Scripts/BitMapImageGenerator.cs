using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BitMapImageGenerator : MonoBehaviour
{
    public Texture2D bitmapTexture;
    public int[,] matrix = new int[10, 10]; // change size as needed
    public Image bitmapImage;

    void Start () {
        // matrix = GenerateMatrix();
        // GenerateBitmap();
        // bitmapImage.sprite = Sprite.Create(bitmapTexture, new Rect(0, 0, bitmapTexture.width, bitmapTexture.height), new Vector2(0.5f, 0.5f));
    }

    // public int[,] GenerateMatrix()
    // {

    //     //read the matrix given by the backend

    //     //get the matrix from backend 
    //     //matrix  = getMatrix() 


    //     //process it for bitmap generation 

    //     //processArray(matrix )

    //     //return the matrix 


    // }

    public void processArray(int[,] room){


        for (int i = 1; i < room.GetLength(0) - 1; i++) {
            for (int j = 1; j < room.GetLength(1) - 1; j++) {
                
                if (room[i, j] == 1) { // if current cell is a wall
                    int count = 0;
                    if (room[i - 1, j] == -1) count++; // top cell
                    if (room[i + 1, j] == -1) count++; // bottom cell
                    if (room[i, j - 1] == -1) count++; // left cell
                    if (room[i, j + 1] == -1) count++; // right cell
                    if (count >= 2) {
                        room[i, j] = 3; // set the wall edge to green
                    }
                }
            }
        }
    }

    void GenerateBitmap () {
        int width = matrix.GetLength(0);
        int height = matrix.GetLength(1);

        bitmapTexture = new Texture2D(width, height);
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                Color color = Color.blue; // default to blue
                int value = matrix[x, y];
                if (value == 0 || value == 2) {
                    color = Color.white;
                } else if (value == 1) {
                    color = Color.red;
                } else if(value == 3){
                    color = Color.green;
                }
                bitmapTexture.SetPixel(x, y, color);
            }
        }

        bitmapTexture.Apply();
    }
}
