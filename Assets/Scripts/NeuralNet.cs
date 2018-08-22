using System.Collections;
using System.Collections.Generic;

public class NeuralNet
{
    int numInputs;          //number of nodes in the input layer of the net
    int numOutputs;         //number of nodes in the output layer of the net
    int numHiddenLayers;    //the number of hidden layers
    int hiddenLayerSize;    //the number of nodes in each hidden layer //DOES NOT SUPPORT VARYING HIDDEN LAYER SIZES//
    Layer[] hiddenLayers;
    
    NeuralNet(int numInputs, int numOutputs, int numHiddenLayers, int hiddenLayerSize)
    {
        //TODO
    }

    Layer feedForward(Layer input)
    {
        //TODO
    }

    backpropagate(float error)
    {
        //TODO
    }
}
