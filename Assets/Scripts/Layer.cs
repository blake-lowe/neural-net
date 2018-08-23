using System.Collections;
using System.Collections.Generic;

public class Layer
{
    readonly int numNodesPrevious;       //number of nodes in the previous layer
    readonly int numNodes;               //number of nodes in this layer
    double[,] weights;           //the weights connecting the previous layer to this layer
    double[] biases;             //one bias for each node in this layer
    double[] values;             //values used for feed-forward calculation

    public Layer(int numNodesPrevious, int numNodes)
    {
        weights = new double[numNodes, numNodesPrevious];
        biases = new double[numNodes];
        values = new double[numNodes];
    }

    public double[,] Weights
    {
        get
        {
            return weights;
        }

        set
        {
            weights = value;
        }
    }

    public double[] Biases
    {
        get
        {
            return biases;
        }

        set
        {
            biases = value;
        }
    }

    public double[] Values
    {
        get
        {
            return values;
        }

        set
        {
            values = value;
        }
    }

    double[] CalculateValues(double[] previousValues)
    {
        //TODO
        return new double[1];
    }


}

public static class ActivationFunctions
{
    public static double Logistic(double x)
    {
        return 1.0 / (1.0 + System.Math.Exp(-x));
    }
}
