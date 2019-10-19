using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * This is the engine responsible for running Tetris. During training, multiple instances of this object will exist, one for each AI agent.
 * The tetris field is stored in the 2D array field, and the currently active tetromino is stored in the 2D array currentTetrominoState.
 * field only stores values for tetrominoes that have already landed, so for visualisation the method UpdateViewField can be called to
 * return 2D array viewField which is a combination of field and currentTetrominoState based on the tetromino's position.
 */
public class TetrisEngine : MonoBehaviour
{
    public ButtonInfo buttonInfo = new ButtonInfo();
    const float _FRAME_RATE = 60.098813897441f;

    public int[][] field = new int[10][];
    public int[][] viewField = new int[10][];

    int[,,] tetrominoPool = new int[4, 4, 4];
    int[,] currentTetrominoState = new int[4, 4];
    int[] currentTetrominoPos = new int[2];

    int rotationState;

    int level;
    float previousDropTime;
    float previousDASUpdate;

    private void Start()
    {
        // Initialise nested arrays of field
        for (int i = 0; i < field.Length; i++)
        {
            field[i] = new int[22];
            viewField[i] = new int[22];
        }

        // Spawn first tetromino
        SpawnTetromino(UnityEngine.Random.Range(1, 8));
    }
    private void Update()
    {
        UpdateViewField();
        DropTetromino();

        DAS();
    }

    // Update the view field
    void UpdateViewField()
    {
        viewField = CopyArrayBuiltIn(field);
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (currentTetrominoState[x, y] == 1 && x + currentTetrominoPos[0] <= field.Length -1 && y + currentTetrominoPos[1] >= 0)
                {
                    viewField[x + currentTetrominoPos[0]][y + currentTetrominoPos[1]] = currentTetrominoState[x, y];
                }
            }
        }
    }
    
    //Calculate DAS and do horizontal shifting
    void DAS()
    {
        if (Time.time >= previousDASUpdate + 1 / _FRAME_RATE)
        {
            previousDASUpdate = Time.time;

            //Determine whether to reset DAS counter or to increment DAS counter
            if ((buttonInfo.rbutton == true || buttonInfo.lButton == true) && buttonInfo.lrPreviousFrame == false)
            {
                buttonInfo.dasCounter = 0;
            }
            else if ((buttonInfo.rbutton == true || buttonInfo.lButton == true) && buttonInfo.lrPreviousFrame == true)
            {
                buttonInfo.dasCounter++;
            }

            if ((buttonInfo.dasCounter == 0 || buttonInfo.dasCounter == 16) && (buttonInfo.lButton || buttonInfo.rbutton))
            {
                // Sets DAS counter to 10 if in quick delay mode
                if (buttonInfo.dasCounter == 16)
                {
                    buttonInfo.dasCounter = 10;
                }
                // Calls for tetromino to be shifted, indicates direction by passing bool (if true, then left)
                ShiftTetromino(buttonInfo.lButton);
            }
        }

        // Update values from previous frame
        buttonInfo.lrPreviousFrame = buttonInfo.lButton || buttonInfo.rbutton;
    }

    void ShiftTetromino(bool goingLeft)
    {
        currentTetrominoPos[0] = currentTetrominoPos[0] + ((goingLeft) ? -1 : 1);
        if (DetectHorizontalCollisions())
        {
            currentTetrominoPos[0] = currentTetrominoPos[0] + ((goingLeft) ? 1 : -1);
        }
    }

    bool DetectHorizontalCollisions()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if ((currentTetrominoState[x, y] == 1) && (currentTetrominoPos[0] + x < 0 || currentTetrominoPos[0] + x > 9 || field[currentTetrominoPos[0] + x][currentTetrominoPos[1] + y] == 1))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Drops the tetromino by one gridcell if the necessary time has passed
    void DropTetromino()
    {
        if (Time.time >= previousDropTime + DropFrameDelays.GetFrameDelay(level) / _FRAME_RATE)
        {
            previousDropTime = Time.time;
            currentTetrominoPos[1]--;
            if (DetectVerticalCollisions())
            {
                Debug.Log("Hit the ground!");
                currentTetrominoPos[1]++;

                AddTetrominoToField();
                CheckForLines();
            }
        }
    }

    // Move tetromino back up one gridcell and call AddTetrominoToField if there is a collision
    bool DetectVerticalCollisions()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if ((currentTetrominoState[x,y] == 1) && (currentTetrominoPos[1] + y < 0 || field[currentTetrominoPos[0] + x][currentTetrominoPos[1] + y] == 1))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Adds the tetromino to the field and instantiates a new one
    void AddTetrominoToField()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (currentTetrominoState[x, y] == 1 && x + currentTetrominoPos[0] <= field.GetLength(0) - 1 && y + currentTetrominoPos[1] >= 0)
                {
                    field[x + currentTetrominoPos[0]][y + currentTetrominoPos[1]] = currentTetrominoState[x, y];
                }
            }
        }
        SpawnTetromino(UnityEngine.Random.Range(1, 8));
    }

    // Called when a tetromino has landed. Iterates over each row of field and checks if there are any lines.
    void CheckForLines()
    {
        for (int y = 20; y >= 0; y--)
        {
            if (CheckRow(y))
            {
                for (int x = 0; x < 10; x++)
                {
                    field[x][y] = 0;
                }
                FallAboveRows(y);
            }
        }
    }
    // Called by CheckForLines, returns true when a row of field (y) has no empty spaces.
    bool CheckRow(int y)
    {
        for (int x = 0; x < 10; x++)
        {
            if (field[x][y] == 0)
            {
                return false;
            }
        }
        return true;
    }
    //Called by CheckForLines, shifts rows above the deleted row down.
    void FallAboveRows(int row)
    {
        for (int y = row + 1; y < 20; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                field[x][y - 1] = field[x][y];
                field[x][y] = 0;
            }
        }
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
        if (DetectRotationCollisions())
        {
            Debug.Log("Reverting rotation");
            rotationState -= direction;
        }
        currentTetrominoState = Slicer3D(tetrominoPool, rotationState % 4);
    }

    bool DetectRotationCollisions()
    {
        if (DetectHorizontalCollisions() || DetectVerticalCollisions())
        {
            return true;
        }
        return false;
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
        rotationState = 0;
        currentTetrominoPos[0] = 3;
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

    static int[][] CopyArrayBuiltIn(int[][] source)
    {
        var len = source.Length;
        var dest = new int[len][];

        for (var x = 0; x < len; x++)
        {
            var inner = source[x];
            var ilen = inner.Length;
            var newer = new int[ilen];
            Array.Copy(inner, newer, ilen);
            dest[x] = newer;
        }

        return dest;
    }

    // Class to store info about the buttons being "pressed"
    public class ButtonInfo
    {
        public bool lrPreviousFrame;

        public bool lButton;
        public bool rbutton;

        public int dasCounter;
    }
}
