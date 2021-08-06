using UnityEngine;
using UnityEngine.UI; // for Button class
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public LevelButton prevButton;
    public LevelButton nextButton;
    public Button button;
    public Text text;
    public int levelBuildIndex;
    public Color activeFontColor = Color.black;
    public Color inactiveFontColor = Color.white;

    void Start()
    {
        if (!button.IsInteractable())
        {
            text.color = inactiveFontColor;
        }
        else
        {
            text.color = activeFontColor;
        }
    }

    public void Enable()
    {
        text.color = activeFontColor;
        button.interactable = true;
    }

    public void Disable()
    {
        text.color = inactiveFontColor;
        button.interactable = false;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelBuildIndex);
    }
}
