using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ValuePointImage : MonoBehaviour
{
    [SerializeField] private TMP_Text valueText;

    public void SetValue(float value)
    {
        valueText.text = value.ToString();
    }

    public string GetValue()
    {
        return valueText.text;
    }
}
