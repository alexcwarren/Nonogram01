using UnityEngine;
using UnityEngine.UI; // for Button class
using UnityEngine.SceneManagement;

public class Button_Lvl : MonoBehaviour
{
    public Button button;
    public Text text;
    public int levelBuildIndex;

    void Start()
    {
        if (!button.IsInteractable())
        {
            text.color = Color.white;
        }
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelBuildIndex);
    }
}
