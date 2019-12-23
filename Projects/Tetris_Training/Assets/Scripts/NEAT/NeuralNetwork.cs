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

        while (outputActivations.Count < noOutputs) {
            // for all non-sensor nodes
                // activate node
                // sum the input

            // for all non-sensor and active nodes
                // calculate the output
        }

        genome.ResetNodeActivity();

        return outputActivations.ToArray();
    }
}
