using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Navigation : MonoBehaviour
{

    [SerializeField] private ScreensAnimator screensAnimator;

    public void OnExersicesNavigationButtonClicked()
    {
        screensAnimator.SwitchScreens(0);
    }

    public void OnReportNavigationButtonClicked()
    {
        screensAnimator.SwitchScreens(1);
    }

    public void OnGraphNavigationButtonClicked()
    {
        screensAnimator.SwitchScreens(2);
    }

    public void OnDiaryNavigationButtonClicked()
    {
        screensAnimator.SwitchScreens(3);
    }
}
