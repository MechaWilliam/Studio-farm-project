using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class slider : MonoBehaviour
{


    Text mainPercentage;



    // Start is called before the first frame update
    void Start()
    {
        mainPercentage = GetComponent<Text>();
        mainPercentage.text = ("100" + "%");

    }

    public void TextUpdate(float value)
    {
        mainPercentage.text = Mathf.RoundToInt(value * 100) + "%";
    }

}
