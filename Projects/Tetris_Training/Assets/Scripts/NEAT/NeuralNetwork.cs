public class NeuralNetwork
{
    public Genome genome;

    public NeuralNetwork(Genome genome) {
        this.genome = genome;
    }

    public float GetNNResult(float[] inputs) {
        float outputActivation = 0;

        foreach (NodeGene node in genome.GetNodes().Values) {
            if (node.getType() == NodeGene.TYPE.OUTPUT) {
                outputActivation = node.GetActivation(inputs);
            }
        }

        genome.ResetNodeActivity();

        return outputActivation;
    }
}
