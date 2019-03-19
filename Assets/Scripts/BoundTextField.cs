using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoundTextField : MonoBehaviour {

    public InputField fieldToBound;
    public bool boundedBelow;
    public float min;
    public bool boundedAbove;
    public float max;

    public void setValue()
    {
        float value = float.Parse(fieldToBound.text);
        if (value > max && boundedAbove)
        {
            fieldToBound.text = max.ToString();
        }
        else if(value < min && boundedBelow)
        {
            fieldToBound.text = min.ToString();
        }
    }
}
