using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardCodeNNTest : MonoBehaviour {

    //input//   //weight0 = 0   //value//
    //          //weight1 = 1   

    // Use this for initialization
    public VisualNet VNet;
	void Start () {

        PlayerPrefs.SetString("IOFilepath", "C:/Users/bolowe19/Downloads");
        
        /*
        Layer[] layers = new Layer[2];
        double[,] weights = new double[1, 2];
        weights[0, 0] = -0.75;
        weights[0, 1] = 1.7;
        layers[0] = new Layer(1, 1, weights);
        weights = new double[1, 2];
        weights[0, 0] = -0.25;
        weights[0, 1] = 3;
        layers[1] = new Layer(1, 1, weights);
		NeuralNet net = new NeuralNet(1, 1, 1, 1, layers);
        */

        NeuralNet net = NeuralNet.ReadFromFile(PlayerPrefs.GetString("IOFilepath"), "test1");

        //VNet.net = net;
        //VNet.Initialize();

        Debug.Log(net.FeedForward(new double[] { 0 })[0]);
        Debug.Log(net.FeedForward(new double[] { 0.5 })[0]);
        Debug.Log(net.FeedForward(new double[] { 1 })[0]);

        

        //net.WriteToFile(PlayerPrefs.GetString("IOFilepath"), "test1");
    }

}
