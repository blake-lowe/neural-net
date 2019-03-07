using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNet:IGeneticIndividual//a class which implements the Neural Net. Designed to be trained through either backpropagation or with a Genetic Algorithm (hence implementation of interface
{
    public int numInputs;          //number of nodes in the input layer of the net
    public int numOutputs;         //number of nodes in the output layer of the net
    public int numHiddenLayers;    //the number of hidden layers
    public int hiddenLayerSize;    //the number of nodes in each hidden layer //DOES NOT SUPPORT VARYING HIDDEN LAYER SIZES// although I don't see why not(8/29)
    public double learningRate;       //factor to multiply deltaweights by during backpropagation
    public Layer[] layers; //including hidden layers and output layers but not input layer. Logic works by attaching a set of weights to before a layer.
    public double numTestSets;
    public double[,] TestInputSets;//first column is index and the second column is the test set
    public double[,] TestOutputSets;

    public Layer[] Layers
    {
        get
        {
            return layers;
        }
    }

    public int NumInputs
    {
        get
        {
            return numInputs;
        }

        set
        {
            numInputs = value;
        }
    }

    public int NumOutputs
    {
        get
        {
            return numOutputs;
        }
    }

    public int NumHiddenLayers
    {
        get
        {
            return numHiddenLayers;
        }
    }

    public int HiddenLayerSize
    {
        get
        {
            return hiddenLayerSize;
        }
    }

    public double LearningRate
    {
        get
        {
            return learningRate;
        }

        set
        {
            learningRate = value;
        }
    }

    public NeuralNet()
    {

    }

    public NeuralNet(int numInputs, int numOutputs, int numHiddenLayers, int hiddenLayerSize, double learningRate)//use this constructor for backpropagation training
    {
        this.numInputs = numInputs;
        this.numOutputs = numOutputs;
        this.numHiddenLayers = numHiddenLayers;
        this.hiddenLayerSize = hiddenLayerSize;
        this.learningRate = learningRate;

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

    public NeuralNet(int numInputs, int numOutputs, int numHiddenLayers, int hiddenLayerSize, double[,] testInputSets, double[,] testOutputSets)//use this constructor for Genetic Algorithm training
    {
        this.numInputs = numInputs;
        this.numOutputs = numOutputs;
        this.numHiddenLayers = numHiddenLayers;
        this.hiddenLayerSize = hiddenLayerSize;
        this.numTestSets = testInputSets.GetLength(0);
        this.TestInputSets = testInputSets;
        this.TestOutputSets = testOutputSets;

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

    public NeuralNet(int numInputs, int numOutputs, int numHiddenLayers, int hiddenLayerSize, Layer[] layers)//use this constructor to hardcode values
    {
        this.numInputs = numInputs;
        this.numOutputs = numOutputs;
        this.numHiddenLayers = numHiddenLayers;
        this.hiddenLayerSize = hiddenLayerSize;
        this.layers = layers;
    }

    public double[] FeedForward(double[] inputValues) //inputValues.Length must equal numInputs //will return double[] with length = numOutputs
    {
        double[] lastValues = new double[hiddenLayerSize]; //holds values of the last layer to be passed into the current layer
        lastValues = layers[0].FeedForward(inputValues);
        for (int i = 1; i < layers.Length - 1; i++)
        {
            lastValues = layers[i].FeedForward(lastValues);//if you get an error on this line check the outputs and target array sizes. Maybe net constructed wrong?
        }
        double[] outputs = layers[layers.Length - 1].FeedForward(lastValues);
        for (int i = 0; i < outputs.Length; i++)//added normalization of outputs
        {
            outputs[i] = Functions.Sigmoid(outputs[i]);
        }
        return outputs;
    }

    public NeuralNet Backpropagate(double[] inputValues, double[] targets)//targets must have length equal to numOutputs. usage is net = net.Backpropagate();
    {
        this.FeedForward(inputValues);
        Layer[] updatedLayers = new Layer[numHiddenLayers + 1];
        //output layer
        double[,] deltaWeights = layers[layers.Length - 1].BackpropagateOutput(targets);
        double[,] updatedWeights = new double[numOutputs, hiddenLayerSize + 1];
        for (int i = 0; i < numOutputs; i++)
        {
            for (int j = 0; j < hiddenLayerSize + 1; j++)
            {
                updatedWeights[i, j] = layers[layers.Length - 1].Weights[i, j] + learningRate*deltaWeights[i, j];
                //updatedWeights[i, j] = Functions.Sigmoid(updatedWeights[i, j]);
            }
        }
        updatedLayers[updatedLayers.Length - 1] = new Layer(hiddenLayerSize, numOutputs, updatedWeights);

        updatedWeights = new double[hiddenLayerSize, hiddenLayerSize + 1];      //hidden layers
        for (int hiddenLayerIndex = numHiddenLayers - 1; hiddenLayerIndex > 0; hiddenLayerIndex--)
        {
            deltaWeights = layers[hiddenLayerIndex].BackpropagateHidden(deltaWeights, layers[hiddenLayerIndex + 1].Weights);
            for (int i = 0; i < hiddenLayerSize; i++)
            {
                for (int j = 0; j < hiddenLayerSize; j++)
                {
                    updatedWeights[i, j] = layers[hiddenLayerIndex].Weights[i, j] + learningRate*deltaWeights[i, j];
                    //updatedWeights[i, j] = Functions.Sigmoid(updatedWeights[i, j]);
                }
            }
            updatedLayers[hiddenLayerIndex] = new Layer(hiddenLayerSize, hiddenLayerSize, updatedWeights);
        }

        updatedWeights = new double[hiddenLayerSize, numInputs + 1];            //first hidden layer
        deltaWeights = layers[0].BackpropagateHidden(deltaWeights, layers[1].Weights);
        for (int i = 0; i < hiddenLayerSize; i++)
        {
            for (int j = 0; j < numInputs; j++)
            {
                updatedWeights[i, j] = layers[0].Weights[i, j] + learningRate*deltaWeights[i, j];
                //updatedWeights[i, j] = Functions.Sigmoid(updatedWeights[i, j]);
            }
        }
        updatedLayers[0] = new Layer(numInputs, hiddenLayerSize, updatedWeights);

        NeuralNet updatedNet = new NeuralNet(numInputs, numOutputs, numHiddenLayers, hiddenLayerSize, learningRate)
        {
            layers = updatedLayers
        };
        return updatedNet;
    }

    public double Fitness()//return the average fitness when all test sets are fed through the network. Will return a value between 0 and numOutputs. Low fitness is bad, high fitness is good
    {
        double errorTotal = 0;
        for (int i = 0; i < numTestSets; i++)//iterate for each test set
        {
            double[] TestInputSet = new double[numInputs];//create a temp variable for the input set
            for (int j = 0; j < numInputs; j++)//fill temp array
            {
                TestInputSet[j] = TestInputSets[i, j];
            }

            double[] TestOutputSet = new double[numOutputs];//create a temp variable for the output set
            for (int j = 0; j < numOutputs; j++)//fill temp array
            {
                TestOutputSet[j] = TestOutputSets[i, j];
            }

            double[] TestResult = FeedForward(TestInputSet);//feed forward

            for (int j = 0; j < numOutputs; j++)//compare result to expected result and add to 
            {
                double error = Math.Abs(TestOutputSet[j] - TestResult[j]);//abs to keep error positive
                errorTotal += error;
            }
        }
        double fitness = -(errorTotal / numTestSets);//take the average value. should be below zero. Close to zero is good
        return fitness;
    }

    public void Randomize()
    {
        //randomize layers
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

    public IGeneticIndividual[] Reproduce(IGeneticIndividual[] IParents, int numCrossoverPoints, int numChildren)  //returns an array of nets given an array of all parents. 
                                                                                                // Doesn't necessarily need to be called by one of the parents but that's probably convenient. 
                                                                                                //numCrossover points SHOULD be > numParents-1 (to use all parents at least once), but it's fine if not
    {
        int numParents = IParents.Length;//convenient variable to have
        //convert array of type IGeneticIndividual to type NeuralNet
        NeuralNet[] parents = new NeuralNet[numParents];
        for (int i = 0; i < numParents; i++)
        {
            parents[i] = IParents[i] as NeuralNet;
        }
        NeuralNet[] children = new NeuralNet[numChildren];//variable to hold generated children. Will be output by method
        for (int childIter = 0; childIter < numChildren; childIter++)//iterate once for each child to be generated
        {
            //generate random crossoverPoints
            int[,] crossoverPoints = new int[numCrossoverPoints, 3];//3 because (i, j, k) are (layer, node, weight)
            for (int cpIter = 0; cpIter < numCrossoverPoints; cpIter++)//iterate for each crossover point and generate it. Code copied from Mutate()
            {
                //layer to cp//
                int i = (int)(RandHolder.NextDouble() * (parents[0].numHiddenLayers + 1));//+1 for output
                //node to cp//
                int j = -1;//should cause an index out of bounds if not set by subsequent if cases
                if (i == 0)
                {//if first hidden layer (same as else case)(same number of nodes as hidden layer so doesn't matter)
                    j = (int)(RandHolder.NextDouble() * parents[0].hiddenLayerSize);
                }
                else if (i == parents[0].numHiddenLayers)
                {//if output layer
                    j = (int)(RandHolder.NextDouble() * parents[0].numOutputs);
                }
                else
                {//if any other hidden layer
                    j = (int)(RandHolder.NextDouble() * parents[0].hiddenLayerSize);
                }
                //weight to cp//
                int k = -1;//should cause an index out of bounds if not set by subsequent if cases
                if (i == 0)//if first hidden layer (same as else case)(same number of nodes as hidden layer so doesn't matter)
                {
                    k = (int)(RandHolder.NextDouble() * (parents[0].numInputs) + 1);//+1 for bias
                }
                else if (i == parents[0].numHiddenLayers)//if output layer same as else case
                {
                    k = (int)(RandHolder.NextDouble() * (parents[0].hiddenLayerSize) + 1);//+1 for bias
                }
                else//if any other hidden layer
                {
                    k = (int)(RandHolder.NextDouble() * (parents[0].hiddenLayerSize) + 1);//+1 for bias
                }

                //assign i,j,k to crossoverPoints
                crossoverPoints[cpIter, 0] = i;
                crossoverPoints[cpIter, 1] = j;
                crossoverPoints[cpIter, 2] = k;//duplicate crossover points might exist but honestly who cares
            }
            //create array to hold new layers, nodes, weights
            Layer[] newLayers = new Layer[parents[0].numHiddenLayers + 1];

            //fill newLayers
            int activeParentIndex = 0;

            for (int i = 0; i < parents[0].numHiddenLayers + 1; i++)//+1 for output layer
            {
                if (i == 0)//first hidden layer
                {
                    double[,] newWeights = new double[parents[0].hiddenLayerSize, parents[0].numInputs + 1];//create empty container, +1 for bias
                    for (int j = 0; j < newWeights.GetLength(0); j++)
                    {
                        for (int k = 0; k < newWeights.GetLength(1); k++)//this will run for each weight and bias
                        {
                            //check if currently at a crossover point if so then swap the activeParentIndex
                            for (int iter = 0; iter < numCrossoverPoints; iter++)//check for each crossover point
                            {
                                if (crossoverPoints[iter, 0] == i && crossoverPoints[iter, 1] == j && crossoverPoints[iter, 2] == k)
                                {
                                    activeParentIndex = (int)(RandHolder.NextDouble() * (numParents));//choose a random parent including currently active parent
                                }
                            }
                            newWeights[j, k] = parents[activeParentIndex].layers[i].Weights[j, k];//copy the value from the active aprent
                        }
                    }
                    newLayers[i] = new Layer(parents[0].numInputs, parents[0].hiddenLayerSize, newWeights);//assign new weights to newLayers array in the form of a newly instantiated Layer

                }
                else if (i > 0 && i < parents[0].numHiddenLayers)//all other hidden layers
                {
                    double[,] newWeights = new double[parents[0].hiddenLayerSize, parents[0].hiddenLayerSize + 1];//create empty container, +1 for bias
                    for (int j = 0; j < newWeights.GetLength(0); j++)
                    {
                        for (int k = 0; k < newWeights.GetLength(1); k++)//this will run for each weight and bias
                        {
                            //check if currently at a crossover point if so then swap the activeParentIndex
                            for (int iter = 0; iter < numCrossoverPoints; iter++)//check for each crossover point
                            {
                                if (crossoverPoints[iter, 0] == i && crossoverPoints[iter, 1] == j && crossoverPoints[iter, 2] == k)
                                {
                                    int temp = (int)(RandHolder.NextDouble() * (numParents - 1));//the minus one is because we are avoiding the current activeParentIndex in this reassignment
                                    if (temp < activeParentIndex)
                                    {
                                        activeParentIndex = temp;
                                    }
                                    else//if greater than or equal to activeParentIndex
                                    {
                                        activeParentIndex = temp + 1;
                                    }
                                }
                            }
                            newWeights[j, k] = parents[activeParentIndex].layers[i].Weights[j, k];//copy the value from the active parent
                        }
                    }
                    newLayers[i] = new Layer(parents[0].numInputs, parents[0].hiddenLayerSize, newWeights);//assign new weights to newLayers array in the form of a newly instantiated Layer
                }
                else if(i == parents[0].numHiddenLayers)//output layer
                {
                    double[,] newWeights = new double[parents[0].numOutputs, parents[0].hiddenLayerSize + 1];//create empty container, +1 for bias
                    for (int j = 0; j < newWeights.GetLength(0); j++)
                    {
                        for (int k = 0; k < newWeights.GetLength(1); k++)//this will run for each weight and bias
                        {
                            //check if currently at a crossover point if so then swap the activeParentIndex
                            for (int iter = 0; iter < numCrossoverPoints; iter++)//check for each crossover point
                            {
                                if (crossoverPoints[iter, 0] == i && crossoverPoints[iter, 1] == j && crossoverPoints[iter, 2] == k)
                                {
                                    int temp = (int)(RandHolder.NextDouble() * (numParents - 1));//the minus one is because we are avoiding the current activeParentIndex in this reassignment
                                    if (temp < activeParentIndex)
                                    {
                                        activeParentIndex = temp;
                                    }
                                    else//if greater than or equal to activeParentIndex
                                    {
                                        activeParentIndex = temp + 1;
                                    }
                                }
                            }
                            newWeights[j, k] = parents[activeParentIndex].layers[i].Weights[j, k];//copy the value from the active aprent
                        }
                    }
                    newLayers[i] = new Layer(parents[0].hiddenLayerSize, parents[0].numOutputs, newWeights);//assign new weights to newLayers array in the form of a newly instantiated Layer
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }

            children[childIter] = new NeuralNet(parents[0].numInputs, parents[0].numOutputs, parents[0].numHiddenLayers, parents[0].hiddenLayerSize, parents[0].TestInputSets, parents[0].TestOutputSets)//create empty with same params as parent
            {
                layers = newLayers//assign the data to complete the child
            };
        }
        //convert array of type NeuralNet to type IGeneticIndividual
        IGeneticIndividual[] IChildren = new IGeneticIndividual[numChildren];
        for (int i = 0; i < numChildren; i++)
        {
            IChildren[i] = children[i] as IGeneticIndividual;
        }
        return IChildren;
    }

    public void Mutate()//set a single random weight to a random value from 0 to 1
    {
        //layer to mutate//
        int i = (int)(RandHolder.NextDouble()*(numHiddenLayers + 1));//+1 for output
        //node to mutate//
        int j = -1;//should cause an index out of bounds if not set by subsequent if cases
        if (i == 0) {//if first hidden layer (same as else case)(same number of nodes as hidden layer so doesn't matter)
            j = (int)(RandHolder.NextDouble() * hiddenLayerSize);
        }
        else if (i == numHiddenLayers) {//if output layer
            j = (int)(RandHolder.NextDouble() * numOutputs);
        }
        else {//if any other hidden layer
            j = (int)(RandHolder.NextDouble() * hiddenLayerSize);
        }
        //weight to mutate//
        int k = -1;//should cause an index out of bounds if not set by subsequent if cases
        if (i == 0)//if first hidden layer (same as else case)(same number of nodes as hidden layer so doesn't matter)
        {
            k = (int)(RandHolder.NextDouble() * (numInputs + 1));//+1 for bias
        }
        else if (i == numHiddenLayers)//if output layer same as else case
        {
            k = (int)(RandHolder.NextDouble() * (hiddenLayerSize) + 1);//+1 for bias
        }
        else//if any other hidden layer
        {
            k = (int)(RandHolder.NextDouble() * (hiddenLayerSize) + 1);//+1 for bias
        }
        //set new value;
        double currentValue = Layers[i].Weights[j, k];
        double newValue = (RandHolder.NextDouble() + 0.5)*currentValue; //[0.5, 1.5)
        if(RandHolder.NextDouble() > 0.5)
        {
            newValue = -newValue;//(-1.5, -0.5] union [0.5, 1.5)
        }
        Layers[i].Weights[j, k] = newValue;
    }

    public int CompareTo(IGeneticIndividual obj)//to be used by Arrays.sort in GeneticAlgorithm.cs>FitnessSort()
    {
        if(this.Fitness()>obj.Fitness())
        {
            return -1;//precede in sort order
        }
        else if(this.Fitness() < obj.Fitness())
        {
            return 1;//succeed in sort order
        }
        else
        {
            return 0;//equal in sort
        }
    }

    public int CompareTo(object obj)//should never be called because should never be compared to other data types but needs to be here to satisfy compiler
    {
        if (obj is IGeneticIndividual)
        {
            IGeneticIndividual GAObj = (IGeneticIndividual)obj;
            if (this.Fitness() > GAObj.Fitness())
            {
                return -1;//precede in sort order
            }
            else if (this.Fitness() < GAObj.Fitness())
            {
                return 1;//succeed in sort order
            }
            else
            {
                return 0;//equal in sort
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public void WriteToFile(string directory, string filename)
    {
        //create empty file
        string filetype = ".csv";
        string filepath = directory + "/" + filename + filetype;
        System.IO.File.WriteAllText(filepath, "");

        //fill with net dimensions
        string header = "numInputs, numOutputs, numHiddenLayers, hiddenLayerSize";
        string record = numInputs.ToString() + ", " + numOutputs.ToString() + ", " + numHiddenLayers.ToString() + ", " + hiddenLayerSize.ToString();
        System.IO.File.WriteAllLines(filepath, new string[] { header, record });

        //fill with net weights///////////////////////
        //first hidden layer
        string[] contents = new string[hiddenLayerSize + 1];//number of nodes in layer +1 for header
        contents[0] = "hidden layer 0";//header
        for (int i = 0; i < hiddenLayerSize; i++)//for each node in layer
        {
            string[] row = new string[numInputs + 1];//number of previous nodes +1 for bias
            for (int j = 0; j < numInputs + 1; j++)//for each node in previous layer
            {
                row[j] = layers[0].weights[i, j].ToString();
            }
            record = string.Join(", ", row);
            contents[i+1] = record;//+1 for header
        }
        AppendAllLines(filepath, contents);

        //hidden layers
        for (int layerIndex = 1; layerIndex < numHiddenLayers; layerIndex++)//for each hidden layer excluding the first which is handled above
        {
            contents = new string[hiddenLayerSize + 1];//number of nodes in layer +1 for header
            contents[0] = "hidden layer "+ layerIndex.ToString();//header
            for (int i = 0; i < hiddenLayerSize; i++)//for each node in layer
            {
                string[] row = new string[hiddenLayerSize + 1];//number of previous nodes +1 for bias
                for (int j = 0; j < hiddenLayerSize + 1; j++)//for each node in previous layer
                {
                    row[j] = layers[layerIndex].weights[i, j].ToString();
                }
                record = string.Join(", ", row);
                contents[i + 1] = record;//+1 for header
            }
            AppendAllLines(filepath, contents);
        }
        //output layer
        contents = new string[numOutputs + 1];//number of nodes in layer +1 for header
        contents[0] = "output layer";//header
        for (int i = 0; i < numOutputs; i++)//for each node in layer
        {
            string[] row = new string[hiddenLayerSize + 1];
            for (int j = 0; j < hiddenLayerSize + 1; j++)//for each node in previous layer
            {
                row[j] = layers[layers.Length-1].weights[i, j].ToString();
            }
            record = string.Join(", ", row);
            contents[i + 1] = record;//+1 for header
        }
        AppendAllLines(filepath, contents);
        ///////////////////////////////////



    }

    public static NeuralNet ReadFromFile(string filepath)
    {
        return null;
    }

    private static void AppendAllLines(string filepath, string[] contents)
    {
        for (int i = 0; i < contents.Length; i++)
        {
            System.IO.File.AppendAllText(filepath, contents[i] + "\n");
        }
    }
}