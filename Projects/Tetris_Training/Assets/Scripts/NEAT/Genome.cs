using System.Collections;
using System.Collections.Generic;
using System;

public class Genome
{
    List<ConnectionGene> connections;
    List<NodeGene> nodes;

    Random rand = new Random();

    public void AddConnectionMutation()
    {
        NodeGene node1 = nodes[rand.Next(nodes.Count)];
        NodeGene node2 = nodes[rand.Next(nodes.Count)];
        float weight = (float)rand.NextDouble() * 2f - 1f;

        if (node2.getType() == NodeGene.TYPE.INPUT || node1.getType() == NodeGene.TYPE.OUTPUT)
        {
            NodeGene tempGene = node1;
            node1 = node2;
            node2 = tempGene;
        }

        bool connectionExists = false; // check if connection already exists
        foreach (ConnectionGene connection in connections)
        {
            if (connection.getInNode() == node1.getId() && connection.getOutNode() == node2.getId())
            {
                connectionExists = true;
            }
        }

        if (!connectionExists)
        {
            ConnectionGene newConnection = new ConnectionGene(node1.getId(), node2.getId(), weight, true, 0);
            connections.Add(newConnection);
        }
    }

    public void AddNodeMutation()
    {
        ConnectionGene connection = connections[rand.Next(connections.Count)];

        NodeGene inNode = nodes[connection.getInNode()];
        NodeGene outNode = nodes[connection.getOutNode()];

        connection.Disable();

        NodeGene newNode = new NodeGene(NodeGene.TYPE.HIDDEN, nodes.Count);
        ConnectionGene inToNew = new ConnectionGene(inNode.getId(), newNode.getId(), 1f, true, 0);
        ConnectionGene newToOut = new ConnectionGene(newNode.getId(), outNode.getId(), connection.getWeight(), true, 0);

        nodes.Add(newNode);
        connections.Add(inToNew);
        connections.Add(newToOut);
    }
}
