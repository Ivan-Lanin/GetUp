using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExercisesManager : MonoBehaviour
{
    [SerializeField] private GameObject ExersicePanelPrefab;
    [SerializeField] private Transform ExersicePanelParent;

    private ExercisesData exersicesData;

    void Start()
    {
        exersicesData = FindObjectOfType<ExercisesData>();
        List<Exercise> exersices = exersicesData.GetExersices();
        foreach (Exercise exersice in exersices)
        {
            GameObject exersicePanel = Instantiate(ExersicePanelPrefab, ExersicePanelParent);
            exersicePanel.GetComponent<ExersicePanelSimple>().SetExersice(exersice);
        }
    }
}
