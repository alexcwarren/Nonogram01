using System;
using System.IO;
using UnityEngine;

using nonogram.userdata;

namespace nonogram.mainmenu
{

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UserData.Username = "default";

        UserData.Initialize();
        LoadUserData();
    }

    void LoadUserData()
    {
        // check for user's level progress
        if (!UserData.LoadLevelProgress())
        {
            UserData.SaveLevelProgress();
        }

        // unlock any level marked COMPLETED for them in addition to the first highest level above the last completed
        foreach (LevelButton button in transform.GetComponentsInChildren<LevelButton>())
        {
            // Debug.Log($"{button.name}: levelNum={button.text.text}");
            if (UserData.IsLevelComplete(button.text.text))
            {
                button.Enable();
                button.nextButton.Enable();
            }
            else if (button.text.text != "01" && !UserData.IsLevelComplete(button.prevButton.text.text))
            {
                button.Disable();
            }
        }
    }

    public void ResetProgress()
    {
        UserData.ResetProgress();

        foreach (LevelButton button in transform.GetComponentsInChildren<LevelButton>())
        {
            if (button.text.text == "01")
            {
                button.Enable();
            }
            else
            {
                button.Disable();
            }
        }
        
        LoadUserData();
    }
} // end MainMenu class

} // end namespace nonogram.mainmenu
