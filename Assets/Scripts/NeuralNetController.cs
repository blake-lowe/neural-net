using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetController : MonoBehaviour {

    private NeuralNet net = new NeuralNet(1,1,1,1,0.1);//not serialized//CHANGE DIMENSIONS HERE
    
    public GameObject prefab;//serialized
    public GameObject textPrefab;//serialized

    public Color Color0;  //limit of nodes and weights color when they approach a value of 0
    public Color Color1;  //limit of nodes and weights color when they approach a value of 1

    private GameObject[,] nodes;

    private void Awake()
    {
        if (net.HiddenLayerSize >= net.NumOutputs)//will run most of the time
        {
            nodes = new GameObject[net.NumHiddenLayers + 1, net.HiddenLayerSize];
        }
        else//will only run if there are more outputs than nodes per hidden layer
        {
            nodes = new GameObject[net.NumHiddenLayers + 1, net.NumOutputs];
        }
        
    }
    // Use this for initialization
    void Start()
    {

        double[] outputs = net.FeedForward(new double[] { 0.1 });

        //create an array with nodes GameObjects//
        for (int i = 0; i < net.NumHiddenLayers; i++)//hidden layers
        {
            for (int j = 0; j < net.HiddenLayerSize; j++)
            {
                //instantiate cube
                GameObject temp = Instantiate(prefab, new Vector3(2*i, 0, 2*j), Quaternion.identity);
                //instantiate text as child of cube
                GameObject text = Instantiate(textPrefab, temp.transform);
                nodes[i, j] = temp;
            }
        }
        for (int j = 0; j < net.NumOutputs; j++)//output layer
        {
            //instantiate cube
            GameObject temp = Instantiate(prefab, new Vector3(2 * net.NumHiddenLayers, 0, 2 * j), Quaternion.identity);
 
           
            //instantiate text as child of cube
            GameObject text = Instantiate(textPrefab, temp.transform);
            nodes[net.NumHiddenLayers, j] = temp;
        }


        UpdateValues();//change values and colors
        Debug.Log(outputs[0]);
        for (int i = 0; i < 5; i++)
        {
            net = net.Backpropagate(new double[1] { 0.2 });//at this line, the values of net become zero. //because the sigmoid of zero is 0.5, no matter the input the net returns 0.5
            outputs = net.FeedForward(new double[] { 0.1 });
            Debug.Log(outputs[0]);
            //UpdateValues();
        }


        
    }

	// Update is called once per frame
	void Update ()
    {
		
	}

    void UpdateValues()
    {
        for (int i = 0; i < net.NumHiddenLayers; i++)
        {
            for (int j = 0; j < net.HiddenLayerSize; j++)
            {
                double value = net.Layers[i].Values[j];
                nodes[i, j].GetComponentInChildren<TMPro.TextMeshPro>().text = value.ToString();
                nodes[i, j].GetComponent<Renderer>().material.SetColor("_Color", Helper.InterpolateColor(Color0, Color1, value));
            }
        }
        for (int j = 0; j < net.NumOutputs; j++)//output layer
        {
            double value = net.Layers[net.NumHiddenLayers].Values[j];
            nodes[net.NumHiddenLayers, j].GetComponentInChildren<TMPro.TextMeshPro>().text = value.ToString();
            nodes[net.NumHiddenLayers, j].GetComponent<Renderer>().material.SetColor("_Color", Helper.InterpolateColor(Color0, Color1, value));
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