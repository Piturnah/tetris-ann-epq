using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisEngine : MonoBehaviour
{
    public int[,] field = new int[10, 20];
    public int[,] viewField = new int[10, 20];
    int iteration = 0;

    private void Start()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                viewField[x, y] = Slicer3D(Tetrominoes.lTetromino, 0)[x, y];
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    viewField[x, y] = Slicer3D(Tetrominoes.zTetromino, iteration % 4)[x, y];
                }
            }
            iteration++;
        }
    }

    int[,] Slicer3D (int[,,] toSlice, int dimension)
    {
        int[,] slice = new int[4, 4];

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                slice[x, y] = toSlice[dimension, x, y];
            }
        }

        return slice;
    }
}
