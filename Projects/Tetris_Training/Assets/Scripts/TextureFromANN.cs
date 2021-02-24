using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureFromANN : MonoBehaviour
{
    Renderer rend;

    private void Start() {
        rend = GetComponent<Renderer>();

        //DrawTexture(TextureFromNeuralNet(new NeuralNetwork(TetrisNEAT.OpenGenome("/TestGenome3.gen")), 101, 101));
    }

    public void DrawTexture(Texture2D texture) {
        rend.sharedMaterial.mainTexture = texture;
        rend.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

/*    public Texture2D TextureFromNeuralNet (NeuralNetwork neuralNet, int width, int height) {

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float nnValue = neuralNet.GetNNResult(new float[] {1, (float)x/width, (float)y/height})[0];

                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, nnValue);
            }
        }

        return TextureFromColourMap(colourMap, width, height);
    }*/

    public Texture2D TextureFromColourMap (Color[] colourMap, int width, int height) {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }
}
