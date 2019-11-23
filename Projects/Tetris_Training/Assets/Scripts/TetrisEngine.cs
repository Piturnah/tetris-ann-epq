using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/*
 * This is the engine responsible for running Tetris. During training, multiple instances of this object will exist, one for each AI agent.
 * The tetris field is stored in the 2D array field, and the currently active tetromino is stored in the 2D array currentTetrominoState.
 * field only stores values for tetrominoes that have already landed, so for visualisation the method UpdateViewField can be called to
 * return 2D array viewField which is a combination of field and currentTetrominoState based on the tetromino's position.
 */
 [RequireComponent(typeof(ScoreController))]
public class TetrisEngine : MonoBehaviour
{
    public ButtonInfo buttonInfo = new ButtonInfo();
    const float _FRAME_RATE = 60.098813897441f;

    public int[][] field = new int[10][];
    public int[][] viewField = new int[10][];
    int[][] previousViewField = new int[10][];

    public int nextTetrominoIndex;

    int[,,] tetrominoPool = new int[4, 4, 4];
    int[,] currentTetrominoState = new int[4, 4];
    int[] currentTetrominoPos = new int[2];

    int rotationState;

    [HideInInspector] public int startLevel = 0;
    [HideInInspector] public int level;
    float previousDropTime;
    float previousDASUpdate;

    public event Action updateField;
    public event Action tetrominoSpawned;
    public event Action nextAnimFrame;
    public static event Action<int, GameObject> death;
    int softDropCounter;

    [HideInInspector]public bool are;
    [HideInInspector]public int frameCounter;
    [HideInInspector]public ScoreController score;

    bool dead;
    int count18; // counts row 18 blocks. if tetromino stops on 18th row twice, engine dies

    private void Start()
    {
        score = GetComponent<ScoreController>();
        score.level = startLevel;

        // Initialise nested arrays of field
        for (int i = 0; i < field.Length; i++)
        {
            field[i] = new int[22];
            viewField[i] = new int[22];
            previousViewField[i] = new int[22];
        }

        // Spawn first tetromino
        nextTetrominoIndex = UnityEngine.Random.Range(1, 8);
        SpawnTetromino(nextTetrominoIndex);
    }
    private void Update()
    {
        UpdateViewField();

        // Don't do these during entry delay
        if (!are)
        {
            DropTetromino();
            DAS();
        }
        if (Time.time * _FRAME_RATE > frameCounter + 1)
        {
            frameCounter++;
        }
        buttonInfo.dPreviousFrame = buttonInfo.dButton;
    }

