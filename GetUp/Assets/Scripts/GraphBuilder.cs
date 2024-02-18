using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class GraphBuilder : MonoBehaviour
{
    [SerializeField] private GameObject graphPointPrefab;
    [SerializeField] private GameObject graphLinePrefab;
    [SerializeField] private GameObject goalLine;

    private List<GameObject> graphPoints;
    private float height;
    private float width;
    private float xStep;
    private float yStep;
    private ExercisesData exersicesData;

    private void Awake()
    {
        exersicesData = FindObjectOfType<ExercisesData>();
        graphPoints = new List<GameObject>();
        height = GetComponent<RectTransform>().rect.height;
        width = GetComponent<RectTransform>().rect.width;
    }

    private void OnEnable()
    {
        RemoveGraphPoints();
    }

    public IEnumerator StartBuildingGraph(List<float> values)
    {
        // Wait for the graph points to be instantiated
        yield return new WaitUntil(() => graphPoints != null);
        BuildGraph(values);
        BuildAverageGraph();
    }

    public void BuildGraph(List<float> values)
    {
        float maxValue = GetMaxValue();
        if (maxValue == 0)
        {
            maxValue = Mathf.Max(values.ToArray());
        }
        CalculateGraphSize(values.Count, maxValue);
        SetGoalLine(yStep);
        List<GameObject> newGraphPoints = new List<GameObject>();
        for (int i = 0; i < values.Count; i++)
        {
            GameObject graphPoint = Instantiate(graphPointPrefab, transform);
            graphPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * xStep, values[i] * yStep);
            graphPoint.GetComponent<ValuePointImage>().SetValue(values[i]);
            graphPoints.Add(graphPoint);
            newGraphPoints.Add(graphPoint);
            // Draw lines between points
            if (i > 0)
            {
                GameObject graphLine = Instantiate(graphLinePrefab, transform);
                Vector2 previousPointPosition = newGraphPoints[i-1].GetComponent<RectTransform>().anchoredPosition;
                Vector2 currentPointPosition = graphPoint.GetComponent<RectTransform>().anchoredPosition;

                Vector2 linePosition = (previousPointPosition + currentPointPosition) / 2;
                graphLine.GetComponent<RectTransform>().anchoredPosition = linePosition;

                Vector2 direction = currentPointPosition - previousPointPosition;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                graphLine.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);

                float distance = Vector2.Distance(previousPointPosition, currentPointPosition);
                graphLine.GetComponent<RectTransform>().sizeDelta = new Vector2(distance, 10);
                
                graphLine.transform.SetAsFirstSibling();
            }
        }
    }

    public void BuildAverageGraph()
    {
        List<float> values = new List<float>();
        List<Report> dayStatsReports = exersicesData.GetDayStatsReports();
        foreach (Report dayStatsReport in dayStatsReports)
        {
            float value = exersicesData.GetAverageScore(dayStatsReport.date);
            if (value == 0)
            {
                value = float.Parse(dayStatsReport.dayPoints);
            }
            values.Add(value);
        }
        BuildGraph(values);
    }

    private void CalculateGraphSize(int valuesCount, float maxValue)
    {
        float spareSpase = 60f;
        xStep = width / (exersicesData.GetDaysCount() - 1);
        yStep = (height - spareSpase) / maxValue;
    }

    private void SetGoalLine(float yStep)
    {
        goalLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yStep * 20);
    }

    private void RemoveGraphPoints()
    {
        if (graphPoints.Count == 0)
        {
            return;
        }
        foreach (GameObject graphPoint in graphPoints)
        {
            Destroy(graphPoint);
        }
        graphPoints.Clear();
    }

    private float GetMaxValue()
    {
        float maxValue = 0;
        foreach (GameObject graphPoint in graphPoints)
        {
            float value = float.Parse(graphPoint.GetComponent<ValuePointImage>().GetValue());
            if (value > maxValue)
            {
                maxValue = value;
            }
        }
        return maxValue;
    }
}
