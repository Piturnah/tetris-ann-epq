using System.Collections;
using System.Collections.Generic;
using System;

public class Genome
{
    Dictionary<int, ConnectionGene> connections;
    Dictionary<int, NodeGene> nodes;

    Random rand = new Random();

    public Genome()
    {
        connections = new Dictionary<int, ConnectionGene>();
        nodes = new Dictionary<int, NodeGene>();
    }

    public void AddConnectionGene(ConnectionGene gene)
    {
        connections.Add(gene.getInnovation(), gene);
    }
    public void AddNodeGene(NodeGene gene)
    {
        nodes.Add(gene.getId(), gene);
    }

    public Dictionary<int, ConnectionGene> GetConnections()
    {
        return connections;
    }
    public Dictionary<int ,NodeGene> GetNodes()
    {
        return nodes;
    }

    public void AddConnectionMutation()
    {
        NodeGene node1 = nodes[rand.Next(nodes.Count)];
        NodeGene node2 = nodes[rand.Next(nodes.Count)];
        float weight = (float)rand.NextDouble() * 2f - 1f;

        if (node2.getType() == NodeGene.TYPE.SENSOR || node1.getType() == NodeGene.TYPE.OUTPUT)
        {
            NodeGene tempGene = node1.CopyNode();
            node1 = node2.CopyNode();
            node2 = tempGene.CopyNode();
        }

        bool connectionExists = false; // check if connection already exists
        foreach (ConnectionGene connection in connections.Values)
        {
            if (connection.getInNode() == node1.getId() && connection.getOutNode() == node2.getId())
            {
                connectionExists = true;
            }
        }

        if (!connectionExists)
        {
            ConnectionGene newConnection = new ConnectionGene(node1.getId(), node2.getId(), weight, true, History.Innovate());
            connections.Add(newConnection.getInnovation(), newConnection);
        }
    }

    public void AddNodeMutation()
    {
        ConnectionGene connection = connections[rand.Next(connections.Count)];

        NodeGene inNode = nodes[connection.getInNode()];
        NodeGene outNode = nodes[connection.getOutNode()];

        connection.Disable();

        NodeGene newNode = new NodeGene(NodeGene.TYPE.HIDDEN, nodes.Count);
        ConnectionGene inToNew = new ConnectionGene(inNode.getId(), newNode.getId(), 1f, true, History.Innovate());
        ConnectionGene newToOut = new ConnectionGene(newNode.getId(), outNode.getId(), connection.getWeight(), true, History.Innovate());

        nodes.Add(newNode.getId(), newNode);
        connections.Add(inToNew.getInnovation(), inToNew);
        connections.Add(newToOut.getInnovation(), newToOut);
    }

    public Genome Crossover(Genome parent1, Genome parent2)
    {
        Genome offspring = new Genome();

        foreach (NodeGene parent1Node in parent1.GetNodes().Values)
        {
            offspring.AddNodeGene(parent1Node.CopyNode());
        }

        foreach (ConnectionGene parent1Connection in parent1.GetConnections().Values)
        {
            if (parent2.GetConnections().ContainsKey(parent1Connection.getInnovation()))
            {
                offspring.AddConnectionGene((rand.NextDouble() > 0.5) ? parent1Connection : parent2.GetConnections()[parent1Connection.getInnovation()]);
            } else
            {
                offspring.AddConnectionGene(parent1Connection);
            }
        }

        return offspring;
    }
}
