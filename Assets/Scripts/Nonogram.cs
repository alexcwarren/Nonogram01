using UnityEngine;
using System; // for Exception class
using System.Collections.Generic; // for Dictionary class
using System.IO; // for StreamReader class
using UnityEngine.UI; // for Text class
using UnityEngine.SceneManagement;

using nonogram.togglecell;

namespace nonogram.nonogram
{

public class Nonogram : MonoBehaviour
{
    protected Vector3 Origin;
    protected float backgroundWidth;
    protected float cellWidth;
    protected static int maxLabels;
    protected static int nonogramSize;
    protected string[,] cellStrings;
    protected static GameObject[,] cells;
    static string[,] updatedCells; // May need to be "protected"
    protected static Dictionary<string, GameObject> labels = new Dictionary<string, GameObject>();
    protected static Dictionary<string, GameObject> toggles = new Dictionary<string, GameObject>();
    protected bool fileIsRead = false;
    protected static bool isEditor = false;
    protected bool mouseDown = false;

    public const int MAX_GRID_SIZE = 10;
    public string level;
    public string filename = ".txt";
    public Text levelTitle;
    public int gridSize = 4;
    public float spaceWidth = 0.05f;
    public GameObject toggleCellPrefab;
    public GameObject labelCellPrefab;
    public GameObject CorrectButton;
    public GameObject IncorrectButton;

    void Start()
    {
        HideEndStatusButtons();

        levelTitle.text = $"LEVEL {level}";
        // Debug.Log($"Loading {levelTitle.text}...");

        Origin = transform.position;

        backgroundWidth = transform.localScale.x;

        if (filename != "")
        {
            ReadDataFile();
        }
        else
        {
            UpdateDimensions(gridSize);
        }

        cells = new GameObject[nonogramSize,nonogramSize];

        cellWidth = (backgroundWidth - spaceWidth*(nonogramSize + 1))/nonogramSize;

        updatedCells = new string[nonogramSize,nonogramSize];

        InstantiateCells();
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
    }

    protected void StartFromInheritor()
    {
        this.Start();
    }

    protected void ReadDataFile()
    {
        string line;
        try
        {
            StreamReader sr = new StreamReader($"Assets/DataFiles/{filename}");
            // Debug.Log($"Reading {filename}...");

            line = sr.ReadLine();
            int num = 0;

            try
            {
                num = Int32.Parse(line);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception: {e.Message}");
                return;
            }

            if (num <= 2 || num > MAX_GRID_SIZE)
            {
                Debug.LogError($"ERROR: Incorrect grid size: {num}.");
            }

            UpdateDimensions(num);

            cellStrings = new string[nonogramSize,nonogramSize];

            line = sr.ReadLine();

            while (line != null)
            {
                string[] tokens = line.Split(':');
                if (tokens.Length != 2)
                {
                    Debug.LogError($"ERROR: Incorrect number of tokens: {tokens.Length} ({line}).");
                    return;
                }

                int row = ParseCoordinates(tokens[0])[0];
                int col = ParseCoordinates(tokens[0])[1];

                if (tokens[1] == "0")
                {
                    tokens[1] = "";
                }
                cellStrings[row,col] = tokens[1];

                line = sr.ReadLine();
            }

            sr.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
            return;
        }

        fileIsRead = true;
    }
    
