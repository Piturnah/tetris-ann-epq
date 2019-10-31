using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 
 * Controls the score and level
 */
[RequireComponent(typeof(TetrisEngine))]
public class ScoreController : MonoBehaviour
{
    [HideInInspector] public int score;
    [HideInInspector] public int level;
    [HideInInspector] public int lines;
    TetrisEngine engine;

    private void Start()
    {
        engine = GetComponent<TetrisEngine>();
    }

    public void RowClearScore(List<int> rows)
    {
        score += Tetrominoes.ComboMultiplier(rows.Count) * (level + 1);
        lines += rows.Count;
    }

    public void SoftDropScore(int toAdd)
    {
        score += toAdd;
    }
}
