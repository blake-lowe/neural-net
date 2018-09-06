using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetController : MonoBehaviour {

    private readonly NeuralNet net = new NeuralNet(1,3,5,5);//not serialized
    
    public GameObject prefab;//serialized
    public GameObject textPrefab;//serialized

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

        double[] outputs = net.FeedForward(new double[] { 0.5 });

        for (int i = 0; i < net.NumHiddenLayers; i++)//hidden layers
        {
            for (int j = 0; j < net.HiddenLayerSize; j++)
            {
                GameObject temp = Instantiate(prefab, new Vector3(2*i, 0, 2*j), Quaternion.identity);
                GameObject text = Instantiate(textPrefab, temp.transform);
                nodes[i, j] = text;
            }
        }
        for (int j = 0; j < net.NumOutputs; j++)//output layer
        {
            GameObject temp = Instantiate(prefab, new Vector3(2 * net.NumHiddenLayers, 0, 2 * j), Quaternion.identity);
            GameObject text = Instantiate(textPrefab, temp.transform);
            nodes[net.NumHiddenLayers, j] = text;
        }


        UpdateValues();
        Debug.Log(outputs[0]);
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
                nodes[i, j].GetComponent<TMPro.TextMeshPro>().text = net.Layers[i].Values[j].ToString();
            }
        }
        for (int j = 0; j < net.NumOutputs; j++)//output layer
        {
            nodes[net.NumHiddenLayers, j].GetComponent<TMPro.TextMeshPro>().text = net.Layers[net.NumHiddenLayers].Values[j].ToString();
        }
    }
}