    protected void InstantiateCells()
    {
        for (int row = 0; row < gridSize + maxLabels; row++)
        {
            for (int col = 0; col < gridSize + maxLabels; col++)
            {
                float x = Origin.x - backgroundWidth/2f + cellWidth*(col + 0.5f) + spaceWidth*(col + 1f);
                float y = Origin.y + backgroundWidth/2f - cellWidth*(row + 0.5f) - spaceWidth*(row + 1f);
                // Debug.Log($"x={x}, y={y}");

                Vector3 currPos = new Vector3(x, y, Origin.z);

                // If at LabelCell location
                if (
                     (row < maxLabels && col >= maxLabels)
                     || (col < maxLabels && row >= maxLabels)
                   )
                {
                    GameObject currLabelCell = Instantiate(labelCellPrefab, currPos, transform.rotation);
                    currLabelCell.name = $"Label_{row},{col}";
                    currLabelCell.transform.SetParent(transform.GetComponentInChildren<Canvas>().transform);
                    currLabelCell.transform.localScale = new Vector3(cellWidth, cellWidth, 0f);

                    if (fileIsRead)
                    {
                        string text = cellStrings[row,col];
                        currLabelCell.GetComponentInChildren<Text>().text = text;
                    }

                    cells[row,col] = currLabelCell;
                }
                // Else If at ToggleCell location
                else if (row >= maxLabels || col >= maxLabels)
                {
                    GameObject currToggleCell = Instantiate(toggleCellPrefab, currPos, transform.rotation);
                    currToggleCell.name = $"Toggle_{row},{col}";
                    currToggleCell.transform.SetParent(transform.GetComponentInChildren<Canvas>().transform);
                    currToggleCell.transform.localScale = new Vector3(cellWidth, cellWidth, 0f);

                    if (fileIsRead && isEditor)
                    {
                        string isOn = cellStrings[row,col];

                        if (isOn == "True")
                        {
                            currToggleCell.GetComponentInChildren<Image>().color = ToggleCell.GetOnColor();
                        }
                        else
                        {
                            currToggleCell.GetComponentInChildren<Image>().color = ToggleCell.GetOffColor();
                        }
                    }

                    cells[row,col] = currToggleCell;
                }
            } // end for col
        } // end for row
    }

    public void LoadMainMenu()
    {
        // Load scene with Build Index = 0 (i.e. the Main Menu)
        SceneManager.LoadScene(0);
    }

    public void ClearToggleCells()
    {
        for (int row = maxLabels; row < nonogramSize; row++)
        {
            for (int col = maxLabels; col < nonogramSize; col++)
            {
                cells[row,col].GetComponent<Image>().color = ToggleCell.GetOffColor();
            } // end for col
        } // end for row
    }

