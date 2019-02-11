using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardCodeNNTest : MonoBehaviour {

    //input//   //weight0 = 0   //value//
    //          //weight1 = 1   

    // Use this for initialization
    public VisualNet VNet;
	void Start () {
        Layer[] layers = new Layer[2];
        double[,] weights = new double[1, 2];
        weights[0, 0] = 0;
        weights[0, 1] = 1;
        layers[0] = new Layer(1, 1, weights);
        weights = new double[1, 2];
        weights[0, 0] = 0;
        weights[0, 1] = 1;
        layers[1] = new Layer(1, 1, weights);
		NeuralNet net = new NeuralNet(1, 1, 1, 1, layers);

        VNet.net = net;
        VNet.Initialize();

        Debug.Log(net.FeedForward(new double[] { 0 })[0]);
        Debug.Log(net.FeedForward(new double[] { 0.5 })[0]);
        Debug.Log(net.FeedForward(new double[] { 1 })[0]);
    }

}
