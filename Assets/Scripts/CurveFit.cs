using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class CurveToFit
{
    public static double Function(double x)
    {
        double y = Mathf.Pow((float) x,2);                           //change function here
        return (double) y;
    }
}

public class CurveFit : MonoBehaviour {

    public GameObject VisualNet;
    public GameObject netPrefab;
    public GameObject curvePrefab;
    public double coordinateScale;
    public double min;
    public double max;
    public int numPoints;

    public int numHiddenLayers;
    public int hiddenLayerSize;
    public int numBackpropagationPasses;
    public double learningRate;

    private GameObject[] CurvePoints;
    private GameObject[] NetPoints;
    private NeuralNet net;

	// Use this for initialization
	void Start () {



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

        for (int i = 0; i < numBackpropagationPasses; i++)
        {
            backpropagate();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
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
                updateNetPoints();
            }
        }
    }

    void updateCurvePoints ()
    {
        for (int i = 0; i < CurvePoints.Length; i++)
        {
            double x = min + i * ((max - min)/numPoints);
            double y = CurveToFit.Function(x);
            CurvePoints[i].GetComponent<Transform>().position = new Vector3((float)(coordinateScale*x), (float)(coordinateScale *(float)y), 0);

        }
    }

    void updateNetPoints ()
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
