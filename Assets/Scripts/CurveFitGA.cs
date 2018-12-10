using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class CurveToFitGA//this is the function to approximate//TODO
{
    public static double Function(double x)
    {
        double y = x;                           //change function here
        return (double)y;
    }
}

public class CurveFitGA : MonoBehaviour
{//a class to test backpropagation of NeuralNet by approximating a function defined above.

    public VisualNet VNet;
    public GameObject netPrefab;
    public GameObject curvePrefab;
    public double coordinateScale;
    public double min;
    public double max;
    public int numPoints;

    public int numHiddenLayers;
    public int hiddenLayerSize;
    public double learningRate;//won't affect anything

    public int populationSize;          //total number of individuals
    public int numParents;              //number of parents per reproduction set
    public int numCrossoverPoints;      //number of crossover points in genetic combination. Will be used in GeneticIndividual.Reproduce(). should be x >= numParents.
    public float mutationChance;        //from 0 to 1. measure of genetic diversity. 0 is no mutation. 1 will cause an infinite loop after changing every weight.
    public float environmentalPressure; //from 0 to 1. 1 is no survivors
    public int tournamentSize;          //size of the randomly chosen subset from which the most fit individual will be chosen for reproduction.  must be 1 <= x <= populationSize.
    public int numGenerations;          //how much training to do
    

    private GameObject[] CurvePoints;
    private GameObject[] NetPoints;
    private NeuralNet net;

    // Use this for initialization
    void Start()
    {
        CurvePoints = new GameObject[numPoints];
        for (int i = 0; i < CurvePoints.Length; i++)
        {
            CurvePoints[i] = Instantiate(curvePrefab);
        }
        updateCurvePoints();

        net = new NeuralNet(1, 1, numHiddenLayers, hiddenLayerSize, learningRate);

        NetPoints = new GameObject[numPoints];
        for (int i = 0; i < NetPoints.Length; i++)
        {
            NetPoints[i] = Instantiate(netPrefab);
        }
        updateNetPoints();

        VNet.net = net;
        VNet.Initialize();

        //training
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void backpropagate()
    {
        for (int i = 0; i < 1; i++)
        {
            for (int j = 0; j < numPoints; j++)
            {
                double x = min + j * ((max - min) / numPoints);
                double y = CurveToFit.Function(x);
                net = net.Backpropagate(new double[] { x }, new double[] { y });
                VNet.net = net;
                updateNetPoints();
            }
        }
    }

    void updateCurvePoints()
    {
        for (int i = 0; i < CurvePoints.Length; i++)
        {
            double x = min + i * ((max - min) / numPoints);
            double y = CurveToFit.Function(x);
            CurvePoints[i].GetComponent<Transform>().position = new Vector3((float)(coordinateScale * x), (float)(coordinateScale * (float)y), 0);

        }
    }

    void updateNetPoints()
    {
        for (int i = 0; i < NetPoints.Length; i++)
        {
            double x = min + i * ((max - min) / numPoints);
            double[] inputs = new double[1];
            inputs[0] = x;
            double[] outputs = net.FeedForward(inputs);
            double y = outputs[0];
            //Debug.Log(x);
            //Debug.Log(y);
            NetPoints[i].GetComponent<Transform>().position = new Vector3((float)(coordinateScale * x), (float)(coordinateScale * (float)y), 0);
        }
    }
}
