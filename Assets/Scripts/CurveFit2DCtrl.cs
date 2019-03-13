using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveFit2DCtrl : MonoBehaviour {

    public int numHiddenLayers; //NN dimensions
    public int hiddenLayerSize; //

    public int populationSize;          //total number of individuals
    public int numParents;              //number of parents per reproduction set
    public int numCrossoverPoints;      //number of crossover points in genetic combination. Will be used in GeneticIndividual.Reproduce(). should be x >= numParents.
    public float mutationChance;        //from 0 to 1. measure of genetic diversity. 0 is no mutation. 1 will cause an infinite loop after changing every weight.
    public float environmentalPressure; //from 0 to 1. 1 is no survivors
    public float eliteFraction;         //number of solutions to save from one generation to the next 0 is none 1 is all saved. (should never be > 1-environmentalPressure then errors) (if > like .2 then GA won't work well)
    public int tournamentSize;          //size of the randomly chosen subset from which the most fit individual will be chosen for reproduction.  must be 1 <= x <= populationSize.

    private GeneticAlgorithm ga;

    public int numPoints;   //
    public int numTestPoints;   //

    private double[,] testInputSets;
    private double[,] testOutputSets;

    public string function;//name of function to be approximated

    public NeuralNet bestNet;

    public VisualNet visualNet;
    public float vNetXArea;
    public float vNetYArea;

	// Use this for initialization
	void Start ()
    {
        testInputSets = new double[numTestPoints, 1];
        testOutputSets = new double[numTestPoints, 1];
        for (int i = 0; i < numTestPoints; i++)
        {
            testInputSets[i, 0] = 0 + i * (1 - 0) / numTestPoints;
            testOutputSets[i, 0] = functionEvaluate((float)testInputSets[i, 0]);
        }

        NeuralNet net = new NeuralNet(1, 1, numHiddenLayers, hiddenLayerSize, testInputSets, testOutputSets);//create net with test sets filled

        visualNet.net = net;
        visualNet.layerSeparation = vNetXArea / (numHiddenLayers + 1);
        visualNet.nodeSeparation = vNetYArea / (hiddenLayerSize + 1);
        visualNet.Initialize();
    }

	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private float functionEvaluate(float x)
    {
        if (function == "constant") { return 0.5f; }
        else if (function == "linear"){ return x; }
        else { return 0f; }
    }
}
