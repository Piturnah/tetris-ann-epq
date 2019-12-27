using System.Collections;
using System.Collections.Generic;

public class NeuralNetwork
{
    public Genome genome;

    public NeuralNetwork(Genome genome) {
        this.genome = genome;
    }

    public float[] GetNNResult(float[] inputs) {
        List<float> outputActivations = new List<float>();

        foreach (NodeGene node in genome.GetNodes().Values) {
            if (node.getType() == NodeGene.TYPE.OUTPUT) {
                outputActivations.Add(node.GetActivation(inputs));
            }
        }

        genome.ResetNodeActivity();

        return outputActivations.ToArray();
    }
}
