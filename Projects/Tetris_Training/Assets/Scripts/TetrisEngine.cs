using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisEngine : MonoBehaviour
{
    const float _FRAME_RATE = 60.098813897441f;

    public int[,] field = new int[10, 22];
    public int[,] viewField = new int[10, 22];

    int[,,] tetrominoPool = new int[4, 4, 4];
    int[,] currentTetrominoState = new int[4, 4];
    int[] currentTetrominoPos = new int[2];

    int rotationState;

    int level = 5;
    float previousDropTime;

    private void Start()
    {
        SpawnTetromino(Random.Range(1, 8));
    }
    private void Update()
    {
        // Update the view field
        viewField = field.Clone() as int[,];
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (currentTetrominoState[x, y] == 1 && x + currentTetrominoPos[0] <= field.GetLength(0) - 1 && y + currentTetrominoPos[1] >= 0)
                {
                    viewField[x + currentTetrominoPos[0], y + currentTetrominoPos[1]] = currentTetrominoState[x, y];
                }
            }
        }

        DropTetromino();
    }

    // Drops the tetromino by one gridcell if the necessary time has passed
    void DropTetromino()
    {
        if (Time.time >= previousDropTime + DropFrameDelays.GetFrameDelay(level) / _FRAME_RATE)
        {
            previousDropTime = Time.time;
            currentTetrominoPos[1]--;
            DetectVerticalCollisions();
        }
    }

    // Move tetromino back up one gridcell and call AddTetrominoToField if there is a collision
    void DetectVerticalCollisions()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if ((currentTetrominoPos[1] + y < 0 || field[currentTetrominoPos[0] + x, currentTetrominoPos[1] + y] == 1) && currentTetrominoState[x,y] == 1)
                {
                    Debug.Log("Hit the ground!");
                    currentTetrominoPos[1]++;

                    AddTetrominoToField();
                    break;
                }
            }
        }
    }

    //Adds the tetromino to the field and instantiates a new one
    void AddTetrominoToField()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (currentTetrominoState[x, y] == 1 && x + currentTetrominoPos[0] <= field.GetLength(0) - 1 && y + currentTetrominoPos[1] >= 0)
                {
                    field[x + currentTetrominoPos[0], y + currentTetrominoPos[1]] = currentTetrominoState[x, y];
                }
            }
        }
        SpawnTetromino(Random.Range(1, 8));
    }

    //Rotates the tetromino: -1 for ACW, 1 for CW
    public void RotateTetromino(int direction)
    {
        rotationState += direction;
        if (rotationState == -1)
        {
            rotationState = 3;
        }

        currentTetrominoState = Slicer3D(tetrominoPool, rotationState % 4);
    }

    // Spawn a new tetromino at the top of the screen
    void SpawnTetromino(int tetrominoIndex)
    {
        switch(tetrominoIndex)
        {
            case 1:
                tetrominoPool = Tetrominoes.iTetromino;
                break;
            case 2:
                tetrominoPool = Tetrominoes.oTetromino;
                break;
            case 3:
                tetrominoPool = Tetrominoes.jTetromino;
                break;
            case 4:
                tetrominoPool = Tetrominoes.lTetromino;
                break;
            case 5:
                tetrominoPool = Tetrominoes.sTetromino;
                break;
            case 6:
                tetrominoPool = Tetrominoes.tTetromino;
                break;
            case 7:
                tetrominoPool = Tetrominoes.zTetromino;
                break;
        }
        currentTetrominoState = Slicer3D(tetrominoPool, 0);
        currentTetrominoPos[0] = 4;
        currentTetrominoPos[1] = 18;
    }

    // Takes a 2d "slice" from the array containing rotation info for a specific tetromino, i.e one rotation state
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
