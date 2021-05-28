using System.Collections.Generic;

[System.Serializable]
public class GenomeData
{
    public List<float> connectionGeneData;
    public List<int> nodeGeneData; 

     public GenomeData(Genome genome) {
        connectionGeneData = new List<float>();
        nodeGeneData = new List<int>();

        foreach (ConnectionGene c in genome.GetConnections().Values) {
            connectionGeneData.Add(c.getInNode());
            connectionGeneData.Add(c.getOutNode());
            connectionGeneData.Add(c.getWeight());
            connectionGeneData.Add((c.getExpressed()) ? 1 : 0);
            connectionGeneData.Add(c.getInnovation());
        }
        foreach (NodeGene n in genome.GetNodes().Values) {
            if (n.getActivation() == NodeGene.ACTIVATION.SINE) { nodeGeneData.Add(0); } else if (n.getActivation() == NodeGene.ACTIVATION.COSINE) { nodeGeneData.Add(1); } else { nodeGeneData.Add(2); }
            if (n.getType() == NodeGene.TYPE.HIDDEN) { nodeGeneData.Add(0); } else if (n.getType() == NodeGene.TYPE.OUTPUT) { nodeGeneData.Add(1); } else { nodeGeneData.Add(2); }
            nodeGeneData.Add(n.getId());
        }
    }
}
