using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNetController : MonoBehaviour {

    public VisualNet vNet;
	// Use this for initialization
	void Start () {
        vNet.net = new NeuralNet(2, 2, 5, 5, 0.1);
        vNet.Initialize();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
