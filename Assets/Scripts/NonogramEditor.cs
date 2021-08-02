using UnityEngine;
using System; // for Exception class
using System.Collections.Generic; // for Dictionary class
using System.IO; // for StreamReader class
using UnityEngine.UI; // for Text class

using nonogram.nonogram;
using nonogram.togglecell;

namespace nonogram.editor
{

public class NonogramEditor : Nonogram
{
    void Start()
    {
        isEditor = true;

        StartFromInheritor();
    }

    public void WriteDataFile()
    {
        if (filename == "")
        {
            Debug.LogError($"ERROR: Cannot save. Filename empty.");
            return;
        }

        try
        {
            StreamWriter sw = new StreamWriter($"Assets/DataFiles/{filename}");
            Debug.Log($"Saving {filename}...");

            sw.WriteLine($"{gridSize}");

            for (int row = 0; row < nonogramSize; row++)
            {
                for (int col = 0; col < nonogramSize; col++)
                {
                    string key = $"{row},{col}";
                    GameObject cell = cells[row,col];

                    if (row < maxLabels && col < maxLabels)
                    {
                        continue;
                    }
                    // If ToggleCell
                    else if (row >= maxLabels && col >= maxLabels)
                    {
                        bool isOn = false;

                        // try
                        // {
                        //     isOn = cell.GetComponent<Image>().color == ToggleCell.GetOnColor();
                        // }
                        // catch(Exception e)
                        // {
                        //     Debug.LogError($"ERROR: at {row},{col}: {e.Message}");
                        // }
                        
                        sw.WriteLine($"{key}:{isOn}");
                    }
                    // Else (i.e. LabelCell)
                    else
                    {
                        string txt = "";

                        try
                        {
                            txt = cell.GetComponentInChildren<Text>().text;
                        }
                        catch(Exception e)
                        {
                            Debug.LogError($"ERROR: at {row},{col}: {e.Message}");
                        }

                        if (txt == "")
                        {
                            txt = "0";
                        }
                        sw.WriteLine($"{key}:{txt}");
                    }
                }
            }

            sw.Close();
        }
        catch(Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
            return;
        }
    }

    public string CalculateDifficulty()
    {
        const int SWEEP_COUNT_THRESHHOLD = 100;

        bool isSolved = false;
        int sweepCount = 0;

        while (!isSolved && sweepCount < SWEEP_COUNT_THRESHHOLD)
        {
            sweepCount++;

            if (HorizontalSweep() || VerticalSweep())
            {
                isSolved = true;
                break;
            }
        }

        string result = "";

        if (isSolved)
        {
            result = sweepCount.ToString();
        }
        else
        {
            result = "?";
        }

        Debug.Log($"\n\nDIFFICULTY = {result}\n\n");

        return result;
    }

    bool HorizontalSweep()
    {
        return false;
    }

    void CheckColumn()
    {
        return;
    }

    bool VerticalSweep()
    {
        return false;
    }

    void CheckRow()
    {
        return;
    }
}

}
