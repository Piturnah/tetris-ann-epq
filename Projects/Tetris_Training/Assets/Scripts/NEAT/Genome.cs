using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Genome
{
    double _PERTURBING_PROBABILITY = 0.8;
    double _UNIFORM_PERTURBATION_P = 0.9;
    double _RANDOM_VALUE_PROBABILITY = 0.1;

    Dictionary<int, ConnectionGene> connections;
    Dictionary<int, NodeGene> nodes;

    System.Random rand = new System.Random();

    public Genome()
    {
        connections = new Dictionary<int, ConnectionGene>();
        nodes = new Dictionary<int, NodeGene>();
    }

    //copier
    public Genome(Genome toCopy) {
        nodes = new Dictionary<int, NodeGene>(toCopy.GetNodes());
        connections = new Dictionary<int, ConnectionGene>(toCopy.GetConnections());
    }

    public void AddConnectionGene(ConnectionGene gene)
    {
        connections.Add(gene.getInnovation(), gene);
        GetNodes()[gene.getOutNode()].AddInNode(GetNodes()[gene.getInNode()], gene);
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

    public void ValueMutation()
    {
        if (rand.NextDouble() < _PERTURBING_PROBABILITY)
        {
            foreach (ConnectionGene connectionToMutate in connections.Values)
            {
                if (rand.NextDouble() < _UNIFORM_PERTURBATION_P) // value adjusted
                {
                    connectionToMutate.SetWeight(connectionToMutate.getWeight() * ((float)rand.Next(5, 15) * 0.1f * Math.Sign((float)rand.NextDouble() * 2 - 1)));
                }
                else // new weight
                {
                    connectionToMutate.SetWeight(rand.Next(-15, 15) * 0.1f);
                }
            }
        }
    }

    public void AddConnectionMutation()
    {
        NodeGene node1 = nodes.ElementAt(rand.Next(0, nodes.Count)).Value;
        NodeGene node2 = nodes.ElementAt(rand.Next(0, nodes.Count)).Value;
        float weight = (float)rand.Next(-15, 15) * 0.1f;

        if (node2.getType() == NodeGene.TYPE.SENSOR || node1.getType() == NodeGene.TYPE.OUTPUT)
        { // swap genes if sensor is output
            NodeGene tempGene = node1.CopyNode();
            node1 = node2.CopyNode();
            node2 = tempGene.CopyNode();
        }

        if (node1.getType() == NodeGene.TYPE.OUTPUT || node2.getType() == NodeGene.TYPE.SENSOR) {
            AddConnectionMutation();
            return; // quietly return if still trying to use output as in-node or sensor as out
        }

        // check if node2 has higher distance from sensor than node1, if not then return
        // to ensure network remains purely feed-forward
        if (node1.CalculateDstFromSensor() != 0 && node2.CalculateDstFromSensor() <= node1.CalculateDstFromSensor() && node2.getType() != NodeGene.TYPE.OUTPUT) {
            AddConnectionMutation();
            return;
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
            //ConnectionGene newConnection = new ConnectionGene(node1.getId(), node2.getId(), weight, true, History.Innovate());
            //connections.Add(newConnection.getInnovation(), newConnection);

            //node2.AddInNode(node1);
            AddConnectionGene(new ConnectionGene(node1.getId(), node2.getId(), weight, true, History.Innovate()));
            //ConsoleLogger.Log("added connection");
        } else {
            //ConsoleLogger.Log("already existed");
        }
    }

    public void AddNodeMutation()
    {
        ConnectionGene connection = connections.ElementAt(rand.Next(0, connections.Count)).Value;

        NodeGene inNode = nodes[connection.getInNode()];
        NodeGene outNode = nodes[connection.getOutNode()];

        connection.Disable();

        //NodeGene newNode = new NodeGene(NodeGene.TYPE.HIDDEN, nodes.Count + 1);
        NodeGene newNode = new NodeGene(NodeGene.TYPE.HIDDEN, History.NodeInnovate());
        ConnectionGene inToNew = new ConnectionGene(inNode.getId(), newNode.getId(), 1f, true, History.Innovate());
        ConnectionGene newToOut = new ConnectionGene(newNode.getId(), outNode.getId(), connection.getWeight(), true, History.Innovate());

        nodes.Add(newNode.getId(), newNode);
        //connections.Add(inToNew.getInnovation(), inToNew);
        //connections.Add(newToOut.getInnovation(), newToOut);
        AddConnectionGene(inToNew);
        AddConnectionGene(newToOut);

        //newNode.AddInNode(GetNodes()[inToNew.getInNode()]);
        //GetNodes()[newToOut.getOutNode()].AddInNode(newNode);

        // remove old in-node from out gene
        outNode.RemoveInNode(inNode);
    }

    public static Genome Crossover(Genome parent1, Genome parent2)
    { // parent1 always more fit parent - no equal fitness parents
        System.Random rand = new System.Random();
        Genome offspring = new Genome();

        foreach (NodeGene parent1Node in parent1.GetNodes().Values)
        {
            offspring.AddNodeGene(parent1Node.CopyNode());
        }

        foreach (ConnectionGene parent1Connection in parent1.GetConnections().Values)
        {
            if (parent2.GetConnections().ContainsKey(parent1Connection.getInnovation()))
            {
                offspring.AddConnectionGene((rand.NextDouble() > 0.5) ? parent1Connection.CopyConnection() : parent2.GetConnections()[parent1Connection.getInnovation()].CopyConnection());
            } else {
                offspring.AddConnectionGene(parent1Connection.CopyConnection());
            }
        }

        return offspring;
    }

    // calculates compatibility between two genomes
    public static float CompatibilityDist(Genome genome1, Genome genome2, float c1, float c2, float c3)
    {
        float[] geneInfo = GetGeneInfo(genome1, genome2); // matching, weight diff, excess, disjoint

        int disjointGenes = (int)geneInfo[3];
        int excessGenes = (int)geneInfo[2];
        int maxGeneCount = GetMaxSize(genome1, genome2);

        return (c1 * excessGenes + c2 * disjointGenes) / maxGeneCount + c3 * geneInfo[1];
    }

    public static int GetMaxSize(Genome genome1, Genome genome2)
    {
        /*List<int> nodeKeys1 = genome1.GetNodes().Keys.ToList();
        for (int i = 0; i < nodeKeys1.Count; i++) {
            if (genome1.GetNodes()[nodeKeys1[i]].getType() == NodeGene.TYPE.SENSOR || genome1.GetNodes()[nodeKeys1[i]].getType() == NodeGene.TYPE.OUTPUT) {
                nodeKeys1.Remove(i);
            }
        }
        nodeKeys1.Sort();

        List<int> nodeKeys2 = genome2.GetNodes().Keys.ToList();
        for (int i = 0; i < nodeKeys2.Count; i++) {
            if (genome2.GetNodes()[nodeKeys2[i]].getType() == NodeGene.TYPE.SENSOR || genome2.GetNodes()[nodeKeys2[i]].getType() == NodeGene.TYPE.OUTPUT) {
                nodeKeys2.Remove(i);
            }
        }
        nodeKeys2.Sort();

        List<int> connectionKeys1 = genome1.GetConnections().Keys.ToList();
        connectionKeys1.Sort();

        List<int> connectionKeys2 = genome2.GetConnections().Keys.ToList();
        connectionKeys2.Sort();
        */

        int size1 = genome1.GetNodes().Count + genome1.GetConnections().Count;
        int size2 = genome2.GetNodes().Count + genome2.GetConnections().Count;

        return Math.Max(size1, size2);
    }

    public static float[] GetGeneInfo(Genome genome1, Genome genome2)
    {
        int matchingGenes = 0;
        float weightDiff = 0;
        int excessGenes = 0;
        int disjointGenes = 0;

        List<int> nodeKeys1 = genome1.GetNodes().Keys.ToList();
        nodeKeys1.Sort();

        List<int> nodeKeys2 = genome2.GetNodes().Keys.ToList();
        nodeKeys2.Sort();

        int highestInnovation1 = nodeKeys1[nodeKeys1.Count() - 1];
        int highestInnovation2 = nodeKeys2[nodeKeys2.Count() - 1];
        int minHighestInnovation = Math.Min(highestInnovation1, highestInnovation2);

        for (int i = 1; i <= minHighestInnovation; i++)
        {
            bool node1 = genome1.GetNodes().TryGetValue(i, out NodeGene value);
            bool node2 = genome2.GetNodes().TryGetValue(i, out NodeGene value2);
            if (node1 && node2 ) // both exist
            {
                matchingGenes++;
            } else if ((!node1) != (!node2)) // only one exists
            {
                disjointGenes++;
            }
        }

        List<int> connectionKeys1 = genome1.GetConnections().Keys.ToList();
        connectionKeys1.Sort();

        List<int> connectionKeys2 = genome2.GetConnections().Keys.ToList();
        connectionKeys2.Sort();

        highestInnovation1 = connectionKeys1[connectionKeys1.Count() - 1];
        highestInnovation2 = connectionKeys2[connectionKeys2.Count() - 1];
        minHighestInnovation = Math.Min(highestInnovation1, highestInnovation2);

        for (int i = 1; i <= minHighestInnovation; i++)
        {
            ConnectionGene value = null;
            ConnectionGene connection1 = (genome1.GetConnections().TryGetValue(i, out value))?genome1.GetConnections()[i]:null;
            ConnectionGene connection2 = (genome2.GetConnections().TryGetValue(i, out value))?genome2.GetConnections()[i]:null;;
            if (connection1 != null && connection2 != null) // both exist
            {
                matchingGenes++;
                weightDiff = Math.Abs(connection1.getWeight() - connection2.getWeight());
            } else if ((connection1 == null) != (connection2 == null)) // only one exists
            {
                disjointGenes++;
            }
        }

        return new float[] { matchingGenes, weightDiff / matchingGenes, excessGenes, disjointGenes};
    }

    public static int CountExcessGenes(Genome genome1, Genome genome2)
    {
        int excessGenes = 0;

        List<int> nodeKeys1 = genome1.GetNodes().Keys.ToList();
        nodeKeys1.Sort();

        List<int> nodeKeys2 = genome2.GetNodes().Keys.ToList();
        nodeKeys2.Sort();

        int highestInnovation1 = nodeKeys1[nodeKeys1.Count() - 1];
        int highestInnovation2 = nodeKeys2[nodeKeys2.Count() - 1];
        int minHighestInnovation = Math.Min(highestInnovation1, highestInnovation2);
        int maxHighestInnovation = Math.Max(highestInnovation1, highestInnovation2);

        excessGenes += maxHighestInnovation - minHighestInnovation;

        List<int> connectionKeys1 = genome1.GetConnections().Keys.ToList();
        connectionKeys1.Sort();

        List<int> connectionKeys2 = genome2.GetConnections().Keys.ToList();
        connectionKeys2.Sort();

        highestInnovation1 = connectionKeys1[nodeKeys1.Count() - 1];
        highestInnovation2 = connectionKeys2[nodeKeys2.Count() - 1];
        minHighestInnovation = Math.Min(highestInnovation1, highestInnovation2);
        maxHighestInnovation = Math.Max(highestInnovation1, highestInnovation2);

        excessGenes += maxHighestInnovation - minHighestInnovation;

        return excessGenes;
    }

    public void ResetNodeActivity() {
        foreach (NodeGene node in GetNodes().Values) {
            node.SetActive(false);
        }
    }
}
