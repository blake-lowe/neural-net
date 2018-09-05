using System;
using System.Collections;
using System.Collections.Generic;

public class Layer
{
    private int numNodesPrevious;       //number of nodes in the previous layer
    private int numNodes;               //number of nodes in this layer
    double[,] weights;                  //the weights connecting the previous layer to this layer [node, nodePrevious]//[i,0] is a bias for the node i
    double[] values;                    //values used for feed-forward calculation

    public Layer(int numNodesPrevious, int numNodes)
    {
        this.NumNodesPrevious = numNodesPrevious;
        this.NumNodes = numNodes;
        weights = new double[numNodes, numNodesPrevious + 1];
        values = new double[numNodes];

        RandomizeWeights();
    }

    public Layer(int numNodesPrevious, int numNodes, double[,] weights)
    {
        this.NumNodesPrevious = numNodesPrevious;
        this.NumNodes = numNodes;
        weights = new double[numNodes, numNodesPrevious + 1];
        values = new double[numNodes];

        this.weights = weights;
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

    public int NumNodesPrevious
    {
        get
        {
            return numNodesPrevious;
        }

        set
        {
            numNodesPrevious = value;
        }
    }

    public int NumNodes
    {
        get
        {
            return numNodes;
        }

        set
        {
            numNodes = value;
        }
    }

    void RandomizeWeights()             //randomizes biases and weights
    {
        for (int i = 0; i < weights.GetLength(0); i++)
        {
            for (int j = 0; j < weights.GetLength(1); j++)
            {
                weights[i, j] = RandHolder.NextDouble();
            }           
        }
    }

    public double[] FeedForward(double[] previousValues)
    {
        double sum;                                     //temporary summation variable
        for (int i = 0; i < NumNodes; i++)              //iterate once per value to be calculated
        {
            sum = 0;
            sum += weights[i, 0];                       //add bias
            for (int j = 1; j < NumNodesPrevious; j++)  //iterate once per weight
            {
                sum += weights[i, j] * previousValues[j];
            }
            sum = Functions.Sigmoid(sum);    //normalize values between 0 and 1
            values[i] = sum;
        }
        return values;
    }

    public double CalculateTotalError(double[] error)            //use only for output layer
    {
        double MSE = 0;                                     //mean square error
        for (int i = 0; i < error.GetLength(0); i++)
        {
            MSE += error[i]*error[i];
        }
        MSE *= 1 / (2 * error.GetLength(0));
        return MSE;
    }

    public double[,] BackpropagateOutput(double[] targets)     //returns array weight error values. Does not change weights
                                                        //use only for the output layer 
                                                        //targets must have length equal to numNodes
    {
        double[,] deltaWeights = new double[weights.GetLength(0), weights.GetLength(1)];//same size as weights
        for (int i = 0; i < NumNodes; i++)              
        {
            for (int j = 0; j < NumNodesPrevious + 1; j++)  //iterate once per weight and +1 for bias
            {
                deltaWeights[i, j] = targets[i] - values[i];
            }
        }
        return deltaWeights;
    }

    public double[,] BackpropagateHidden(double[,] errorValues, double[,] weightsAfter)     
                                                        //returns array weight error values. Does not change weights
                                                        //use only for the hidden layers
                                                        //weightsAfter is the weights of the layer one step after this in the FeedForward direction
                                                        //errors must have same dimensions as weights. 
                                                        //will be called with the output of BackpropagateOutput() or BackpropagateHidden()
    {
        double[,] deltaWeights = new double[weights.GetLength(0), weights.GetLength(1)];//same size as weights
        for (int i = 0; i < NumNodes; i++)//iterate once per node
        {
            //find error value for the node
            double sum = 0;
            for (int k = 0; k < errorValues.GetLength(0); k++)
            {
                sum += errorValues[i, k] * weightsAfter[i, k];
            }
            double deltaWeight = Functions.SigmoidDeritive(values[i])* sum;
            //assign error values to deltaWeights
            for (int j = 0; j < NumNodesPrevious + 1; j++)  //iterate once per weight per node and +1 for bias
            {
                deltaWeights[i, j] = deltaWeight;
            }
        }
        return deltaWeights;
    }

}

public static class Functions
{
    public static double Sigmoid(double x)//is bounded between 0 and 1
    {
        return 1.0 / (1.0 + System.Math.Exp(-x));
    }
    public static double SigmoidDeritive(double x)
    {
        return x * (1 - x);
    }
}