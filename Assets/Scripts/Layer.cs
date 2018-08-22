using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    int numNodesPrevious;       //number of nodes in the previous layer
    int numNodes;               //number of nodes in this layer
    float[,] weights;           //the weights connecting the previous layer to this layer
    float[] biases;             //one bias for each node in this layer
    float[] values;             //values used for feed-forward calculation

    Layer(int numNodesPrevious, int numNodes)
    {
        //to be implemented
    }

    float[] calculateValues(float[] previousValues)
    {
        //to be implemented
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
