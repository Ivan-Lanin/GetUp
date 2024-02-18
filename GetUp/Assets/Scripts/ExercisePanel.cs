using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExercisePanel : MonoBehaviour
{
    private ReportManager reportManager;
    private ExercisesData exersicesData;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_Text nameHint;
    [SerializeField] private TMP_Text difficultyLabel;
    [SerializeField] private TMP_Dropdown progressDropdown;
    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject donePanel;

    void Start()
    {
        confirmButton.transform.gameObject.SetActive(false);
        reportManager = FindObjectOfType<ReportManager>();
        exersicesData = FindObjectOfType<ExercisesData>();
        nameInputField.onValueChanged.AddListener(delegate { OnNameEdited(); });
        progressDropdown.onValueChanged.AddListener(delegate { OnDropdown(); });
        confirmButton.onClick.AddListener(delegate { OnConfirm(); });
        CheckDonePanel();
    }

    void OnEnable()
    {
        CheckDonePanel();
    }

    public void OnNameEdited()
    {
        confirmButton.transform.gameObject.SetActive(true);

        List<string> exersicesNames = exersicesData.GetExersicesNames();
        string hint = "?";
        string text = nameInputField.text.ToLower();
        foreach (string exersiceName in exersicesNames)
        {
            if (exersiceName.ToLower().Contains(text))
            {
                hint = exersiceName;
                break;
            }
        }
        nameHint.text = hint;
        if (hint != "?")
        {
            SetDifficulty(hint);
        }
        else
        {
            SetDifficulty(0);
        }
    }

    private void SetDifficulty(string name)
    {
        List<Exercise> exersices = exersicesData.GetExersices();
        foreach (Exercise exersice in exersices)
        {
            if (exersice.name == name)
            {
                difficultyLabel.text = exersice.difficulty.ToString();
            }
        }
    }

    private void SetDifficulty(int value)
    {
        difficultyLabel.text = value.ToString();
    }

    private void OnDropdown()
    {
        reportManager.UpdateScore();

        exersicesData.UpdateReports(PanelToReport());

        CheckDonePanel();
    }

    private void CheckDonePanel()
    {
        if (progressDropdown.value > 0)
        {
            donePanel.transform.gameObject.SetActive(true);
        }
        else
        {
            donePanel.transform.gameObject.SetActive(false);
        }
    }

    private void OnConfirm()
    {
        confirmButton.transform.gameObject.SetActive(false);

        nameInputField.text = nameHint.text;

        exersicesData.AddReport(PanelToReport());
        exersicesData.UpdateReports(PanelToReport());
    }

    private Report PanelToReport()
    {
        Report report = new Report();
        report.date = reportManager.GetSelectedDate();
        report.name = nameHint.text.ToString();
        report.difficulty = difficultyLabel.text;
        report.progress = progressDropdown.value.ToString();
        report.dayPoints = "null";
        report.result = "null";
        return report;
    }

    public float GetPoints()
    {
        float points = (float.Parse(difficultyLabel.text) * progressDropdown.value) / 4f;
        return points;
    }

    public void SetAll(string name, string Difficulty, string Progress)
    {
        nameInputField.text = name;
        nameHint.text = name;
        difficultyLabel.text = Difficulty;
        progressDropdown.value = int.Parse(Progress);
    }
}
