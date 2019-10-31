public static class Tetrominoes
{
    public static int[,,] iTetromino =
    {
        // 0 degrees
        {{0, 1, 0, 0 },
         {0, 1, 0, 0 },
         {0, 1, 0, 0 },
         {0, 1, 0, 0 } },

        // 90 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {1, 1, 1, 1 },
         {0, 0, 0, 0 } },

        // 180 degrees
        {{0, 1, 0, 0 },
         {0, 1, 0, 0 },
         {0, 1, 0, 0 },
         {0, 1, 0, 0 } },
        
        //270 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {1, 1, 1, 1 },
         {0, 0, 0, 0 } }
    };
    public static int[,,] oTetromino =
    {
        // 0 degrees
        {{0, 0, 0, 0 },
         {1, 1, 0, 0 },
         {1, 1, 0, 0 },
         {0, 0, 0, 0 } },

        // 90 degrees
        {{0, 0, 0, 0 },
         {1, 1, 0, 0 },
         {1, 1, 0, 0 },
         {0, 0, 0, 0 } },

        // 180 degrees
        {{0, 0, 0, 0 },
         {1, 1, 0, 0 },
         {1, 1, 0, 0 },
         {0, 0, 0, 0 } },

        // 270 degrees
        {{0, 0, 0, 0 },
         {1, 1, 0, 0 },
         {1, 1, 0, 0 },
         {0, 0, 0, 0 } }
    };
    public static int[,,] jTetromino =
    {
        // 0 degrees
        {{0, 0, 0, 0 },
         {0, 2, 0, 0 },
         {0, 2, 0, 0 },
         {2, 2, 0, 0 } },

        // 90 degrees
        {{0, 0, 0, 0 },
         {2, 0, 0, 0 },
         {2, 2, 2, 0 },
         {0, 0, 0, 0 } },

        // 180 degrees
        {{0, 0, 0, 0 },
         {0, 2, 2, 0 },
         {0, 2, 0, 0 },
         {0, 2, 0, 0 } },

        // 270 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {2, 2, 2, 0 },
         {0, 0, 2, 0 } }
    };
    public static int[,,] lTetromino =
    {
        // 0 degrees
        {{0, 0, 0, 0 },
         {3, 3, 0, 0 },
         {0, 3, 0, 0 },
         {0, 3, 0, 0 } },

        // 90 degrees
        {{0, 0, 0, 0 },
         {0, 0, 3, 0 },
         {3, 3, 3, 0 },
         {0, 0, 0, 0 } },

        // 180 degrees
        {{0, 0, 0, 0 },
         {0, 3, 0, 0 },
         {0, 3, 0, 0 },
         {0, 3, 3, 0 } },

        // 270 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {3, 3, 3, 0 },
         {3, 0, 0, 0 } }
    };
    public static int[,,] sTetromino =
    {
        // 0 degrees
        {{0, 0, 0, 0 },
         {2, 0, 0, 0 },
         {2, 2, 0, 0 },
         {0, 2, 0, 0 } },

        // 90 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {0, 2, 2, 0 },
         {2, 2, 0, 0 } },

        // 180 degrees
        {{0, 0, 0, 0 },
         {2, 0, 0, 0 },
         {2, 2, 0, 0 },
         {0, 2, 0, 0 } },

        // 270 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {0, 2, 2, 0 },
         {2, 2, 0, 0 } }
    };
    public static int[,,] tTetromino =
    {
        // 0 degrees
        {{0, 0, 0, 0 },
         {0, 1, 0, 0 },
         {1, 1, 0, 0 },
         {0, 1, 0, 0 } },

        // 90 degrees
        {{0, 0, 0, 0 },
         {0, 1, 0, 0 },
         {1, 1, 1, 0 },
         {0, 0, 0, 0 } },

        // 180 degrees
        {{0, 0, 0, 0 },
         {0, 1, 0, 0 },
         {0, 1, 1, 0 },
         {0, 1, 0, 0 } },

        // 270 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {1, 1, 1, 0 },
         {0, 1, 0, 0 } }
    };
    public static int[,,] zTetromino =
    {
        // 0 degrees
        {{0, 0, 0, 0 },
         {0, 3, 0, 0 },
         {3, 3, 0, 0 },
         {3, 0, 0, 0 } },

        // 90 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {3, 3, 0, 0 },
         {0, 3, 3, 0 } },

        // 180 degrees
        {{0, 0, 0, 0 },
         {0, 3, 0, 0 },
         {3, 3, 0, 0 },
         {3, 0, 0, 0 } },

        // 270 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {3, 3, 0, 0 },
         {0, 3, 3, 0 } }
    };
    public static int[,] zeros = 
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {0, 0, 0, 0 } };

    public static int[,,] GetTetrominoFromIndex(int index)
    {
        switch (index)
        {
            case 1:
                return iTetromino;
            case 2:
                return oTetromino;
            case 3:
                return jTetromino;
            case 4:
                return lTetromino;
            case 5:
                return sTetromino;
            case 6:
                return tTetromino;
            case 7:
                return zTetromino;
            default:
                return iTetromino;
        }
    }

    public static int ComboMultiplier(int combo)
    {
        switch (combo)
        {
            case 1:
                return 40;
            case 2:
                return 100;
            case 3:
                return 300;
            case 4:
                return 1200;
            default:
                return 0;
        }
    }
}

public static class EntryFrameDelays
{
    // Returns the amount of frames to wait depending on the row the tetromino was locked on
    public static int GetFrameDelay(int lockRow)
    {
        switch (lockRow)
        {
            case 0: case 1:
                return 10;
            case 2: case 3: case 4: case 5:
                return 12;
            case 6: case 7: case 8: case 9:
                return 14;
            case 10: case 11: case 12: case 13:
                return 16;
            case 14: case 15: case 16: case 17:
                return 18;
            default:
                return 20;
        }
    }
}

public static class DropFrameDelays
{
    // Returns the amount of time a tetronimo rests at each vertical gridcell, in frames, in accordance to the current level
    public static int GetFrameDelay(int level)
    {
        if (level >= 19 && level <= 28)
        {
            level = 19;
        }
        switch (level)
        {
            case 0: return 48;
            case 1: return 43;
            case 2: return 38;
            case 3: return 33;
            case 4: return 28;
            case 5: return 23;
            case 6: return 18;
            case 7: return 13;
            case 8: return 8;
            case 9: return 6;
            case 10: case 11: case 12: return 5;
            case 13: case 14: case 15: return 4;
            case 16: case 17: case 18: return 3;
            case 19: return 2;
            default:
                return 1;
        }
    }
}