using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEATRenderer : MonoBehaviour
{
    public static void DrawGenome(Genome genome, Vector3 position, Vector2 dimensions, float nodeSize = 0.5f) {

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

        print(sensors.Count);
        print(outputs.Count);
        print(hiddens.Count);

        Dictionary<int, GameObject> drawnNodes = new Dictionary<int, GameObject>();

        // draw sensors
        for (int i = 0; i < sensors.Count; i++) {
            GameObject newNode = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newNode.transform.position = new Vector3(position.x - 0.5f * dimensions.x + i * (dimensions.x / (sensors.Count - 1)),
                                                    position.y - 0.5f * dimensions.y, 0);
            newNode.transform.localScale = Vector3.one * nodeSize;

            drawnNodes.Add(i + 1, newNode);
        }

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

            drawnNodes.Add(i + sensors.Count + 1, newNode);
        }

        // draw hidden
        List<List<NodeGene>> hiddenLayers = new List<List<NodeGene>>();
        System.Random rand = new System.Random();
        for (int i = 0; i < hiddens.Count; i++) {
            GameObject newNode = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newNode.transform.position = new Vector3(position.x + ((float)rand.NextDouble() * 2 - 1) * dimensions.x / 2 , position.y - 0.5f * dimensions.y + (i + 1) * (dimensions.y / (hiddens.Count + 2)), 0);

            newNode.transform.localScale = Vector3.one * nodeSize;
            drawnNodes.Add(i + sensors.Count + outputs.Count + 1, newNode);
        }

        // draw connections
        foreach (ConnectionGene connection in genome.GetConnections().Values) {

            if (connection.getExpressed()) {
                GameObject newConnection = GameObject.CreatePrimitive(PrimitiveType.Quad);
                newConnection.transform.position = (drawnNodes[connection.getInNode()].transform.position + drawnNodes[connection.getOutNode()].transform.position) / 2f;

                newConnection.transform.localScale = new Vector3 (0.05f, Vector3.Distance(drawnNodes[connection.getInNode()].transform.position, drawnNodes[connection.getOutNode()].transform.position), 1);
                

                newConnection.transform.rotation = Quaternion.Euler(0,0,90-(Mathf.Atan2(drawnNodes[connection.getOutNode()].transform.position.y - drawnNodes[connection.getInNode()].transform.position.y,
                                                                                                drawnNodes[connection.getOutNode()].transform.position.x - drawnNodes[connection.getInNode()].transform.position.x )) * Mathf.Rad2Deg * -1);

                
                if (connection.getWeight() < 0f) {
                    newConnection.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Color");
                    newConnection.GetComponent<MeshRenderer>().material.color = Color.grey;
                }
            }
        }
    }
}
