using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class CurveToFitGA//this is the function to approximate
{
    public static double Function(double x)
    {
        return x;  //change function he
    }
}

public class CurveFitGA : MonoBehaviour
{//a class to test training with a GeneticAlgorithm of NeuralNet by approximating a function defined above.

    public VisualNet VNet;//for visualization
    public GameObject netPrefab;//an object to create for each point
    public GameObject curvePrefab;//an object to create for each point
    public double coordinateScale;//scale factor
    public double min;      //for the generation of curve points
    public double max;      //
    public int numPoints;   //

    public double testMin;      //for creation of testI/O sets
    public double testMax;      //
    public int numTestPoints;   //

    public int numHiddenLayers; //NN dimensions
    public int hiddenLayerSize; //

    public int populationSize;          //total number of individuals
    public int numParents;              //number of parents per reproduction set
    public int numCrossoverPoints;      //number of crossover points in genetic combination. Will be used in GeneticIndividual.Reproduce(). should be x >= numParents.
    public float mutationChance;        //from 0 to 1. measure of genetic diversity. 0 is no mutation. 1 will cause an infinite loop after changing every weight.
    public float environmentalPressure; //from 0 to 1. 1 is no survivors
    public float eliteFraction;         //number of solutions to save from one generation to the next 0 is none 1 is all saved. (should never be > 1-environmentalPressure then errors) (if > like .2 then GA won't work well)
    public int tournamentSize;          //size of the randomly chosen subset from which the most fit individual will be chosen for reproduction.  must be 1 <= x <= populationSize.
    public int numGenerationsPerSecond;          //how much training to do
    

    private GameObject[] CurvePoints;
    private GameObject[] NetPoints;
    private NeuralNet net;

    private GeneticAlgorithm ga;
    private double[,] testInputSets;
    private double[,] testOutputSets;

    private float nextGenerationTime;

    public AnimationCurve plotBest = new AnimationCurve();
    public AnimationCurve plotWorst = new AnimationCurve();

    // Use this for initialization
    void Start()
    {
        //create actual curve visualization
        CurvePoints = new GameObject[numPoints];
        for (int i = 0; i < CurvePoints.Length; i++)
        {
            CurvePoints[i] = Instantiate(curvePrefab);
        }
        updateCurvePoints();

        testInputSets = new double[numTestPoints, 1];
        testOutputSets = new double[numTestPoints, 1];
        for (int i = 0; i < numTestPoints; i++)
        {
            testInputSets[i, 0] = testMin + i * (testMax - testMin) / numTestPoints;
            testOutputSets[i, 0] = CurveToFitGA.Function(testInputSets[i, 0]);
        }

        net = new NeuralNet(1, 1, numHiddenLayers, hiddenLayerSize, testInputSets, testOutputSets);//create net with test sets filled

        //create visualization of the net points
        NetPoints = new GameObject[numPoints];
        for (int i = 0; i < NetPoints.Length; i++)
        {
            NetPoints[i] = Instantiate(netPrefab);
        }
        updateNetPoints(net);

        VNet.net = net;
        VNet.Initialize();

        ga = new GeneticAlgorithm(net, populationSize, numParents, environmentalPressure, eliteFraction, numCrossoverPoints, mutationChance, tournamentSize);
    }

    // Update is called once per frame
    void Update()
    {
        float secondsPerGeneration = 1 / numGenerationsPerSecond;
        if (Time.time > nextGenerationTime)
        {
            //training
            if (ga != null)
            {
                net = (NeuralNet)ga.TrainGeneration(1);
                updateNetPoints(net);
                VNet.net = net;
                float bestFitnessNow = (float)ga.individuals[0].Fitness();
                float worstFitnessNow = (float)ga.individuals[populationSize - 1].Fitness();
                plotBest.AddKey(Time.realtimeSinceStartup, bestFitnessNow);
                plotWorst.AddKey(Time.realtimeSinceStartup, worstFitnessNow);
                nextGenerationTime += secondsPerGeneration;
            }
        }


    }

    void updateCurvePoints()
    {
        for (int i = 0; i < CurvePoints.Length; i++)
        {
            double x = min + i * ((max - min) / numPoints);
            double y = CurveToFitGA.Function(x);
            CurvePoints[i].GetComponent<Transform>().position = new Vector3((float)(coordinateScale * x), (float)(coordinateScale * (float)y), 0);

        }
    }

    void updateNetPoints(NeuralNet netarg)
    {
        for (int i = 0; i < NetPoints.Length; i++)
        {
            double x = min + i * ((max - min) / numPoints);
            double[] inputs = new double[1];
            inputs[0] = x;
            double[] outputs = netarg.FeedForward(inputs);
            double y = outputs[0];
            NetPoints[i].GetComponent<Transform>().position = new Vector3((float)(coordinateScale * x), (float)(coordinateScale * (float)y), 0);
        }
    }
}