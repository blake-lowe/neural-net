using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNetController : MonoBehaviour
{

    public VisualNet vNet;
    // Use this for initialization
    void Start()
    {
        vNet.net = new NeuralNet(1, 1, 4, 3, 0.1);
        vNet.net.FeedForward(new double[] { 0.1 });
        vNet.Initialize();
        for (int i = 0; i < 100; i++)
        {
            StartCoroutine(Backpropagate());

        }
        
    }

    IEnumerator Backpropagate()
    {
        yield return new WaitForSeconds(1);
        Debug.Log(vNet.net.FeedForward(new double[] { 0.1 })[0]);
        vNet.net = vNet.net.Backpropagate(new double[] { 0.1 }, new double[] { 0.1 });
    }
}
