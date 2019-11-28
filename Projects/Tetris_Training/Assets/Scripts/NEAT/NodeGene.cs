using System.Collections;
using System.Collections.Generic;

public class NodeGene
{
    public enum TYPE
    {
        INPUT,
        HIDDEN,
        OUTPUT
    }

    TYPE type;
    int id;

    public NodeGene(TYPE type, int id)
    {
        this.type = type;
        this.id = id;
    }

    public TYPE getType()
    {
        return type;
    }
    public int getId()
    {
        return id;
    }
}
