using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic; // for List class?

using nonogram.nonogram;

namespace nonogram.cells
{

public abstract class Cell : MonoBehaviour
{
    protected int row;
    protected int col;
    protected GameObject obj;
    protected string type;

    public virtual void Initialize(int row, int col, GameObject obj, Transform parent, string type)
    {
        this.row = row;
        this.col = col;
        this.obj = obj;
        this.type = type;

        this.InitializeObj(parent);

        // Debug.Log($"Creating a {type}Cell at row {row}, column {col}...");
    }

    protected void InitializeObj(Transform parent)
    {
        obj.name = $"{type}_{row},{col}";
        obj.transform.SetParent(parent);

        float width = Nonogram.GetCellWidth();
        obj.transform.localScale = new Vector3(width, width, 0f);
    }

    public virtual void Print()
    {
        Debug.Log($"{type}Cell: row {row}, column {col}, name {obj.name}");
    }
}

public class ToggleCell : Cell
{
    float holdDuration = 0f;
    bool isMouseDown = false;
    bool isOn = false;

    protected Nonogram nonogram;

    static Color _offColor;
    static Color _onColor;

    public Color offColor = Color.white;
    public Color onColor = Color.black;
    public Button thisButton;
    public float holdTime = 0.2f;


    public void Initialize(Nonogram nonogram, int row, int col, GameObject toggleCellObj, Transform parent)
    {
        this.nonogram = nonogram;
        base.Initialize(row, col, toggleCellObj, parent, "Toggle");
    }

    // Use Start(), instead of a constructor method, to access Monobehavior attributes/methods, i.e. transform
    void Start()
    {
        _offColor = this.offColor;
        _onColor = this.onColor;
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
            if (!this.isOn)
            {
                this.TurnOn();
            }
        }
    }

    public void Toggle()
    {
        if (this.isOn)
        {
            this.TurnOff();
        }
        else
        {
            this.TurnOn();
        }

        // Debug.Log($"Toggle_{this.row},{this.col} selected\n");
    }

    public void SetState(bool isOn)
    {
        if (isOn)
        {
            thisButton.GetComponent<Image>().color = onColor;
        }
        else
        {
            thisButton.GetComponent<Image>().color = offColor;
        }

        this.isOn = isOn;

        this.nonogram.UpdateLabels(row, col);
    }

    public bool IsOn()
    {
        return this.isOn;
    }

    public void TurnOn()
    {
        this.SetState(true);
    }

    public void TurnOff()
    {
        this.SetState(false);
    }

    public static Color GetOffColor()
    {
        return _offColor;
    }

    public static Color GetOnColor()
    {
        return _onColor;
    }
} // end class ToggleCell

public class LabelCell : Cell
{
    string hiddenValue = "";
    
    public void Initialize(int row, int col, GameObject labelCellObj, Transform parent)
    {
        
        base.Initialize(row, col, labelCellObj, parent, "Label");
    }

    public void UpdateText(string text)
    {
        this.obj.GetComponentInChildren<Text>().text = text;
    }

    public void UpdateValue(string value)
    {
        this.hiddenValue = value;
        // Debug.Log($"Updating {this.obj.name} hiddenValue to {hiddenValue}...");
    }

    public string GetText()
    {
        return this.obj.GetComponentInChildren<Text>().text;
    }

    public bool TextMatchesValue()
    {
        // Debug.Log($"{this.obj.name}: text={this.GetText()}, value={this.hiddenValue}");
        return this.GetText() == this.hiddenValue;
    }
}


public abstract class Label : List<GameObject>
{
    protected string type;
    public string name;
    protected List<GameObject> toggles;

    public virtual void UpdateLabel(bool updateText = true)
    {
        // Debug.Log($"Updating {this.name}...");
        List<int> counts = CountToggles(toggles);
        
        int ii = 0;

        ClearHiddenValues();

        foreach (int num in counts)
        {
            // Debug.Log($"num={num}");
            string value = num.ToString();
            if (num <= 0)
            {
                value = "";
            }

            // Debug.Log($"{this[ii].name} hiddenValue={this[ii].GetComponent<LabelCell>().hiddenValue}");
            this[ii].GetComponent<LabelCell>().UpdateValue(value);

            if (Nonogram.isEditor && updateText)
            {
                // Debug.Log($"text={this[ii].GetComponent<LabelCell>().GetText()}");
                this[ii].GetComponent<LabelCell>().UpdateText(value);
            }

            // Debug.Log($"counts = {countsTxt}");
            // Debug.Log($"{this[ii].name} hiddenValue={this[ii].GetComponent<LabelCell>().hiddenValue}");
            ii++;
        }
    }

    void ClearHiddenValues()
    {
        foreach (GameObject labelCellObj in this)
        {
            labelCellObj.GetComponent<LabelCell>().UpdateValue("");
        }
    }

    public virtual bool isValid()
    {
        foreach (GameObject labelCellObj in this)
        {
            if (!labelCellObj.GetComponent<LabelCell>().TextMatchesValue())
            {
                return false;
            }
        }

        return true;
    }

    List<int> CountToggles(List<GameObject> toggles)
    {
        bool lastWasOn = false;
        bool isFirstToggle = true;

        List<int> counts = new List<int>();
        counts.Add(0);

        foreach (GameObject toggle in toggles)
        {
            bool toggleIsOn = toggle.GetComponent<ToggleCell>().IsOn();

            if (toggleIsOn)
            {
                if (lastWasOn || isFirstToggle)
                {
                    // Debug.Log($"Last was on, or this is 1st toggle checked for this label");
                    counts[counts.Count - 1]++;
                    isFirstToggle = false;
                }
                else
                {
                    // Debug.Log($"Last was not on");
                    counts.Add(1);
                }
            }

            lastWasOn = toggleIsOn;
        }

        return counts;
    }
}

public class RowLabel : Label
{
    int row;

    public RowLabel(int row) : base()
    {
        this.type = "Row";
        this.row = row;
        this.name = $"{this.type}Label{this.row}";
        this.toggles = Nonogram.toggleCellRows[this.row];
    }
}

public class ColLabel : Label
{
    int col;

    public ColLabel(int col) : base()
    {
        this.type = "Col";
        this.col = col;
        this.name = $"{this.type}Label{this.col}";
        this.toggles = Nonogram.toggleCellCols[this.col];
    }
}

} // end namespace nonogram.cells
