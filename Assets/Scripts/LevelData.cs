using System;
using UnityEngine;

using nonogram.nonogram;
using nonogram.cells;

namespace nonogram.leveldata
{

[Serializable]
public class LevelData
{
    int gridSize;
    string[,] cellData;

    public LevelData(int gridSize, GameObject[,] cellObjs)
    {
        this.gridSize = gridSize;
        InitializeCellData(cellObjs);
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

    public int GetGridSize()
    {
        return this.gridSize;
    }

    public string[,] GetCellData()
    {
        return this.cellData;
    }
}

} // end namespace nonogram.leveldata
