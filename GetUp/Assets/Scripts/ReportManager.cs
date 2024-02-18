using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the creation and display of exercise reports for a specific date.
/// </summary>
public class ReportManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField day;
    [SerializeField] private TMP_InputField month;
    [SerializeField] private TMP_InputField year;
    [SerializeField] private Button addButton;
    [SerializeField] private Button nextDayButton;
    [SerializeField] private Button previousDayButton;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text averageScoreText;
    [SerializeField] private GameObject ExersicePanelPrefab;
    [SerializeField] private Transform ExersicePanelParent;

    private float score = 0;
    private List<ExercisePanel> exersicePanels;
    private ExercisesData exersicesData;

    /// <summary>
    /// Initializes the ReportManager by finding the ExersicesData instance.
    /// </summary>
    private void Awake()
    {
        exersicePanels = new List<ExercisePanel>();
        exersicesData = FindObjectOfType<ExercisesData>();
    }

    /// <summary>
    /// Subscribes date input fields to the OnDateEdited method and initializes exercise panels.
    /// </summary>
    private void Start()
    {
        day.onSubmit.AddListener(delegate { OnDateEdited(); });
        month.onSubmit.AddListener(delegate { OnDateEdited(); });
        year.onSubmit.AddListener(delegate { OnDateEdited(); });
        addButton.onClick.AddListener(OnAddExersicePanel);
        nextDayButton.onClick.AddListener(delegate { TurnDayPage(1); });
        previousDayButton.onClick.AddListener(delegate { TurnDayPage(-1); });

        InstantiateExersices();
        UpdateScore();
    }

    /// <summary>
    /// Sets the current date when the ReportManager is enabled and updates the score.
    /// </summary>
    private void OnEnable()
    {
        day.text = DateTime.Now.ToString("dd");
        month.text = DateTime.Now.ToString("MM");
        year.text = DateTime.Now.ToString("yyyy");
        InstantiateExersices();
        UpdateScore();
    }

    /// <summary>
    /// Clears exercise panels when the ReportManager is disabled.
    /// </summary>
    private void OnDisable()
    {
        RemoveExersicePanels();
    }

    /// <summary>
    /// Invoked when the date input fields are edited. Instantiates exercise panels and updates the score.
    /// </summary>
    public void OnDateEdited()
    {
        if (GetSelectedDate() == "") { return; }
        InstantiateExersices();
        UpdateScore();
    }

    /// <summary>
    /// Calculates and updates the total score based on exercise panels.
    /// </summary>
    public void UpdateScore()
    {
        score = 0;
        foreach (ExercisePanel exersicePanel in exersicePanels)
        {
            score += exersicePanel.GetPoints();
        }
        scoreText.text = score.ToString();
        exersicesData.UpdateReports(GetSelectedDate(), score.ToString());
        UpdateAverageScore();
    }

    public void OnAddExersicePanel()
    {
        GameObject exersicePanel = Instantiate(ExersicePanelPrefab, ExersicePanelParent);
        exersicePanels.Add(exersicePanel.GetComponent<ExercisePanel>());
    }

    /// <summary>
    /// Turns the day page by a specified amount of days.
    /// </summary>
    private void TurnDayPage(int days)
    {
        DateTime date = DateTime.ParseExact(GetSelectedDate(), "dd.MM.yy", null);
        date = date.AddDays(days);
        day.text = date.ToString("dd");
        month.text = date.ToString("MM");
        year.text = date.ToString("yyyy");
        OnDateEdited();
    }

    /// <summary>
    /// Updates the average score and displays it.
    /// </summary>
    /// <param name="selectedDay">The selected day to calculate the average score for.</param>
    /// <returns>The calculated average score.</returns>
    public float UpdateAverageScore(string selectedDay = null)
    {
        if (selectedDay == null)
        {
            selectedDay = GetSelectedDate();
        }

        float averageScore = exersicesData.GetAverageScore(selectedDay);
        averageScoreText.text = averageScore.ToString();

        return averageScore;
    }

    /// <summary>
    /// Removes all exercise panels from the list and destroys their game objects.
    /// </summary>
    public void RemoveExersicePanels()
    {
        foreach (ExercisePanel panel in exersicePanels)
        {
            Destroy(panel.gameObject);
        }
        exersicePanels = new List<ExercisePanel>();
    }

    /// <summary>
    /// Retrieves the formatted selected date.
    /// </summary>
    /// <returns>The formatted selected date as a string.</returns>
    public string GetSelectedDate()
    {
        if (int.Parse(day.text) > 31) { return "???"; }
        if (int.Parse(month.text) > 12) { return "???"; }
        if (int.Parse(year.text) > 2200 || int.Parse(year.text) < 2000) { return "???"; }
        if (day.text.Length < 2) { day.text = "0" + day.text; }
        if (month.text.Length < 2) { month.text = "0" + month.text; }

        string dayFormatted = day.text + '.' + month.text + '.' + year.text.Substring(2, 2);
        return dayFormatted;
    }

    /// <summary>
    /// Instantiates exercise panels based on the available reports for the selected date.
    /// </summary>
    public void InstantiateExersices()
    {
        RemoveExersicePanels();
        List<Report> reports = exersicesData.GetReports();
        string selectedDay = GetSelectedDate();
        foreach (Report report in reports)
        {
            if (report.date != selectedDay) { continue; }
            if (report.name == "null") { continue; }
            if (report.difficulty == "null")
            {
                report.difficulty = "0";
            }
            if (report.progress == "null")
            {
                report.progress = "0";
            }
            if (report.name != "null")
            {
                GameObject exersicePanel = Instantiate(ExersicePanelPrefab, ExersicePanelParent);
                exersicePanel.GetComponent<ExercisePanel>().SetAll(report.name, report.difficulty, report.progress);
                exersicePanels.Add(exersicePanel.GetComponent<ExercisePanel>());
            }
        }

        if (exersicePanels.Count == 0)
        {
            AddExersiceDay();
            InstantiateExersices();
        }
    }

    /// <summary>
    /// Automatically adds an exercise "Day of exersises" if no exercise panels are present.
    /// </summary>
    private void AddExersiceDay()
    {
        exersicesData.AddReport(new Report(GetSelectedDate(), "Day of exersises", "5", "0", "null", "null"));
    }
}
