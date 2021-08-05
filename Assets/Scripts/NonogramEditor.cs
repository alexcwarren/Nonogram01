using UnityEngine;
using System; // for Exception class
using System.IO; // for StreamReader class

using nonogram.nonogram;
using nonogram.cells;

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
            sw.Write(TraverseLabelCells());
            sw.Write(TraverseToggleCells());

            sw.Close();
        }
        catch(Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
            return;
        }
    }

    public void CalculateDifficulty()
    {
        // const int SWEEP_COUNT_THRESHHOLD = 100;
        const int SWEEP_COUNT_THRESHHOLD = 1;

        bool isSolved = false;
        int sweepCount = 0;

        // ClearToggleCells();

        while (!isSolved && sweepCount < SWEEP_COUNT_THRESHHOLD)
        {
            sweepCount++;
            Debug.Log($"Sweep {sweepCount}: Attempting to solve...");

            if (HorizontalSweep() || VerticalSweep())
            {
                Debug.Log($"Solved!");
                isSolved = true;
                break;
            }
        }
        
        // LoadTogglesData();

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
    }

    bool HorizontalSweep()
    {
        Debug.Log($"Starting HorizontalSweep...");
        for (int col = maxLabels; col < nonogramSize; col++)
        {
            CheckColumn(col);
        }

        return ValidateSolution();
    }

    bool VerticalSweep()
    {
        Debug.Log($"Starting VerticalSweep...");
        for (int row = maxLabels; row < nonogramSize; row++)
        {
            CheckRow(row);
        }

        return ValidateSolution();
    }

    void CheckLabel(Label label)
    {
        Debug.Log($"Checking {label.name}...");

        string labelText = "";
        foreach (GameObject labelObj in label)
        {
            labelText += $"{labelObj.GetComponent<LabelCell>().GetText()} ";
        }
        Debug.Log($"{labelText}");
    }

    void CheckRow(int row)
    {
        CheckLabel(rowLabels[row]);
    }

    void CheckColumn(int col)
    {
        CheckLabel(colLabels[col]);
    }
} // end class NonogramEditor

} // end namespace nonogram.editor
