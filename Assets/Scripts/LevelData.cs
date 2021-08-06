using System;
using UnityEngine;

using nonogram.nonogram;
using nonogram.cells;

namespace nonogram.leveldata
{

[Serializable]
public class LevelData
{
    public string level;
    int gridSize;
    public int NonogramSize{get;}
    string[,] cellData;

    public LevelData(int gridSize, GameObject[,] cellObjs)
    {
        this.gridSize = gridSize;
        this.NonogramSize = this.gridSize + Nonogram.GetMaxLabels();
        InitializeCellData(cellObjs);
    }

    public LevelData(string level, int gridSize)
    {
        this.level = level;
        this.gridSize = gridSize;
        this.NonogramSize = this.gridSize + Nonogram.GetMaxLabels();

        this.cellData = new string[this.NonogramSize, this.NonogramSize];
        for (int row = Nonogram.GetMaxLabels(); row < NonogramSize; row++)
        {
            for (int col = Nonogram.GetMaxLabels(); col < NonogramSize; col++)
            {
                this.cellData[row,col] = false.ToString();
            }
        }
    }

    void InitializeCellData(GameObject[,] cellObjs)
    {
        int length0 = cellObjs.GetLength(0);
        int length1 = cellObjs.GetLength(1);
        int MAX_LABELS = Nonogram.GetMaxLabels();

        this.cellData = new string[length0, length1];

        for (int row = 0; row < length0; row++)
        {
            for (int col = 0; col < length0; col++)
            {
                // If cell is an empty cell (the ones in the top-left corner)
                if (row < MAX_LABELS && col < MAX_LABELS)
                {
                    continue;
                }

                // If cell is a ToggleCell
                if (row >= MAX_LABELS && col >= MAX_LABELS)
                {
                    this.cellData[row,col] = cellObjs[row,col].GetComponent<ToggleCell>().IsOn().ToString();
                }

                // Else (i.e. cell is a LabelCell)
                else
                {
                    this.cellData[row,col] = cellObjs[row,col].GetComponent<LabelCell>().GetText();
                }
            }
        }
    }

    public void UpdateCellData(int row, int col, bool state)
    {
        // Prevent invalid row/col values
        if (row < Nonogram.GetMaxLabels() || col < Nonogram.GetMaxLabels())
        {
            Debug.LogError($"Invalid index: row/col must be >= {Nonogram.GetMaxLabels()}: row={row}, col={col}.");
            return;
        }

        this.cellData[row,col] = state.ToString();
    }

    public int GetGridSize()
    {
        return this.gridSize;
    }

    public string[,] GetCellData()
    {
        return this.cellData;
    }

    public void PrintCellData()
    {
        Debug.Log($"levelData:");
        for (int row = Nonogram.GetMaxLabels(); row < cellData.GetLength(0); row++)
        {
            string message = $"{row}: ";
            for (int col = Nonogram.GetMaxLabels(); col < cellData.GetLength(0); col++)
            {
                if (cellData[row,col] == "True")
                {
                    message += "1 ";
                }
                else
                {
                    message += "0 ";
                }
            }
            Debug.Log($"{message}");
        }
    }
} // end LevelData class

} // end namespace nonogram.leveldata
