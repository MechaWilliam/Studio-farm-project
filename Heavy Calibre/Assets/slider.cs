using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class slider : MonoBehaviour
{
    TMP_Text sliderPercentage;

    // Start is called before the first frame update
    void Start()
    {
        sliderPercentage = GetComponent<TMP_Text>();
        sliderPercentage.text = "100" + "%";
    }

    public void UpdateText(float value)
    {
        sliderPercentage.text = Mathf.RoundToInt(value * 100) + "%";
    }
}
