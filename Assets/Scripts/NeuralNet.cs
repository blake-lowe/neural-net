using System.Collections;
using System.Collections.Generic;

public class NeuralNet
{
    readonly int numInputs;          //number of nodes in the input layer of the net
    readonly int numOutputs;         //number of nodes in the output layer of the net
    readonly int numHiddenLayers;    //the number of hidden layers
    readonly int hiddenLayerSize;    //the number of nodes in each hidden layer //DOES NOT SUPPORT VARYING HIDDEN LAYER SIZES//
    private readonly Layer[] hiddenLayers;
    
    public NeuralNet(int numInputs, int numOutputs, int numHiddenLayers, int hiddenLayerSize)
    {
        //TODO
    }

    void FeedForward(Layer input)
    {
        //TODO
    }

    void Backpropagate(double error)
    {
        //TODO
    }
}
