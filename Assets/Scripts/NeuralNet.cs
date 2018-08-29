using System;
using System.Collections;
using System.Collections.Generic;

public class NeuralNet
{
    readonly int numInputs;          //number of nodes in the input layer of the net
    readonly int numOutputs;         //number of nodes in the output layer of the net
    readonly int numHiddenLayers;    //the number of hidden layers
    readonly int hiddenLayerSize;    //the number of nodes in each hidden layer //DOES NOT SUPPORT VARYING HIDDEN LAYER SIZES// although I don't see why not(8/29)
    private readonly Layer[] layers; //including hidden layers and output layers but not input layer. Logic works by attaching a set of weights to before a layer.

    Random rand;

    public Random Rand
    {
        get
        {
            return rand;
        }

        set
        {
            rand = value;
        }
    }

    public NeuralNet(int numInputs, int numOutputs, int numHiddenLayers, int hiddenLayerSize)
    {
        this.numInputs = numInputs;
        this.numOutputs = numOutputs;
        this.numHiddenLayers = numHiddenLayers;
        this.hiddenLayerSize = hiddenLayerSize;

        //instantiate layers
        layers = new Layer[numHiddenLayers + 1];    //+1 for output layer
        //assign first term (different because must fit into the input layer)
        layers[0] = new Layer(numInputs, hiddenLayerSize);
        for (int i = 1; i < layers.Length - 1; i++)
        {
            layers[i] = new Layer(hiddenLayerSize, hiddenLayerSize);
        }
        //assign last term (output Layer)
        layers[layers.Length - 1] = new Layer(hiddenLayerSize, numOutputs);
        
    }

    public NeuralNet(int numInputs, int numOutputs, int numHiddenLayers, int hiddenLayerSize, Random rand)//constructor to pass the random object to layers to enable reproduction of trials
    {
        this.numInputs = numInputs;
        this.numOutputs = numOutputs;
        this.numHiddenLayers = numHiddenLayers;
        this.hiddenLayerSize = hiddenLayerSize;
        this.rand = rand;

        //instantiate layers
        layers = new Layer[numHiddenLayers + 1];    //+1 for output layer
        //assign first term (different because must fit into the input layer)
        layers[0] = new Layer(numInputs, hiddenLayerSize, rand);
        for (int i = 1; i < layers.Length - 1; i++)
        {
            layers[i] = new Layer(hiddenLayerSize, hiddenLayerSize, rand);
        }
        //assign last term (output Layer)
        layers[layers.Length - 1] = new Layer(hiddenLayerSize, numOutputs, rand);
    }

    double[] FeedForward(double[] inputValues) //inputValues.Length must equal numInputs //will return double[] with length = numOutputs
    {
        double[] lastValues = new double[hiddenLayerSize]; //holds values of the last layer to be passed into the current layer
        lastValues = layers[0].FeedForward(inputValues);
        for (int i = 1; i < layers.Length - 1; i++)
        {
            lastValues = layers[i].FeedForward(lastValues);
        }
        return layers[layers.Length - 1].FeedForward(lastValues);
    }

    void Backpropagate(double error)
    {
        //TODO
    }
}
