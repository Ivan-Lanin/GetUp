using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    [SerializeField] private GraphBuilder graphBuilder;

    private ExercisesData exersicesData;
    private List<Report> DayStatsReport;

    private void Awake()
    {
        exersicesData = FindObjectOfType<ExercisesData>();
    }

    private void OnEnable()
    {
        DayStatsReport = exersicesData.GetDayStatsReports();
        List<float> values = new List<float>();
        foreach (Report report in DayStatsReport)
        {
            if (report.dayPoints.Contains(','))
            {
                report.dayPoints = report.dayPoints.Replace(',', '.');
            }
            values.Add(float.Parse(report.dayPoints, CultureInfo.InvariantCulture));
            //Application.OpenURL("https://www.google.com/search?q=" + float.Parse(report.dayPoints).ToString());
        }
        StartCoroutine(graphBuilder.StartBuildingGraph(values));
    }
}
