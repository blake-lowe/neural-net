﻿using System;
using System.Collections;
using System.Collections.Generic;

public class NeuralNet
{
    readonly int numInputs;          //number of nodes in the input layer of the net
    readonly int numOutputs;         //number of nodes in the output layer of the net
    readonly int numHiddenLayers;    //the number of hidden layers
    readonly int hiddenLayerSize;    //the number of nodes in each hidden layer //DOES NOT SUPPORT VARYING HIDDEN LAYER SIZES// although I don't see why not(8/29)
    private Layer[] layers; //including hidden layers and output layers but not input layer. Logic works by attaching a set of weights to before a layer.

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

    NeuralNet Backpropagate(double[] targets)//targets must have length equal to numNodes
    {
        Layer[] updatedLayers = new Layer[numHiddenLayers + 1];
        //output layer
        double[,] deltaWeights = layers[layers.Length - 1].BackpropagateOutput(targets);
        double[,] updatedWeights = new double[numOutputs, hiddenLayerSize];
        for (int i = 0; i < numOutputs; i++)
        {
            for (int j = 0; j < hiddenLayerSize; j++)
            {
                updatedWeights[i, j] = layers[layers.Length - 1].Weights[i, j] + deltaWeights[i, j];
            }
        }
        updatedLayers[updatedLayers.Length - 1].Weights = updatedWeights;

        updatedWeights = new double[hiddenLayerSize, hiddenLayerSize];      //hidden layers
        for (int hiddenLayerIndex = numHiddenLayers - 1; hiddenLayerIndex > 0; hiddenLayerIndex--)
        {
            deltaWeights = layers[hiddenLayerIndex].BackpropagateHidden(deltaWeights, layers[hiddenLayerIndex + 1].Weights);
            for (int i = 0; i < hiddenLayerSize; i++)
            {
                for (int j = 0; j < hiddenLayerSize; j++)
                {
                    updatedWeights[i, j] = layers[hiddenLayerIndex].Weights[i, j] + deltaWeights[i, j];
                }
            }
            updatedLayers[hiddenLayerIndex].Weights = updatedWeights;
        }

        updatedWeights = new double[hiddenLayerSize, numInputs];            //first hidden layer
        deltaWeights = layers[0].BackpropagateHidden(deltaWeights, layers[1].Weights);
        for (int i = 0; i < hiddenLayerSize; i++)
        {
            for (int j = 0; j < numInputs; j++)
            {
                updatedWeights[i, j] = layers[0].Weights[i, j] + deltaWeights[i, j];
            }
        }
        updatedLayers[0].Weights = updatedWeights;

        NeuralNet updatedNet = new NeuralNet(numInputs, numOutputs, numHiddenLayers, hiddenLayerSize, rand);
        updatedNet.layers = updatedLayers;
        return updatedNet;
    }
}
