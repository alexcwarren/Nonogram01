using UnityEngine;
using System; // for Exception class
using System.IO; // for StreamReader class

using nonogram.nonogram;

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
        const int SWEEP_COUNT_THRESHHOLD = 100;

        bool isSolved = false;
        int sweepCount = 0;

        while (!isSolved && sweepCount < SWEEP_COUNT_THRESHHOLD)
        {
            Debug.Log($"Is solution valid yet? >>> {isValidSolution}");
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
    }

    bool HorizontalSweep()
    {
        for (int col = 0; col < gridSize; col++)
        {
            CheckColumn(col);
        }

        return ValidateSolution();
    }

    bool VerticalSweep()
    {
        for (int row = 0; row < gridSize; row++)
        {
            CheckRow(row);
        }

        return ValidateSolution();
    }

    void CheckRow(int row)
    {
        Debug.Log($"Checking row {row}...");
        return;
    }

    void CheckColumn(int col)
    {
        Debug.Log($"Checking column {col}...");
        return;
    }
} // end class NonogramEditor

} // end namespace nonogram.editor
