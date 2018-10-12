using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNet : MonoBehaviour {

    public Vector3 origin;
    public float layerSeparation;
    public float nodeSeparation;

    public GameObject nodeParent;//should be empty gameObject
    public GameObject weightParent;//should be empty gameObject
    public GameObject nodePrefab;
    public GameObject weightPrefab;

    public Color Color0;  //limit of nodes and weights color when they approach a value of 0
    public Color Color1;  //limit of nodes and weights color when they approach a value of 1

    [System.NonSerialized]
    public NeuralNet net;
    private bool isVNetInitialized = false;
    private GameObject[,] nodes;
    private GameObject[,,] weights;
    // Use this for initialization
    public void Initialize()
    {
        //instantiate array
        nodes = new GameObject[net.NumHiddenLayers + 1, net.HiddenLayerSize];

        //fill array and set positions of prefabs
        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            for (int j = 0; j < nodes.GetLength(1); j++)
            {
                Vector3 position = new Vector3(origin.x + layerSeparation * (i + 1), origin.y + nodeSeparation * j, origin.z);
                nodes[i, j] = Instantiate(nodePrefab, position, Quaternion.identity, nodeParent.GetComponent<Transform>());
            }
        }
        //instantiate array
        weights = new GameObject[net.NumHiddenLayers + 1, net.HiddenLayerSize, net.HiddenLayerSize + 1];
        //fill array and set position vectors of lineRenderers
        for (int i = 0; i < weights.GetLength(0); i++)
        {
            for (int j = 0; j < weights.GetLength(1); j++)
            {
                for (int k = 0; k < weights.GetLength(2); k++)
                {
                    Vector3 position0 = new Vector3(origin.x + layerSeparation * (i + 1), origin.y + nodeSeparation * j, origin.z);//node
                    Vector3 position1 = new Vector3(origin.x + layerSeparation * i, origin.y + nodeSeparation * k, origin.z);//node previous
                    weights[i, j, k] = Instantiate(weightPrefab, weightParent.GetComponent<Transform>());
                    LineRenderer lr = weights[i, j, k].GetComponent<LineRenderer>();
                    lr.SetPosition(0, position0);
                    lr.SetPosition(1, position1);
                }
            }
        }
        isVNetInitialized = true;
    }
    // Update is called once per frame
    void Update () {//set colors
        if (net != null && isVNetInitialized)
        {
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    double value = net.Layers[i].Values[j];//grab value//error here due to output layer having fewer values (to fix. change all loops to skip output layer and don't even fill the array with prefabs in the first place
                    Color colorValue = Helper.InterpolateColor(Color0, Color1, value);//calculate color
                    nodes[i, j].GetComponent<Renderer>().material.SetColor("_Color", colorValue);//set color of material
                }
            }
            for (int i = 0; i < weights.GetLength(0); i++)
            {
                for (int j = 0; j < weights.GetLength(1); j++)
                {
                    for (int k = 0; k < weights.GetLength(2); k++)
                    {
                        double weight = net.Layers[i].Weights[j, k];//grab weight
                        Color colorWeight = Helper.InterpolateColor(Color0, Color1, weight);//calculate color
                        weights[i, j, k].GetComponent<LineRenderer>().material.SetColor("_Color", colorWeight);
                    }
                }
            }
        }
	}
}

static class Helper
{
    public static Color InterpolateColor(Color Color0, Color Color1, double x)
    {
        double r = ((x * (Color1.r - Color0.r)) + Color0.r);
        double g = ((x * (Color1.g - Color0.g)) + Color0.g);
        double b = ((x * (Color1.b - Color0.b)) + Color0.b);
        return new Color((float)r, (float)g, (float)b);

    }
}
