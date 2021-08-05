using UnityEngine;
using System; // for Exception class
using System.Collections.Generic; // for Dictionary class
using System.IO; // for FileStrem class
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI; // for Text class
using UnityEngine.SceneManagement;

using nonogram.cells;
using nonogram.leveldata;

namespace nonogram.nonogram
{

public class Nonogram : MonoBehaviour
{
    bool fileIsRead = false;
    string[,] cellData;
    Vector3 Origin;
    float backgroundWidth;
    string filepath = "Assets/DataFiles/";

    static float cellWidth;
    
    protected bool isValidSolution = false;

    protected static int maxLabels;
    protected static int nonogramSize;
    protected static GameObject[,] cellObjs;
    
    public string level;
    public string filename = ".dat";
    public Text levelTitle;
    public int gridSize = 4;
    public float spaceWidth = 0.05f;
    public GameObject toggleCellPrefab;
    public GameObject labelCellPrefab;
    public GameObject CorrectButton;
    public GameObject IncorrectButton;

    public const int MAX_GRID_SIZE = 10;
    public static Dictionary<int, Label> rowLabels;
    public static Dictionary<int, Label> colLabels;
    public static Dictionary<int, List<GameObject>> toggleCellRows;
    public static Dictionary<int, List<GameObject>> toggleCellCols;
    public static bool isEditor = false;

    void Start()
    {
        HideEndStatusButtons();

        levelTitle.text = $"LEVEL {level}";
        // Debug.Log($"Loading {levelTitle.text}...");

        Origin = transform.position;

        backgroundWidth = transform.localScale.x;

        UpdateDimensions(gridSize);

        if (filename != "")
        {
            ReadDataFile();
        }

        InstantiateCells();

        if (isEditor && fileIsRead)
        {
            LoadTogglesData();
        }
    }

    protected void StartFromInheritor()
    {
        this.Start();
    }

    void UpdateDimensions(int size)
    {
        gridSize = size;

        maxLabels = gridSize/2;
        if (gridSize % 2 == 1)
        {
            maxLabels++;
        }

        nonogramSize = gridSize + maxLabels;

        cellWidth = (backgroundWidth - spaceWidth*(nonogramSize + 1))/nonogramSize;
    }

    protected void ReadDataFile()
    {
        try
        {
            string path = GetFilepath();

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path} not found.");
            }

            Debug.Log($"Reading data from {path}...");

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            LevelData levelData = formatter.Deserialize(stream) as LevelData;

            UpdateDimensions(levelData.GetGridSize());

            cellData = levelData.GetCellData();

            stream.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
            return;
        }

        fileIsRead = true;

