using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExersicePanelSimple : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private TMP_Text usefullnessText;

    public void SetExersice(Exercise exersice)
    {
        nameText.text = exersice.name;
        difficultyText.text = exersice.difficulty.ToString();
        usefullnessText.text = exersice.usefullness.ToString();
    }
}
