using System.Collections;
using System.Collections.Generic;

public class ConnectionGene
{
    int inNode;
    int outNode;
    float weight;
    bool expressed;
    int innovation;

    public ConnectionGene(int inNode, int outNode, float weight, bool expressed, int innovation)
    {
        this.inNode = inNode;
        this.outNode = outNode;
        this.weight = weight;
        this.expressed = expressed;
        this.innovation = innovation;
    }

    public void SetWeight(float newWeight)
    {
        weight = newWeight;
    }

    public void Disable()
    {
        expressed = false;
    }

    public int getInNode()
    {
        return inNode;
    }
    public int getOutNode()
    {
        return outNode;
    }
    public float getWeight()
    {
        return weight;
    }
    public bool getExpressed()
    {
        return expressed;
    }
    public int getInnovation()
    {
        return innovation;
    }

    public ConnectionGene CopyConnection()
    {
        return new ConnectionGene(inNode, outNode, weight, expressed, innovation);
    }
}
