using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

using nonogram.nonogram;

namespace nonogram.togglecell
{

public class ToggleCell : MonoBehaviour
{
    private int row;
    private int col;
    float duration = 0f;
    bool mouseDown = false;
    public Button thisButton;
    public float holdTime = 0.2f;
    static public Color offColor = Color.white;
    static public Color onColor = Color.black;

    void Start()
    {
        string[] coords = transform.GetComponent<ToggleCell>().name.Remove(0, "Toggle_".Length).Split(',');
        row = Int32.Parse(coords[0]);
        col = Int32.Parse(coords[1]);
    }

    void Update()
    {
        mouseDown = Input.GetMouseButton(0);

        if (mouseDown)
        {
            duration += Time.deltaTime;
        }
        else
        {
            duration = 0f;
        }
    }

    void OnMouseOver()
    {
        if (mouseDown && duration > holdTime)
        {
            // Debug.Log($"Pointing at {this.name}");
            if (this.GetColor() == ToggleCell.GetOffColor())
            {
                this.SetColor(ToggleCell.GetOnColor());
            }
        }
    }

    public void Toggle()
    {
        // Debug.Log($"Toggle_{this.row},{this.col} selected\n");
        ToggleColor();
        Nonogram.UpdateLabels();
    }

    public Color GetColor()
    {
        return thisButton.GetComponent<Image>().color;
    }

    void ToggleColor()
    {
        if (this.GetColor() == offColor)
        {
            this.SetColor(onColor);
        }
        else
        {
            this.SetColor(offColor);
        }
    }

    void SetColor(Color color)
    {
        thisButton.GetComponent<Image>().color = color;
    }

    static public Color GetOffColor()
    {
        return offColor;
    }

    static public Color GetOnColor()
    {
        return onColor;
    }
}

}