        // for (int row = 0; row < cellData.GetLength(0); row++)
        // {
        //     for (int col = 0; col < cellData.GetLength(1); col++)
        //     {
        //         Debug.Log($"cellData[{row},{col}] = >>>{cellData[row,col]}<<<");
        //     }
        // }
    } // end ReadDataFile

    protected void LoadTogglesData()
    {
        TraverseToggleCells(load: true);
    }

    public void LoadMainMenu()
    {
        // Load scene with Build Index = 0 (i.e. the Main Menu)
        SceneManager.LoadScene(0);
    }

    void InstantiateCells()
    {
        cellObjs = new GameObject[nonogramSize, nonogramSize];
        
        InstantiateToggleCells();
        InstantiateLabels();
    }

    void InstantiateToggleCells()
    {
        toggleCellRows = new Dictionary<int, List<GameObject>>();
        toggleCellCols = new Dictionary<int, List<GameObject>>();

        TraverseToggleCells(instantiate: true);
    }

    protected void TraverseToggleCells
    (
        bool instantiate = false,
        bool load = false,
        bool clear = false
    )
    {
        // Prevent more than one bool argument being true
        if
        (
            (instantiate && load) || (instantiate && clear) || (load && clear)
        )
        {
            Debug.LogError($"Only one argument can be set to 'true': instantiate={instantiate}, load={load}, clear={clear}.");
            return;
        }

        string function = "Traversing";
        if (instantiate)
        {
            function = "Instantiating";
        }
        else if (load)
        {
            function = "Loading";
        }
        else if (clear)
        {
            function = "Clearing";
        }
        Debug.Log($"{function} ToggleCells...");

        for (int row = 0; row < cellObjs.GetLength(0); row++)
        {
            for (int col = 0; col < cellObjs.GetLength(1); col++)
            {
                // If at ToggleCell location
                if (row >= maxLabels && col >= maxLabels)
                {
                    if (instantiate)
                    {
                        GameObject currToggle = Instantiate(toggleCellPrefab, GetVector3(row, col), transform.rotation);
                        cellObjs[row,col] = currToggle;
                        
                        // ToggleCell is already a component via ToggleCell prefab settings in Unity
                        cellObjs[row,col].GetComponent<ToggleCell>().Initialize
                        (
                            this, row, col, currToggle, transform.GetComponentInChildren<Canvas>().transform
                        );

                        // If cell not already in row/colToggleCells, add new List to each Dictionary
                        if (!toggleCellRows.ContainsKey(row))
                        {
                            toggleCellRows.Add(row, new List<GameObject>());
                        }

                        if (!toggleCellCols.ContainsKey(col))
                        {
                            toggleCellCols.Add(col, new List<GameObject>());
                        }

                        // Add ToggleCell objects to List in row/col Dictionaries
                        toggleCellRows[row].Add(cellObjs[row,col]);
                        toggleCellCols[col].Add(cellObjs[row,col]);
                    }

                    else if (load)
                    {
                        // Debug.Log($"Loading {cellObjs[row,col].name}...");
                        string isOn = cellData[row,col];
                        cellObjs[row,col].GetComponent<ToggleCell>().SetState(isOn == "True");
                    }

                    else if (clear)
                    {
                        cellObjs[row,col].GetComponent<ToggleCell>().TurnOff();
                    }

                    // cellObjs[row,col].GetComponent<ToggleCell>().Print();
                } // end if at ToggleCell location
            } // end for col
        } // end for row
    } // end TraverseToggleCells

    public void ClearToggleCells()
    {
        TraverseToggleCells(clear: true);
    }

    void InstantiateLabels()
    {
        TraverseLabelCells(instantiate: true);
    }

    protected void TraverseLabelCells
    (
        bool instantiate = false
    )
    {
        string function = "Traversing";
        if (instantiate)
        {
            function = "Instantiating";
        }
        Debug.Log($"{function} LabelCells...");

        rowLabels = new Dictionary<int, Label>();
        colLabels = new Dictionary<int, Label>();

        for (int row = 0; row < cellObjs.GetLength(0); row++)
        {
            for (int col = 0; col < cellObjs.GetLength(1); col++)
            {
                bool isRowLabelCell = false;
                bool isColLabelCell = false;

                // If at LabelCell location
                if (row < maxLabels && col >= maxLabels)
                {
                    isColLabelCell = true;
                }
                else if (col < maxLabels && row >= maxLabels)
                {
                    isRowLabelCell = true;
                }
                else
                {
                    continue;
                }

                if (instantiate)
                {
                    GameObject currLabelCell = Instantiate(labelCellPrefab, GetVector3(row, col), transform.rotation);
                    
                    currLabelCell.AddComponent<LabelCell>();
                    currLabelCell.GetComponent<LabelCell>().Initialize
                    (
                        row, col, currLabelCell, transform.GetComponentInChildren<Canvas>().transform
                    );

                    if (fileIsRead)
                    {
                        string text = cellData[row,col];
                        currLabelCell.GetComponent<LabelCell>().UpdateText(text);
                    }

                    cellObjs[row,col] = currLabelCell;
                    // currLabelCell.GetComponent<LabelCell>().Print();

                    if (isRowLabelCell)
                    {
                        // If a Label for current row does not exist
                        if (!rowLabels.ContainsKey(row))
                        {
                            // Add new Label to rowLabels
                            rowLabels.Add(row, new RowLabel(row));
                        }

                        // Add currLabelCell to Label at row in rowLabels
                        rowLabels[row].Add(currLabelCell);
                    }

                    if (isColLabelCell)
                    {
                        // If a Label for current col does not exist
                        if (!colLabels.ContainsKey(col))
                        {
                            // Add new Label to colLabels
                            colLabels.Add(col, new ColLabel(col));
                        }

                        // Add currLabelCell to Label at col in colLabels
                        colLabels[col].Add(currLabelCell);
                    }
                } // end if instantiate
            } // end for col
        } // end for row
    } // end TraverseLabelCells

    public void UpdateLabels(int toggleRow, int toggleCol)
    {
        if (rowLabels.ContainsKey(toggleRow))
        {
            rowLabels[toggleRow].UpdateLabel();
        }

        if (colLabels.ContainsKey(toggleCol))
        {
            colLabels[toggleCol].UpdateLabel();
        }
    }

    bool CompareLabels()
    {
        return CheckLabels(rowLabels) && CheckLabels(colLabels);
    }

    bool CheckLabels(Dictionary<int, Label> labelsDict)
    {
        foreach (int key in labelsDict.Keys)
        {
            if (!labelsDict[key].isValid())
            {
                return false;
            }
        }

        return true;
    }

    public void SubmitSolution()
    {
        isValidSolution = ValidateSolution();

        if (!isValidSolution)
        {
            Debug.Log($"INCORRECT");

            IncorrectButton.transform.SetAsLastSibling();
            IncorrectButton.SetActive(true);
        }
        else
        {
            Debug.Log($"Correct!");

            CorrectButton.transform.SetAsLastSibling();
            CorrectButton.SetActive(true);
        }
    }

    protected bool ValidateSolution()
    {
        return CompareLabels();
    }

    void SetEndStatusButtons(bool status)
    {
        CorrectButton.SetActive(status);
        IncorrectButton.SetActive(status);
    }

    public void HideEndStatusButtons()
    {
        if (isEditor)
        {
            return;
        }

        SetEndStatusButtons(false);
    }

    Vector3 GetVector3(int row, int col)
    {
        float x = Origin.x - backgroundWidth/2f + cellWidth*(col + 0.5f) + spaceWidth*(col + 1f);
        float y = Origin.y + backgroundWidth/2f - cellWidth*(row + 0.5f) - spaceWidth*(row + 1f);
        // Debug.Log($"x={x}, y={y}");

        return new Vector3(x, y, Origin.z);
    }

    protected string GetFilepath()
    {
        return $"{filepath}{filename}";
    }

    public static int GetMaxLabels()
    {
        return maxLabels;
    }

    public static float GetCellWidth()
    {
        return cellWidth;
    }
} // end class Nonogram

} // end namespace nonogram.nonogram
