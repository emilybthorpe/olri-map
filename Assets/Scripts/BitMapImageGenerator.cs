using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BitMapImageGenerator : MonoBehaviour
{
    public Texture2D bitmapTexture;
    public int[,] matrix;
    public Image bitmapImage;

    void Start () {
    }

    private int[,] proccesMatrix(char[,] matrix) {
        int[,] intArray = new int[matrix.GetLength(0), matrix.GetLength(1)];
        for(int x = 0; x < matrix.GetLength(0); x++) {
            for(int y = 0; y < matrix.GetLength(1); y++) {
                if(matrix[y,x].Equals('*')) {
                    intArray[y,x] = 3;
                } else {
                    intArray[y,x]= (int) char.GetNumericValue(matrix[y,x]);
                }
            }
        }
        return intArray;
    }

    public void SetMatrix(char[,] matrix) {
        this.matrix = proccesMatrix(matrix);
    }

    public Texture2D Generate(){
        processArray();
        GenerateBitmap();
        return this.bitmapTexture;
    }

    public void processArray(){

        
        for (int i = 1; i < this.matrix.GetLength(0) - 1; i++) {
            for (int j = 1; j < this.matrix.GetLength(1) - 1; j++) {
                
                if  (this.matrix[i, j] == 1) { // if current cell is a wall
                    int count = 0;
                    if ( this.matrix[i - 1, j] == -1) count++; // top cell
                    if  (this.matrix[i + 1, j] == -1) count++; // bottom cell
                    if  (this.matrix[i, j - 1] == -1) count++; // left cell
                    if  (this.matrix[i, j + 1] == -1) count++; // right cell
                    if (count >= 2) {
                     this.matrix[i, j] = 3; // set the wall edge to green
                    }
                }
            }
        }
    }

    public void GenerateBitmap () {
        int width = matrix.GetLength(0);
        int height = matrix.GetLength(1);

        bitmapTexture = new Texture2D(width, height);
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                Color color = Color.black; // default to blue
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
