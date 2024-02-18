using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExercisesData : MonoBehaviour
{
    [SerializeField] private TextAsset csvFileExercises;
    [SerializeField] private TextAsset csvFilereports;

    private Exercises exersices;
    private Reports globalReports;
    private string exersicesFile = "exersices.json";
    private string reportsFile = "reportsNew.json";



    void Awake()
    {
        string exersicesFilePath = Application.persistentDataPath + "/" + exersicesFile;
        if (!File.Exists(exersicesFilePath))
        {
            exersices = JsonUtility.FromJson<Exercises>(csvFileExercises.ToString());
            SaveTo(exersicesFile);
        }
        else
        {
            exersices = JsonUtility.FromJson<Exercises>(File.ReadAllText(exersicesFilePath));
        }
        if (exersices == null)
        {
            Debug.LogError("Exersices file is empty");
        }

        string reportsFilePath = Application.persistentDataPath + "/" + reportsFile;
        if (!File.Exists(reportsFilePath))
        {
            globalReports = JsonUtility.FromJson<Reports>(csvFilereports.ToString());
            SaveTo(reportsFile);
        }
        else
        {
            globalReports = JsonUtility.FromJson<Reports>(File.ReadAllText(reportsFilePath));
        }
        if (globalReports == null)
        {
            Debug.LogError("Reports file is empty");
        }
    }

    public List<string> GetExersicesNames()
    {
        return exersices.exersices.Select(exersice => exersice.name).ToList();
    }

    public List<Exercise> GetExersices()
    {
        return exersices.exersices;
    }

    public List<Report> GetReports()
    {
        return globalReports.reports;
    }

    private void FillEmptyDaysStats()
    {
        List<Report> reports = GetReports();
        List<string> days = new List<string>();

        // get all days dates from first report to today
        string firstReportDate = reports[0].date;
        string todaysDate = DateTime.Now.ToString("dd.MM.yy");
        DateTime firstDate = DateTime.ParseExact(firstReportDate, "dd.MM.yy", null);
        DateTime todaysDateDate = DateTime.ParseExact(todaysDate, "dd.MM.yy", null);
        for (DateTime date = firstDate; date <= todaysDateDate; date = date.AddDays(1))
        {
            days.Add(date.ToString("dd.MM.yy"));
        }

        foreach (Report report in reports)
        {
            if (days.Contains(report.date))
            {
                days.Remove(report.date);
            }
        }

        foreach (string day in days)
        {
            Report dayReport = new Report();
            dayReport.date = day;
            dayReport.name = "null";
            dayReport.difficulty = "null";
            dayReport.progress = "null";
            dayReport.dayPoints = "0";
            dayReport.result = "null";
            reports.Add(dayReport);
        }

        // sort reports by date
        globalReports.reports = reports.OrderBy(report => DateTime.ParseExact(report.date, "dd.MM.yy", null)).ToList();

        SaveTo(reportsFile);
    }

    public List<Report> GetDayStatsReports()
    {
        FillEmptyDaysStats();
        List<Report> dayStats = new List<Report>();
        foreach (Report report in globalReports.reports)
        {
            if (report.name == "null")
            {
                dayStats.Add(report);
            }
        }
        return dayStats;
    }

    /// <summary>
    /// Get all days scores before selected day
    /// </summary>
    public List<float> GetSelectedDaysScore(string selectedDay)
    {
        List<Report> dayStatsReports = GetDayStatsReports();

        List<Report> filteredReports = dayStatsReports.TakeWhile(report => report.date != selectedDay).ToList();
        filteredReports.Add(dayStatsReports.Find(report => report.date == selectedDay));
        List<float> values = new List<float>();
        foreach (Report report in filteredReports)
        {
            if (report.dayPoints.Contains(','))
            {
                report.dayPoints = report.dayPoints.Replace(',', '.');
            }
            values.Add(float.Parse(report.dayPoints, CultureInfo.InvariantCulture));
        }
        return values;
    }

    public float GetAverageScore(string selectedDay)
    {
        List<float> filteredReports = GetSelectedDaysScore(selectedDay);
        if (filteredReports.Count == 0)
        {
            return 0;
        }
        float averageScore = filteredReports.Average();
        averageScore = (float) Math.Round(averageScore, 2);
        return averageScore;
    }

    public int GetDaysCount()
    {
        return GetDayStatsReports().Count;
    }

    public void AddReport(Report report)
    {
        if (globalReports.reports.Find(r => r.date == report.date && r.name == "null") == null)
        {
            Report dayReport = new Report();
            dayReport.date = report.date;
            dayReport.name = "null";
            dayReport.difficulty = "null";
            dayReport.progress = "null";
            dayReport.dayPoints = "null";
            dayReport.result = "null";
            globalReports.reports.Add(dayReport);
        }
        //add report to reports if it is not there
        if (globalReports.reports.Find(r => r.date == report.date && r.name == report.name) == null)
        {
            globalReports.reports.Add(report);
        }
        
        SaveTo(reportsFile);
    }

    public void UpdateReports(Report report)
    {
        // update report if it is there
        if (globalReports.reports.Find(r => r.date == report.date && r.name == report.name) != null)
        {
            Report reportToUpdate = globalReports.reports.Find(r => r.date == report.date && r.name == report.name);
            reportToUpdate.difficulty = report.difficulty;
            reportToUpdate.progress = report.progress;
        }
        SaveTo(reportsFile);
    }

    public void UpdateReports(string date, string dayPoints)
    {
        if (globalReports.reports.Find(r => r.date == date && r.name == "null") != null)
        {
            Report reportToUpdate = globalReports.reports.Find(r => r.date == date && r.name == "null");
            reportToUpdate.dayPoints = dayPoints;
        }
        SaveTo(reportsFile);
    }

    private void SaveTo(string filename)
    {

        string filepath = Application.persistentDataPath + "/" + filename;
        if (!File.Exists(filepath))
        {
            print("This file doesn't exists");

            switch (filename)
            {
                case "exersices.json":
                    File.WriteAllText(filepath, JsonUtility.ToJson(exersices, true));
                    return;
                case "reportsNew.json":
                    File.WriteAllText(filepath, JsonUtility.ToJson(globalReports, true));
                    return;
            }
        }
        if (filename == exersicesFile)
        {
            File.WriteAllText(filepath, JsonUtility.ToJson(exersices, true));
        }
        if (filename == reportsFile)
        {
            File.WriteAllText(filepath, JsonUtility.ToJson(globalReports, true));
        }
    }
}

[Serializable]
public class Exercise
{
    public string name;
    public int difficulty;
    public int usefullness;
}

[Serializable]
public class Exercises
{
    public List<Exercise> exersices;
}

[Serializable]
public class Report
{
    public string date;
    public string name;
    public string difficulty;
    public string progress;
    public string dayPoints;
    public string result;

    /// <summary>
    /// Initializes a new instance of the <see cref="Report"/> class.
    /// </summary>
    /// <param name="date">The date of the report.</param>
    /// <param name="name">The name of the exercise.</param>
    /// <param name="difficulty">The difficulty of the exercise.</param>
    /// <param name="progress">The progress of the exercise.</param>
    /// <param name="dayPoints">The points earned for the day.</param>
    /// <param name="result">The result of the exercise.</param>
    public Report(string date = null, string name = null, string difficulty = null, string progress = null, string dayPoints = null, string result = null)
    {
        this.date = date;
        this.name = name;
        this.difficulty = difficulty;
        this.progress = progress;
        this.dayPoints = dayPoints;
        this.result = result;
    }
}

[Serializable]
public class Reports
{
    public List<Report> reports;
}
