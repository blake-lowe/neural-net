using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNet : MonoBehaviour {

    public Vector3 origin;
    public float layerSeparation;
    public float nodeSeparation;

    public GameObject nodePrefab;
    public GameObject weightPrefab;

    public Color Color0;  //limit of nodes and weights color when they approach a value of 0
    public Color Color1;  //limit of nodes and weights color when they approach a value of 1

    [System.NonSerialized]
    public NeuralNet net;
    private GameObject[,] nodes;
    private GameObject[,,] weights;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {//set colors
        if (net != null)
        {
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    //todo
                }
            }
            for (int i = 0; i < weights.GetLength(0); i++)
            {
                for (int j = 0; j < weights.GetLength(1); j++)
                {
                    for (int k = 0; k < weights.Length; k++)
                    {
                        //todo
                    }
                }
            }
        }
	}
    public void Initiate()
    {
        nodes = new GameObject[net.NumHiddenLayers + 1, net.HiddenLayerSize];

        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            for (int j = 0; j < nodes.GetLength(1); j++)
            {
                Vector3 position = new Vector3(origin.x + layerSeparation * (i+1), origin.y + nodeSeparation * j, origin.z);
                nodes[i, j] = Instantiate(nodePrefab)//todo
            }
        }

        weights = new GameObject[net.NumHiddenLayers + 1, net.HiddenLayerSize, net.HiddenLayerSize + 1];

        for (int i = 0; i < weights.GetLength(0); i++)
        {
            for (int j = 0; j < weights.GetLength(1); j++)
            {
                for (int k = 0; k < weights.Length; k++)
                {
                    Vector3 position1 = new Vector3(origin.x + layerSeparation * (i + 1), origin.y + nodeSeparation * j, origin.z);//node
                    Vector3 position2 = new Vector3(origin.x + layerSeparation * i, origin.y + nodeSeparation * k, origin.z);//node previous
                    weights[i, j, k] = Instantiate(weightPrefab);
                    //todo //add to array
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
