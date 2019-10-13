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
         {0, 1, 0, 0 },
         {0, 1, 0, 0 },
         {1, 1, 0, 0 } },

        // 90 degrees
        {{0, 0, 0, 0 },
         {1, 0, 0, 0 },
         {1, 1, 1, 0 },
         {0, 0, 0, 0 } },

        // 180 degrees
        {{0, 0, 0, 0 },
         {0, 1, 1, 0 },
         {0, 1, 0, 0 },
         {0, 1, 0, 0 } },

        // 270 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {1, 1, 1, 0 },
         {0, 0, 1, 0 } }
    };
    public static int[,,] lTetromino =
    {
        // 0 degrees
        {{0, 0, 0, 0 },
         {1, 1, 0, 0 },
         {0, 1, 0, 0 },
         {0, 1, 0, 0 } },

        // 90 degrees
        {{0, 0, 0, 0 },
         {0, 0, 1, 0 },
         {1, 1, 1, 0 },
         {0, 0, 0, 0 } },

        // 180 degrees
        {{0, 0, 0, 0 },
         {0, 1, 0, 0 },
         {0, 1, 0, 0 },
         {0, 1, 1, 0 } },

        // 270 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {1, 1, 1, 0 },
         {1, 0, 0, 0 } }
    };
    public static int[,,] sTetromino =
    {
        // 0 degrees
        {{0, 0, 0, 0 },
         {1, 0, 0, 0 },
         {1, 1, 0, 0 },
         {0, 1, 0, 0 } },

        // 90 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {0, 1, 1, 0 },
         {1, 1, 0, 0 } },

        // 180 degrees
        {{0, 0, 0, 0 },
         {1, 0, 0, 0 },
         {1, 1, 0, 0 },
         {0, 1, 0, 0 } },

        // 270 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {0, 1, 1, 0 },
         {1, 1, 0, 0 } }
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
         {0, 1, 0, 0 },
         {1, 1, 0, 0 },
         {1, 0, 0, 0 } },

        // 90 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {1, 1, 0, 0 },
         {0, 1, 1, 0 } },

        // 180 degrees
        {{0, 0, 0, 0 },
         {0, 1, 0, 0 },
         {1, 1, 0, 0 },
         {1, 0, 0, 0 } },

        // 270 degrees
        {{0, 0, 0, 0 },
         {0, 0, 0, 0 },
         {1, 1, 0, 0 },
         {0, 1, 1, 0 } }
    };
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