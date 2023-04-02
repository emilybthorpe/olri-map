using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class BitMapImageGenerator : MonoBehaviour
{
    public ManageCoordinates manageCoordinates; 
    public Texture2D bitmapTexture;
    public int[,] matrix;
    public Image bitmapImage;
    public MapGenScript mapgenerator; 
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

    public void SetMatrix(int[,] mat) {
        this.matrix = mat;
    }

    public void Generate(){
        processArray();
        Debug.Log(this.matrix);
        GenerateBitmap();
        mapgenerator = mapgenerator.GetComponent<MapGenScript>();
        mapgenerator.Map = this.bitmapTexture;
        mapgenerator.PressButon();
        
    }

    public void processArray(){

        int [,] temp = new int[this.matrix.GetLength(0),this.matrix.GetLength(1)];
        temp = this.matrix;
        for (int i = 1; i < this.matrix.GetLength(0) - 1; i++) {
            for (int j = 1; j < this.matrix.GetLength(1) - 1; j++) {
                
                if  (this.matrix[i, j] == 1) { // if current cell is a wall
                    int count = 0;
                    if ( this.matrix[i - 1, j] == -1) count++; // top cell
                    if  (this.matrix[i + 1, j] == -1) count++; // bottom cell
                    if  (this.matrix[i, j - 1] == -1) count++; // left cell
                    if  (this.matrix[i, j + 1] == -1) count++; // right cell
                    if (count >= 2) {
                     temp[i, j] = 3; // set the wall edge to green
                    }
                }
            }
        }
        this.matrix = temp;
        Debug.Log("________");
        Debug.Log(temp[0,1]);
    }
public void SaveTextureToFile(Texture2D texture, string filename)
{
    byte[] bytes = texture.EncodeToPNG();
    File.WriteAllBytes(filename, bytes);
}
    public void GenerateBitmap () {
        int width = this.matrix.GetLength(0);
        int height = this.matrix.GetLength(1);
        Debug.Log("dimensions : " + width + " ,  " + height);
        bitmapTexture = new Texture2D(width, height);
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                Color color = Color.black; // default to blue
                int value = this.matrix[x, y];
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
        SaveTextureToFile(bitmapTexture, "Assets/Resources/myTexture.png");
        // mapgenerator.Map = bitmapTexture; 
        // mapgenerator.PressButon();
        
    }
}
