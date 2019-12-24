using System.Collections;
using System.Collections.Generic;

public class NeuralNetwork
{
    Genome genome;

    public NeuralNetwork(Genome genome) {
        this.genome = genome;
    }

    public float[] GetNNResult(float[] inputs, int noOutputs) {
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
