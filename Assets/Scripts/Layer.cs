using System.Collections;
using System.Collections.Generic;

public class Layer
{
    int numNodesPrevious;       //number of nodes in the previous layer
    int numNodes;               //number of nodes in this layer
    double[,] weights;           //the weights connecting the previous layer to this layer
    double[] biases;             //one bias for each node in this layer
    double[] values;             //values used for feed-forward calculation

    Layer(int numNodesPrevious, int numNodes)
    {
        weights = new double[numNodes, numNodesPrevious];
        biases = new double[numNodes];
        values = new double[numNodes];
    }

    double[] calculateValues(double[] previousValues)
    {
        //TODO
    }

    void setWeights(double[,] weights)
    {
        this.weights = weights;
    }
    
    double[] getValues()
    {
        return values;
    }

    void setValues(double[] values)
    {
        this.values = values;
    }
}

public static class ActivationFunctions
{
    public static double logistic(double x)
    {
        return 1.0 / (1.0 + Math.Exp(-x));
    }
}
