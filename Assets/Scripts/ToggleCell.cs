using UnityEngine;
using UnityEngine.UI;
using System;

using nonogram.nonogram;

namespace nonogram.togglecell
{

public class ToggleCell : MonoBehaviour
{
    private int row;
    private int col;
    float holdDuration = 0f;
    bool isMouseDown = false;
    public Button thisButton;
    public float holdTime = 0.2f;
    static public Color offColor = Color.white;
    static public Color onColor = Color.black;

    void Start()
    {
        // Get row and column of current ToggleCell from its Compononent name
        string[] coords = transform.GetComponent<ToggleCell>().name.Remove(0, "Toggle_".Length).Split(',');
        row = Int32.Parse(coords[0]);
        col = Int32.Parse(coords[1]);
    }

    void Update()
    {
        isMouseDown = Input.GetMouseButton(0);

        if (isMouseDown)
        {
            holdDuration += Time.deltaTime;
        }
        else
        {
            holdDuration = 0f;
        }
    }

    void OnMouseOver()
    {
        if (isMouseDown && holdDuration > holdTime)
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
