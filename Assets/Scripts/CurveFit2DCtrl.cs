using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject configPanel;
    public InputField populationField;
    public InputField numParentsField;
    public InputField numCrossoverPointsField;
    public InputField environmentalPressureField;
    public InputField eliteFractionField;
    public InputField tournamentSizeField;

    public InputField numHiddenLayersField;
    public InputField hiddenLayerSizeField;
    public InputField numPointsField;
    public InputField numTestPointsField;
    public Dropdown functionDropdown;

    // Use this for initialization
    void Start()
    {
        configPanel.SetActive(true);
    }

    private void setupUI()
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
        else if (function == "negativeLinear") { return 1f - x; }
        else if (function == "piecewiseLinear")
        {
            if(x >= 0 && x < 0.5f) { return 2f * x; }
            else if(x >= 0.5f && x < 0.625f) { return 3f - 4f * x; }
            else if(x >= 0.625f && x < 0.75f) { return -0.75f + 2f * x; }
            else if(x >= 0.75f && x <= 1f) { return 2.25f - 2f * x; }
            else { return 0f; }
        }
        else if (function == "quadratic") { return x*x; }
        else if (function == "cubic") { return x*x*x; }
        else if (function == "squareRoot") { return Mathf.Sqrt(x); }
        else if (function == "exponential") { return Mathf.Exp(x); }
        else if (function == "sine") { return Mathf.Sin(x); }
        else if (function == "cosine") { return Mathf.Cos(x); }
        else { return 0f; }
    }

    public void configDone()
    {
        setupUI();
        configPanel.SetActive(false);
    }
}
