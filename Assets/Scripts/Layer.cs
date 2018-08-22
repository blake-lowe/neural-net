using System.Collections;
using System.Collections.Generic;

public class Layer
{
    int numNodesPrevious;       //number of nodes in the previous layer
    int numNodes;               //number of nodes in this layer
    float[,] weights;           //the weights connecting the previous layer to this layer
    float[] biases;             //one bias for each node in this layer
    float[] values;             //values used for feed-forward calculation

    Layer(int numNodesPrevious, int numNodes)
    {
        //TODO
    }

    float[] calculateValues(float[] previousValues)
    {
        //TODO
    }
    
    float[] getValues()
    {
        return values;
    }

    setValues(float[] values)
    {
        this.values = values;
    }
}
