using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensAnimator : MonoBehaviour
{
    [SerializeField] private List<GameObject> screens;

    private int currentScreen;

    private void Start()
    {
        currentScreen = 1;
    }

    public void SwitchScreens(int screenToShow)
    {
        if (screenToShow == currentScreen)
            return;
        screens[currentScreen].SetActive(false);
        screens[screenToShow].SetActive(true);
        currentScreen = screenToShow;
    }
}
