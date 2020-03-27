using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NEATRenderer : MonoBehaviour
{
    public NEATRenderer() {

    }

    public void DrawGenome(Genome genome, Vector3 position, Vector2 dimensions, float nodeSize = 0.5f) {

        List<NodeGene> sensors = new List<NodeGene>();
        List<NodeGene> outputs = new List<NodeGene>();
        List<NodeGene> hiddens = new List<NodeGene>();

        // get quantities of nodes
        foreach (NodeGene node in genome.GetNodes().Values) {

            switch (node.getType()) {
                case NodeGene.TYPE.SENSOR:
                    sensors.Add(node); break;
                case NodeGene.TYPE.OUTPUT:
                    outputs.Add(node); break;
                case NodeGene.TYPE.HIDDEN:
                    hiddens.Add(node); break;
                default:
                    Debug.LogWarning("Invalid type!"); break;
            }
        }

        Dictionary<int, GameObject> drawnNodes = new Dictionary<int, GameObject>();

        Texture2D network = new Texture2D(Mathf.RoundToInt(dimensions.x * 100), Mathf.RoundToInt(dimensions.y * 100));
        network.filterMode = FilterMode.Point;
        network.wrapMode = TextureWrapMode.Clamp;

        Color[] newPixels = new Color[network.GetPixels().Length];
        for (int i = 0; i < newPixels.Length; i++) {
            newPixels[i] = Color.white;
        }
        network.SetPixels(newPixels);
        network.Apply();


        //GameObject quadDisplay = GameObject.CreatePrimitive(PrimitiveType.Quad);
        //quadDisplay.transform.position = position;
        //quadDisplay.transform.localScale = dimensions;
        //quadDisplay.GetComponent<Renderer>().material = new Material (Shader.Find("Unlit/Texture"));
        //quadDisplay.GetComponent<Renderer>().sharedMaterial.mainTexture = network;

        GameObject parentObj = Instantiate(new GameObject("ParentObj"));

        // draw sensors
        for (int i = 0; i < sensors.Count; i++) {
            GameObject newNode = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newNode.transform.position = new Vector3(position.x - 0.5f * dimensions.x + i * (dimensions.x / (sensors.Count - 1)),
                                                    position.y - 0.5f * dimensions.y, 0);
            newNode.transform.localScale = Vector3.one * nodeSize;
            newNode.isStatic = true;

            drawnNodes.Add(i + 1, newNode);
            newNode.transform.parent = parentObj.transform;
        }
    
        network.Apply();

        // draw outputs
        for (int i = 0; i < outputs.Count; i++) {
            GameObject newNode = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (outputs.Count != 1) {
                newNode.transform.position = new Vector3(position.x - 0.5f * dimensions.x + i * (dimensions.x / (outputs.Count - 1)),
                                                        position.y + 0.5f * dimensions.y, 0);
            } else {
                newNode.transform.position = new Vector3(position.x, position.y + 0.5f * dimensions.y, 0);
            }
            newNode.transform.localScale = Vector3.one * nodeSize;
            newNode.isStatic = true;

            drawnNodes.Add(i + sensors.Count + 1, newNode);
            newNode.transform.parent = parentObj.transform;
        }

        // sort hidden
        Dictionary<int, List<NodeGene>> hiddenMap = new Dictionary<int, List<NodeGene>>();
        foreach (NodeGene hidden in hiddens) {
            int dstFromSensor = hidden.CalculateDstFromSensor();
            if (hiddenMap.ContainsKey(dstFromSensor)) {
                hiddenMap[dstFromSensor].Add(hidden);
            } else {
                hiddenMap.Add(dstFromSensor, new List<NodeGene> {hidden});
            }
        }

        // draw hidden
        foreach (KeyValuePair<int, List<NodeGene>> layer in hiddenMap) {
            for (int i = 0; i < layer.Value.Count; i++) {
                GameObject newNode = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newNode.transform.position = new Vector3(position.x - 0.5f * dimensions.x + (i + 1) * (dimensions.x / (layer.Value.Count + 1)),
                                                        position.y - 0.5f * dimensions.y + (layer.Key) * (dimensions.y / (hiddenMap.Count + 1)), 0);

                newNode.transform.localScale = Vector3.one * nodeSize;
                newNode.isStatic = true;

                drawnNodes.Add(layer.Value[i].getId(), newNode);
                newNode.transform.parent = parentObj.transform;
            }
        }

        // draw connections
        foreach (ConnectionGene connection in genome.GetConnections().Values) {

            if (connection.getExpressed()) {
                GameObject newConnection = GameObject.CreatePrimitive(PrimitiveType.Quad);
                newConnection.transform.position = (drawnNodes[connection.getInNode()].transform.position + drawnNodes[connection.getOutNode()].transform.position) / 2f;

                newConnection.transform.localScale = new Vector3 (0.05f, Vector3.Distance(drawnNodes[connection.getInNode()].transform.position, drawnNodes[connection.getOutNode()].transform.position), 1);
                

                newConnection.transform.rotation = Quaternion.Euler(0,0,90-(Mathf.Atan2(drawnNodes[connection.getOutNode()].transform.position.y - drawnNodes[connection.getInNode()].transform.position.y,
                                                                                                drawnNodes[connection.getOutNode()].transform.position.x - drawnNodes[connection.getInNode()].transform.position.x )) * Mathf.Rad2Deg * -1);

                newConnection.isStatic = true;

                if (connection.getWeight() < 0f) {
                    newConnection.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Color");
                    newConnection.GetComponent<MeshRenderer>().material.color = Color.grey;
                }

                newConnection.transform.parent = parentObj.transform;
            }
        }
    }

    public static Texture2D DrawCircle(Texture2D texture, Color colour, int x, int y, int radius) {
        float rSquared = radius * radius;

        for (int u = x - radius; u < x + radius + 1; u++) {
            for (int v = y - radius; v < y + radius + 1; v++) {
                if (Mathf.Pow(x-u, 2) + Mathf.Pow(y-v, 2) < rSquared) {
                    texture.SetPixel(u, v, colour);
                }
            }
        }
        return texture;
    }

    public static Texture2D DrawLine(Texture2D texture, Color colour, Vector2 p1, Vector2 p2) {
        Vector2 t = p1;
        float frac = 1/Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
        float ctr = 0;

        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y) {
            t = Vector2.Lerp(p1, p2, ctr);
            ctr += frac;
            texture.SetPixel((int)t.x, (int)t.y, colour);
        }
        return texture;
    }
}
