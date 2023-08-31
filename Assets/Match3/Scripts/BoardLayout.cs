using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DB_Match3
{
    public class BoardLayout : MonoBehaviour
    {
        public LayoutRow[] allRows;

        public Gem[,] GetLayout()
        {
            int width = allRows[0].gemsInRow.Length;
            int height = allRows.Length;
            Gem[,] theLayout = new Gem[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Gem gem = allRows[y].gemsInRow[x];
                    if (gem != null)
                    {
                        theLayout[x, height - 1 - y] = gem;
                    }
                }
            }

            return theLayout;
        }
    }

    [System.Serializable]
    public class LayoutRow
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
            DrawButtons(boardLayout);
            GUILayout.Space(30);
            DrawGrid(boardLayout);

            if (GUILayout.Button("Clear Assignments"))
            {
                ClearAssignments(boardLayout);
            }

            EditorUtility.SetDirty(boardLayout);
        }

        private void DrawButtons(BoardLayout boardLayout)
        {
            if (GUILayout.Button("Add Row")) AddRow(boardLayout);
            if (GUILayout.Button("Remove Row")) RemoveRow(boardLayout);
            if (GUILayout.Button("Add Column")) AddColumn(boardLayout);
            if (GUILayout.Button("Remove Column")) RemoveColumn(boardLayout);
        }

        private void DrawGrid(BoardLayout boardLayout)
        {
            foreach (LayoutRow row in boardLayout.allRows)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < row.gemsInRow.Length; x++)
                {
                    DrawGemField(row.gemsInRow[x], row, x);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawGemField(Gem gem, LayoutRow row, int x)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 60);
            row.gemsInRow[x] = (Gem)EditorGUI.ObjectField(rect, gem, typeof(Gem), true);
            if (gem?.gemSprite != null)
            {
                GUI.DrawTexture(rect, gem.gemSprite.sprite.texture, ScaleMode.ScaleToFit);
            }
        }

        private void AddRow(BoardLayout boardLayout)
        {
            List<LayoutRow> rows = new List<LayoutRow>(boardLayout.allRows);
            if (rows.Count > 0)
            {
                int rowLength = rows[0].gemsInRow.Length;
                LayoutRow newRow = new LayoutRow { gemsInRow = new Gem[rowLength] };
                rows.Add(newRow);
            }
            else
            {
                rows.Add(new LayoutRow { gemsInRow = new Gem[0] });
            }
            boardLayout.allRows = rows.ToArray();
        }

        private void RemoveRow(BoardLayout boardLayout)
        {
            if (boardLayout.allRows.Length > 0)
            {
                List<LayoutRow> rows = new List<LayoutRow>(boardLayout.allRows);
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
}