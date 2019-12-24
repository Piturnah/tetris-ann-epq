using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TetrisAgent : MonoBehaviour
{
    public event Action<int> died;
    public NeuralNetwork neuralNet;

    TetrisEngine engine;

    private void Awake() {
        engine = GetComponent<TetrisEngine>();
    }

    private void Start() {
        engine.death += GiveScore;
    }

    void GiveScore(int score) {
        died?.Invoke(score);
    }

    private void Update() {
        GetOutputs();
    }

    void DoMove(float[] outputs) {
        switch (outputs.ToList().IndexOf(outputs.Max())) {
            case 0:
                engine.buttonInfo.lButton = true; break;
            case 1:
                engine.buttonInfo.dButton = true; break;
            case 2:
                engine.buttonInfo.rbutton = true; break;
            case 3:
                engine.buttonInfo.aButton = true; break;
            case 4:
                engine.buttonInfo.bButton = true; break;
            default: break;
        }
    }

    void GetOutputs() {
        // add field
        List<float> inputField = new List<float>();
        for (int x = 0; x < 10; x++) {
            for (int y = 0; y < 20; y++) {
                inputField.Add(engine.field[x][y]);
            }
        }
        // add tetromino state
        for (int x = 0; x < 4; x++) {
            for (int y = 0; y < 4; y++) {
                inputField.Add(engine.currentTetrominoState[x, y]);
            }
        }
        // add next tetromino index
        inputField.Add(engine.nextTetrominoIndex);
        // add tetromino position
        inputField.Add(engine.currentTetrominoPos[0]);
        inputField.Add(engine.currentTetrominoPos[1]);
        // add bias
        inputField.Add(1f);

        float[] outputs = neuralNet.GetNNResult(inputField.ToArray());
        DoMove(outputs);
    }
}
