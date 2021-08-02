using UnityEngine;

namespace nonogram.labelcell
{

public class LabelCell : MonoBehaviour
{
    public void GetStatus(int row, int col)
    {
        Debug.Log($"Label_{row},{col} selected\n");
    }
}

}
