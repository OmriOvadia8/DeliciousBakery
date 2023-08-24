using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

// Your existing BoardLayout class
public class BoardLayout : MonoBehaviour
{
    public LayoutRaw[] allRows;

    public Gem[,] GetLayout()
    {
        Gem[,] theLayout = new Gem[allRows[0].gemsInRow.Length, allRows.Length];

        for (int y = 0; y < allRows.Length; y++)
        {
            for (int x = 0; x < allRows[y].gemsInRow.Length; x++)
            {
                if (x < theLayout.GetLength(0))
                {
                    if (allRows[y].gemsInRow[x] != null)
                    {
                        theLayout[x, allRows.Length - 1 - y] = allRows[y].gemsInRow[x];
                    }
                }
            }
        }

        return theLayout;
    }
}

[System.Serializable]
public class LayoutRaw
{
    public Gem[] gemsInRow;
}

#if UNITY_EDITOR
[CustomEditor(typeof(BoardLayout))]
public class BoardLayoutEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BoardLayout boardLayout = (BoardLayout)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Add Row"))
        {
            AddRow(boardLayout);
        }

        if (GUILayout.Button("Remove Row"))
        {
            RemoveRow(boardLayout);
        }

        if (GUILayout.Button("Add Column"))
        {
            AddColumn(boardLayout);
        }

        if (GUILayout.Button("Remove Column"))
        {
            RemoveColumn(boardLayout);
        }

        // Add some space before drawing the grid
        GUILayout.Space(30);

        for (int y = 0; y < boardLayout.allRows.Length; y++)
        {
            LayoutRaw row = boardLayout.allRows[y];
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < row.gemsInRow.Length; x++)
            {
                Rect rect = EditorGUILayout.GetControlRect(false, 60);
                row.gemsInRow[x] = (Gem)EditorGUI.ObjectField(rect, row.gemsInRow[x], typeof(Gem), true);
                if (row.gemsInRow[x] != null && row.gemsInRow[x].gemSprite != null)
                {
                    GUI.DrawTexture(rect, row.gemsInRow[x].gemSprite.sprite.texture, ScaleMode.ScaleToFit);
                }

            }
            EditorGUILayout.EndHorizontal();
        }

        // Add "Clear Assignments" button
        if (GUILayout.Button("Clear Assignments"))
        {
            ClearAssignments(boardLayout);
        }

        // Save changes
        EditorUtility.SetDirty(boardLayout);
    }

    private void AddRow(BoardLayout boardLayout)
    {
        List<LayoutRaw> rows = new List<LayoutRaw>(boardLayout.allRows);

        if (rows.Count > 0)
        {
            int rowLength = rows[0].gemsInRow.Length;
            LayoutRaw newRow = new LayoutRaw { gemsInRow = new Gem[rowLength] };
            rows.Add(newRow);
        }
        else
        {
            rows.Add(new LayoutRaw { gemsInRow = new Gem[0] });
        }

        boardLayout.allRows = rows.ToArray();
    }

    private void RemoveRow(BoardLayout boardLayout)
    {
        if (boardLayout.allRows.Length > 0)
        {
            List<LayoutRaw> rows = new List<LayoutRaw>(boardLayout.allRows);
            rows.RemoveAt(rows.Count - 1);
            boardLayout.allRows = rows.ToArray();
        }
    }

    private void AddColumn(BoardLayout boardLayout)
    {
        for (int i = 0; i < boardLayout.allRows.Length; i++)
        {
            List<Gem> gemsInRow = new List<Gem>(boardLayout.allRows[i].gemsInRow);
            gemsInRow.Add(null);
            boardLayout.allRows[i].gemsInRow = gemsInRow.ToArray();
        }
    }

    private void RemoveColumn(BoardLayout boardLayout)
    {
        for (int i = 0; i < boardLayout.allRows.Length; i++)
        {
            if (boardLayout.allRows[i].gemsInRow.Length > 0)
            {
                List<Gem> gemsInRow = new List<Gem>(boardLayout.allRows[i].gemsInRow);
                gemsInRow.RemoveAt(gemsInRow.Count - 1);
                boardLayout.allRows[i].gemsInRow = gemsInRow.ToArray();
            }
        }
    }
    private void ClearAssignments(BoardLayout boardLayout)
    {
        for (int y = 0; y < boardLayout.allRows.Length; y++)
        {
            for (int x = 0; x < boardLayout.allRows[y].gemsInRow.Length; x++)
            {
                boardLayout.allRows[y].gemsInRow[x] = null;
            }
        }
    }
}
#endif
