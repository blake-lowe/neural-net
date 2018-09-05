using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetController : MonoBehaviour {

    private readonly NeuralNet net = new NeuralNet(1,1,2,3);
    [SerializeField]
    public GameObject prefab;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < net.NumHiddenLayers; i++)
        {
            for (int j = 0; j < net.HiddenLayerSize; j++)
            {
                Instantiate(prefab, new Vector3(2*i, 0, 2*j), Quaternion.identity);
            }
        }
        double[] outputs = net.FeedForward(new double[] { 0.5 });
        Debug.Log(outputs[0]);
        Debug.Log(net.Layers[0].Weights[0, 0]);
        Debug.Log(net.Layers[0].Weights[0, 1]);
        Debug.Log(net.Layers[1].Weights[0, 0]);
        Debug.Log(net.Layers[1].Weights[0, 1]);
    }

	// Update is called once per frame
	void Update ()
    {
		
	}
}
