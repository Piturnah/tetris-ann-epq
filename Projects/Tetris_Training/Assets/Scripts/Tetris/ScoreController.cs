using System.Collections.Generic;
using UnityEngine;

/* 
 * Controls the score and level
 */
[RequireComponent(typeof(TetrisEngine))]
public class ScoreController : MonoBehaviour
{
    [HideInInspector] public int score;
    [HideInInspector] public int level;
    [HideInInspector] public int lines;
    int previousLevelLines;
    int requiredLines;
    TetrisEngine engine;

    private void Start()
    {
        engine = GetComponent<TetrisEngine>();
        requiredLines = Mathf.Min(engine.startLevel * 10 + 10, Mathf.Max(100, engine.startLevel * 10 - 50));
    }

    public void RowClearScore(List<int> rows)
    {
        score += Tetrominoes.ComboMultiplier(rows.Count) * (level + 1);
        lines += rows.Count;
        CheckLevel();
    }

    public void SoftDropScore(int toAdd)
    {
        score += toAdd;
    }

    void CheckLevel()
    {
        if (lines >= requiredLines)
        {
            level++;
            requiredLines += 10;
        }
    }
}
