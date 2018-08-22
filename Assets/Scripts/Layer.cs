using System.Collections;
using System.Collections.Generic;

public class Layer
{
    int numNodesPrevious;       //number of nodes in the previous layer
    int numNodes;               //number of nodes in this layer
    double[,] weights;           //the weights connecting the previous layer to this layer
    double[] biases;             //one bias for each node in this layer
    double[] values;             //values used for feed-forward calculation

    public Layer(int numNodesPrevious, int numNodes)
    {
        weights = new double[numNodes, numNodesPrevious];
        biases = new double[numNodes];
        values = new double[numNodes];
    }

    double[] CalculateValues(double[] previousValues)
    {
        //TODO
        return new double[1];
    }

    void SetWeights(double[,] weights)
    {
        this.weights = weights;
    }
    
    double[] GetValues()
    {
        return values;
    }

    void SetValues(double[] values)
    {
        this.values = values;
    }
}

public static class ActivationFunctions
{
    public static double Logistic(double x)
    {
        return 1.0 / (1.0 + System.Math.Exp(-x));
    }
}