    // Update the view field
    void UpdateViewField()
    {
        viewField = CopyArray(field);
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (currentTetrominoState[x, y] != 0 && x + currentTetrominoPos[0] <= field.Length -1 && y + currentTetrominoPos[1] >= 0)
                {
                    viewField[x + currentTetrominoPos[0]][y + currentTetrominoPos[1]] = currentTetrominoState[x, y];
                }
            }
        }
        // If the view field is different to last frame, call event updateField
        if (!JaggedsEqual(viewField, previousViewField))
        {
            updateField?.Invoke();
            previousViewField = CopyArray(viewField);
        }
    }

    // Returns true if the values of the jagged arrays are equivalent
    bool JaggedsEqual(int[][] array1, int[][] array2)
    {
        for (int row = 0; row < array1.Length; row++)  
        {
            if (!array1[row].SequenceEqual(array2[row]))
            {
                return false;
            }
        }
        return true;
    }
    
    //Calculate DAS and do horizontal shifting
    void DAS()
    {
        if (frameCounter >= previousDASUpdate + 1)
        {
            previousDASUpdate = frameCounter;

            //Determine whether to reset DAS counter or to increment DAS counter
            if ((buttonInfo.rbutton || buttonInfo.lButton) && !buttonInfo.lrPreviousFrame)
            {
                buttonInfo.dasCounter = 0; // Resets counter to 0 if started inputting again after no input previous frame
            }
            else if ((buttonInfo.rbutton || buttonInfo.lButton) && buttonInfo.lrPreviousFrame)
            {
                buttonInfo.dasCounter = Mathf.Clamp(buttonInfo.dasCounter + 1, 0, 16); // Increments the counter if inputting current frame and previous frame
            }

            if ((buttonInfo.dasCounter == 0 || buttonInfo.dasCounter == 16) && (buttonInfo.lButton || buttonInfo.rbutton))
            {
                // Sets DAS counter to 10 if in quick delay mode
                if (buttonInfo.dasCounter == 16)
                {
                    buttonInfo.dasCounter = 10;
                }
                // Calls for tetromino to be shifted, indicates direction by passing bool (if true, then left)
                if (!ShiftTetromino(buttonInfo.lButton))
                {
                    buttonInfo.dasCounter = (buttonInfo.dasCounter == 0) ? 16 : buttonInfo.dasCounter;
                }
            }
        }

        // Update values from previous frame
        buttonInfo.lrPreviousFrame = buttonInfo.lButton || buttonInfo.rbutton;
    }

    bool ShiftTetromino(bool goingLeft)
    {
        currentTetrominoPos[0] = currentTetrominoPos[0] + ((goingLeft) ? -1 : 1);

        bool collided = DetectHorizontalCollisions();
        if (collided)
        {
            currentTetrominoPos[0] = currentTetrominoPos[0] + ((goingLeft) ? 1 : -1);
        }

        return !collided;
    }

    bool DetectHorizontalCollisions()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if ((currentTetrominoState[x, y] != 0) && (currentTetrominoPos[0] + x < 0 || currentTetrominoPos[0] + x > 9 || field[currentTetrominoPos[0] + x][currentTetrominoPos[1] + y] != 0))
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
        bool softDroppingThisFrame = buttonInfo.dButton && Time.time >= previousDropTime + Mathf.Min(2, DropFrameDelays.GetFrameDelay(score.level)) / _FRAME_RATE;
        if (Time.time >= previousDropTime + DropFrameDelays.GetFrameDelay(score.level) / _FRAME_RATE || softDroppingThisFrame)
        {
            if (softDroppingThisFrame)
            {
                if (buttonInfo.softDropCounter == 16)
                {
                    buttonInfo.softDropCounter = 10;
                }
                if (buttonInfo.dPreviousFrame)
                {
                    buttonInfo.softDropCounter++;
                } else
                {
                    buttonInfo.softDropCounter = 0;
                }
            }

            previousDropTime = Time.time;
            currentTetrominoPos[1]--;
            if (DetectVerticalCollisions())
            {
                if (softDroppingThisFrame)
                {
                    score.SoftDropScore(buttonInfo.softDropCounter);
                }
                currentTetrominoPos[1]++;

                int minYPos = AddTetrominoToField(); // returns minimum y pos of tetromino after adding it to field
                if (!CheckForLines(minYPos)) //TODO: Fix this bs
                {
                    StartCoroutine(LineClearAnimation(new List<int>(), minYPos));
                    if (minYPos + currentTetrominoPos[1] >= 18)
                    {
                        count18++;
                        if (count18 >= 2)
                        {
                            death?.Invoke(score.score, gameObject); // this engine dies
                            dead = true;
                            buttonInfo.Reset();
                        }
                    }
                }
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
                if ((currentTetrominoState[x,y] != 0) && (currentTetrominoPos[1] + y < 0 || field[currentTetrominoPos[0] + x][currentTetrominoPos[1] + y] != 0))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Adds the tetromino to the field and instantiates a new one, returns minimum local y position
    int AddTetrominoToField()
    {
        int minYPos = int.MaxValue;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (currentTetrominoState[x, y] != 0 && x + currentTetrominoPos[0] <= field.GetLength(0) - 1 && y + currentTetrominoPos[1] >= 0)
                {
                    minYPos = Mathf.Min(minYPos, y);
                    field[x + currentTetrominoPos[0]][y + currentTetrominoPos[1]] = currentTetrominoState[x, y];
                }
            }
        }
        currentTetrominoState = Tetrominoes.zeros;
        if (GetComponent<EngineUI>() != null)
        {
            GetComponent<EngineUI>().yLock = minYPos + currentTetrominoPos[1];
        }
        return minYPos;
    }
    
    // Entry delay then spawns new tetromino
    IEnumerator ARE(int waitFrames)
    {
        are = true;
        yield return new WaitForSeconds(waitFrames / _FRAME_RATE);
        SpawnTetromino(nextTetrominoIndex);
        are = false;
    }

    // Called when a tetromino has landed. Iterates over each row of field and checks if there are any lines.
    bool CheckForLines(int minYPos)
    {
        bool lineFound = false;
        List<int> linesFound = new List<int>();
        for (int y = 20; y >= 0; y--)
        {
            if (CheckRow(y))
            {
                lineFound = true;
                linesFound.Add(y);
            }
        }
        if (lineFound)
        {
            StartCoroutine(LineClearAnimation(linesFound, minYPos));
            score.RowClearScore(linesFound);
        }
        return lineFound;
    }
    IEnumerator LineClearAnimation(List<int> yRows, int minYPos)
    {
        for (int i = 0; i < yRows.Count(); i++)
        {
            StartCoroutine(ClearLine(yRows[i], i, minYPos));
        }
        if (!yRows.Any())
        {
            StartCoroutine(ARE(EntryFrameDelays.GetFrameDelay(currentTetrominoPos[1] + minYPos))); // tetromino spawned in ARE
        }
        yield return null;
    }

    IEnumerator ClearLine(int y, int j, int minYPos)
    {
        float timeThen = Time.time;
        are = true;
        for (int i = 0; i < 5; i++)
        {
            if (j == 3) // make screen flash if tetris (and if has EngineUI attached)
            {
                nextAnimFrame?.Invoke();
            }
            yield return new WaitForSeconds((4 - frameCounter % 4) / _FRAME_RATE); // Time in seconds before frame counter % 4 = 0
            field[i + 5][y] = 0;
            field[4 - i][y] = 0;
        }
        are = false;
        FallAboveRows(y);
        if (j == 0) // first iteration of clearline
        {
            StartCoroutine(ARE(EntryFrameDelays.GetFrameDelay(currentTetrominoPos[1] + minYPos))); // tetromino spawned in ARE
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
        if (!dead)
        {
            buttonInfo.softDropCounter = 0;
            nextTetrominoIndex = UnityEngine.Random.Range(1, 8);

            tetrominoPool = Tetrominoes.GetTetrominoFromIndex(tetrominoIndex);
            currentTetrominoState = Slicer3D(tetrominoPool, 0);
            rotationState = 0;
            currentTetrominoPos[0] = 3;
            currentTetrominoPos[1] = 18;

            tetrominoSpawned?.Invoke();
        }
    }

    // Takes a 2d "slice" from the array containing rotation info for a specific tetromino, i.e one rotation state
    public int[,] Slicer3D (int[,,] toSlice, int dimension)
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

    static int[][] CopyArray(int[][] source)
    {
        int len = source.Length;
        int[][] dest = new int[len][];

        for (int x = 0; x < len; x++)
        {
            int[] inner = source[x];
            int ilen = inner.Length;
            int[] newer = new int[ilen];
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
        public int softDropCounter;

        public bool dPreviousFrame;
        public bool dButton;

        public bool aButton;
        public bool bButton;

        public void Reset()
        {
            lrPreviousFrame = lButton = rbutton = dPreviousFrame = dButton = aButton = bButton = false;
        }
    }
}