    public void SubmitSolution(bool beQuiet = false)
    {
        UpdateLabels();

        bool isValidSolution = true;

        for (int row = 0; row < updatedCells.GetLength(0); row++)
        {
            for (int col = 0; col < updatedCells.GetLength(1); col++)
            {
                // Continue if not a LabelCell
                if (
                     (row < maxLabels && col < maxLabels) // Empty cell
                     || (row >= maxLabels && col >= maxLabels) // ToggleCell
                   )
                {
                    continue;
                }

                string updated = updatedCells[row,col];
                string cellTxt = cells[row,col].GetComponentInChildren<Text>().text;

                // Debug.Log($"{row},{col}: updated={updated}, cellTxt={cellTxt}");

                if (updated != cellTxt)
                {
                    if (!beQuiet)
                    {
                        Debug.Log($"INCORRECT");

                        IncorrectButton.transform.SetAsLastSibling();
                        IncorrectButton.SetActive(true);
                    }

                    isValidSolution = false;
                    break;
                }
            }

            if (!isValidSolution)
            {
                break;
            }
        }

        if (isValidSolution && !beQuiet)
        {
            Debug.Log($"Correct!");
            CorrectButton.transform.SetAsLastSibling();
            CorrectButton.SetActive(true);
        }

        // return isValidSolution;
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

    public static void UpdateLabels()
    {
        for (int i = 0; i < updatedCells.GetLength(0); i++)
        {
            for (int j = 0; j < updatedCells.GetLength(1); j++)
            {
                updatedCells[i,j] = "";
            }
        }
        
        HashSet<int> visitedRowLabels = new HashSet<int>();
        HashSet<int> visitedColLabels = new HashSet<int>();
        
        // Debug.Log("Traversing all labels...");
        for (int lblRow = 0; lblRow < cells.GetLength(0); lblRow++)
        {
            for (int lblCol = 0; lblCol < cells.GetLength(1); lblCol++)
            {
                if (
                        (lblRow >= maxLabels && lblCol >= maxLabels) // If at ToggleCell coordinates
                        || (visitedRowLabels.Contains(lblRow) || visitedColLabels.Contains(lblCol)) // If already visisted Row/Column LabelCell
                    )
                {
                    continue;
                }

                if (
                        (lblRow < maxLabels && lblCol >= maxLabels) // Column LabelCell
                        || (lblCol < maxLabels && lblRow >= maxLabels) // Row LabelCell
                    )
                {
                    GameObject lblObj = cells[lblRow,lblCol];
                    // Debug.Log($"At Label {lblRow},{lblCol}");

                    string lblTxt = "";
                    try
                    {
                        lblTxt = lblObj.GetComponentInChildren<Text>().text;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"ERROR: at {lblRow},{lblCol}: {e.Message}");
                    }
                    // Debug.Log($"Label currently is: >>>{lblTxt}<<<");
                    
                    List<int> counts = new List<int>();
                    counts.Add(0);
                    
                    bool isRowLabel = false;
                    bool isColLabel = false;
                    int tglRow = lblRow;
                    int tglCol = lblCol;
                    int rowOffset = 0;
                    int colOffset = 0;
                    bool lastWasOn = false;
                    bool isFirstToggle = true;

                    // If row LabelCell
                    if (lblRow >= maxLabels)
                    {
                        isRowLabel = true;
                        visitedRowLabels.Add(lblRow);
                        tglCol = maxLabels;
                        colOffset = 1;
                    }
                    // Else (i.e. is column LabelCell)
                    else
                    {
                        isColLabel = true;
                        visitedColLabels.Add(lblCol);
                        tglRow = maxLabels;
                        rowOffset = 1;
                    }
                    
                    // Debug.Log($"Checking its {nonogramSize - maxLabels} Toggles...");
                    for (int i = 0; i < (nonogramSize - maxLabels); i++)
                    {
                        // Debug.Log($"At Toggle {tglRow},{tglCol}");
                        GameObject tglObj = cells[tglRow,tglCol];

                        bool isToggleOn = false;
                        try
                        {
                            isToggleOn = tglObj.GetComponent<Image>().color == ToggleCell.GetOnColor();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"ERROR: at {tglRow},{tglCol}: {e.Message}");
                        }

                        // Debug.Log($"Toggle status: {isToggleOn}");

                        // Increment if True
                        if (isToggleOn)
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

                        // Debug.Log($"Now visited {i+1} Toggles");

                        tglRow += rowOffset;
                        tglCol += colOffset;
                        lastWasOn = isToggleOn;
                    } // end for LabelCell's ToggleCells

                    // Reset Row/Column LabelCell (current and adjacent, i.e. same Row/Col)
                    for (int i = 0; i < maxLabels; i++)
                    {
                        if (isRowLabel)
                        {
                            if (isEditor)
                            {
                                cells[lblRow,lblCol + i].GetComponentInChildren<Text>().text = "";
                            }
                            // else
                            // {}
                        }
                        else if (isColLabel)
                        {
                            if (isEditor)
                            {
                                cells[lblRow + i,lblCol].GetComponentInChildren<Text>().text = "";
                            }
                            // else
                            // {}
                        }
                        else
                        {
                            Debug.LogError($"ERROR!");
                            return;
                        }
                    }

                    for (int i = 0; i < counts.Count; i++)
                    {
                        // Debug.Log($"counts[{i}] = {counts[i]}");
                        string countTxt = counts[i].ToString();
                        if (counts[i] == 0)
                        {
                            continue;
                        }

                        if (isRowLabel)
                        {
                            if (isEditor)
                            {
                                cells[lblRow,lblCol + i].GetComponentInChildren<Text>().text = countTxt;
                            }
                            else
                            {
                                updatedCells[lblRow,lblCol + i] = countTxt;
                            }
                        }
                        else if (isColLabel)
                        {
                            if (isEditor)
                            {
                                cells[lblRow + i,lblCol].GetComponentInChildren<Text>().text = countTxt;
                            }
                            else
                            {
                                updatedCells[lblRow + i,lblCol] = countTxt;
                            }
                        }
                        else
                        {
                            Debug.LogError($"ERROR!");
                            return;
                        }
                    }
                } // end if valid lblRow and lblCol
            } // end for lblCol
        } // end for lblRow
    }

    static int[] ParseCoordinates(string coordStr)
    {
        string[] tokens = coordStr.Split(',');
        
        int r = Int32.Parse(tokens[0]);
        int c = Int32.Parse(tokens[1]);

        int[] coords = {r, c};
        return coords;
    }
}

}
