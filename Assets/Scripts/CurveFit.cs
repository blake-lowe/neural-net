using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class CurveToFit
{
    public static double Function(double x)
    {
        double y = x;                           //change function here
        return y;
    }
}

public class CurveFit : MonoBehaviour {

    public GameObject prefab;
    public double coordinateScale;
    public double min;
    public double max;
    public int numPoints;

    public int numHiddenLayers;
    public int hiddenLayerSize;
    public double learningRate;

    private GameObject[] CurvePoints;
    private GameObject[] NetPoints;
    private NeuralNet net;

	// Use this for initialization
	void Start () {
        CurvePoints = new GameObject[numPoints];
        for (int i = 0; i < CurvePoints.Length; i++)
        {
            CurvePoints[i] = Instantiate(prefab);
        }
        updateCurvePoints();

        net = new NeuralNet(1, 1, numHiddenLayers, hiddenLayerSize, learningRate);

        NetPoints = new GameObject[numPoints];
        for (int i = 0; i < NetPoints.Length; i++)
        {
            NetPoints[i] = Instantiate(prefab);
        }
        updateNetPoints();

        //backpropagate
        for (int i = 0; i < 1; i++)
        {
            for (int j = 0; j < numPoints; j++)
            {
                double x = min + j * ((max - min) / numPoints);
                double y = CurveToFit.Function(x);
                net.Backpropagate(new double[] { x }, new double[] { y });
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void updateCurvePoints ()
    {
        for (int i = 0; i < CurvePoints.Length; i++)
        {
            double x = min + i * ((max - min)/numPoints);
            double y = CurveToFit.Function(x);
            Debug.Log(y);
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
            NetPoints[i].GetComponent<Transform>().position = new Vector3((float)(coordinateScale * x), (float)(coordinateScale * (float)y), 0);
        }
    }
}
