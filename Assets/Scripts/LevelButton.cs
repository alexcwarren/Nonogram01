using UnityEngine;
using UnityEngine.UI; // for Button class
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
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

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelBuildIndex);
    }
}
