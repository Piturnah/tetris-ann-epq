using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TetrisAgent : MonoBehaviour
{
    public event Action<int, GameObject, int> died;
    public NeuralNetwork neuralNet;

    TetrisEngine engine;
    int previousInputFrame;
    bool rendered = false;
    EngineUI uiEngine;

    private void Awake() {
        engine = GetComponent<TetrisEngine>();
    }

    private void Start() {
        engine.death += GiveScore;
        if (GetComponent<EngineUI>().isActiveAndEnabled) {
            rendered = true;
            uiEngine = GetComponent<EngineUI>();

            NEATRenderer neatRenderer = new NEATRenderer();
            
            if (GameObject.Find("ParentObj(Clone)") != null) {
                Destroy(GameObject.Find("ParentObj(Clone)"));
            }
            if (GameObject.Find("ParentObj") != null) {
                Destroy(GameObject.Find("ParentObj"));
            }

            neatRenderer.DrawGenome(neuralNet.genome, new Vector3 (16, 8.2f, 0), new Vector2(11,12f));
        }
        Clock.clockTick += GetOutputs;
    }

    void GiveScore(int score) {
        died?.Invoke(engine.score.score, gameObject, engine.dropCounter);
    }

    void DoMove(float[] outputs) {
        engine.buttonInfo.ResetInputs();
        switch (outputs.ToList().IndexOf(outputs.Max())) {
            case 0: break;
            case 1:
                engine.buttonInfo.lButton = true; break;
            case 2:
                engine.buttonInfo.dButton = true; break;
            case 3:
                engine.buttonInfo.rbutton = true; break;
            case 4:
                engine.RotateTetromino(-1);
                engine.buttonInfo.aButton = true; break;
            case 5:
                engine.RotateTetromino(1);
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
        // add lr prev frame info
        inputField.Add((engine.buttonInfo.lrPreviousFrame)?1:0);
        // add bias
        inputField.Add(1f);

        float[] outputs = neuralNet.GetNNResult(inputField.ToArray());
        DoMove(outputs);

        if (rendered) {
            uiEngine.UpdateControllerActivations(outputs);
        }
    }
}
