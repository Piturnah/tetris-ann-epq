using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NodeGene
{
    public enum TYPE
    {
        SENSOR,
        HIDDEN,
        OUTPUT
    }

    TYPE type;
    int id;
    bool isActive;
    List<NodeGene> directInNodes;

    public int CalculateDstFromSensor() {
        if (directInNodes.Any()) {
            int maxInInNodes = 0;

            foreach (NodeGene node in directInNodes) {
                int dst = node.CalculateDstFromSensor();
                if (dst > maxInInNodes) {
                    maxInInNodes = dst;
                }
            }

            return 1 + maxInInNodes;
        }

        return 0;
    }

    public void AddInNode(NodeGene node) {
        if (!directInNodes.Contains(node)) {
            directInNodes.Add(node);
        }
    }

    public void RemoveInNode(NodeGene node) {
        directInNodes.Remove(node);
    }

    public NodeGene(TYPE type, int id)
    {
        this.type = type;
        this.id = id;

        this.directInNodes = new List<NodeGene>();
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
        return new NodeGene(type, id);
    }

    public void SetActive(bool activity) {
        isActive = activity;
    }

    public bool GetActive() {
        return isActive;
    }
}
