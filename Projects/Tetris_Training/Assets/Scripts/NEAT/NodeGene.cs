using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class NodeGene
{
    public enum TYPE
    {
        SENSOR,
        HIDDEN,
        OUTPUT
    }

    public enum ACTIVATION 
    {
        SIGMOID,
        SINE,
        COSINE
    }

    TYPE type;
    ACTIVATION activation;
    int id;
    bool isActive;
    Dictionary<NodeGene, ConnectionGene> directInNodes;

    public float GetActivation(float[] inputs) {
        if (type == TYPE.SENSOR) {
            return inputs[id-1];
        }

        else {
            float summedActivation = 0;
            foreach (NodeGene node in directInNodes.Keys) {
                ConnectionGene connectionIn = directInNodes[node];

                summedActivation += (connectionIn.getExpressed())? node.GetActivation(inputs) * connectionIn.getWeight() : 0;
            }

            if (activation == ACTIVATION.SIGMOID) {
                return History.Sigmoid(summedActivation);
            }
            if (activation == ACTIVATION.SINE) {
                return (float)Math.Sin(summedActivation * 2 * Math.PI);
            }
            if (activation == ACTIVATION.COSINE) {
                return (float)Math.Cos(summedActivation * 2 * Math.PI);
            }
            return summedActivation;
        }
    }

    public int CalculateDstFromSensor() {
        if (directInNodes.Any()) {
            int maxInInNodes = 0;

            foreach (NodeGene node in directInNodes.Keys) {
                int dst = node.CalculateDstFromSensor();
                if (dst > maxInInNodes) {
                    maxInInNodes = dst;
                }
            }

            return 1 + maxInInNodes;
        }

        return 0;
    }

    public void AddInNode(NodeGene node, ConnectionGene connection) {
        if (!directInNodes.Keys.Contains(node)) {
            directInNodes.Add(node, connection);
        }
    }

    public void RemoveInNode(NodeGene node) {
        directInNodes.Remove(node);
    }

    public NodeGene(TYPE type, int id)
    {
        this.type = type;
        this.id = id;

        this.directInNodes = new Dictionary<NodeGene, ConnectionGene>();
        this.isActive = false;
    }

    public NodeGene(TYPE type, int id, ACTIVATION activation) {
        this.type = type;
        this.id = id;
        this.activation = activation;

        this.directInNodes = new Dictionary<NodeGene, ConnectionGene>();
        this.isActive = false;
    }

    public TYPE getType()
    {
        return type;
    }
    public int getId()
    {
        return id;
    }

    public NodeGene CopyNode()
    {
        return new NodeGene(type, id, activation);
    }

    public void SetActive(bool activity) {
        isActive = activity;
    }

    public bool GetActive() {
        return isActive;
    }
}
