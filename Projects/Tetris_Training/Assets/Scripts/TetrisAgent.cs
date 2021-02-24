using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Burst;
using Unity.Mathematics;

public class TetrisAgent : MonoBehaviour
{
    public event Action<int, GameObject, int> died;
    public NeuralNetwork neuralNet;

    TetrisEngine engine;
    int previousInputFrame;
    bool rendered = false;
    EngineUI uiEngine;

    float[] weights;

    private void Awake() {
        engine = GetComponent<TetrisEngine>();
    }

    private void Start() {
        if (Manager.gameType == Manager.GameType.Display) {
            neuralNet = new NeuralNetwork(Manager.displayGenome);
        }
        weights = new float[44400];
        GetHyperWeights();
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

            //neatRenderer.DrawGenome(neuralNet.genome, new Vector3 (16, 8.2f, 0), new Vector2(11,12f));
        }
        Clock.clockTick += GetOutputs;

    }

    private void OnDestroy() {
        Clock.clockTick -= GetOutputs;
        engine.death -= GiveScore;
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

    public void GetHyperWeights() {
        //Loop tetromino position
        for (int x1 = 0; x1 < 10; x1++) {
            for (int y1 = 0; y1 < 20; y1++) {
                //Loop tetromino state
                for (int x2 = 0; x2 < 4; x2++) {
                    for (int y2 = 0; y2 < 4; y2++) {
                        weights[y1 * 160 + x1 * 16 + y2 * 4 + x2] = neuralNet.GetNNResult(new float[] { math.lerp(-1f, 1f, x2 / 3f), math.lerp(-1f, 1f, y2 / 3f), math.lerp(-1f, 1f, x1 / 9f), math.lerp(-1f, 1f, y1 / 19f) });
                    }
                }
                //Loop gameboard cells
                for (int x2 = 0; x2 < 10; x2++) {
                    for (int y2 = 0; y2 < 20; y2++) {
                        weights[3200 + y2 * 2000 + x2 * 200 + y1 * 10 + x1] = neuralNet.GetNNResult(new float[] { math.lerp(-1f, 1f, x1 / 9f), math.lerp(-1f, 1f, y1 / 19f), math.lerp(-1f, 1f, x2 / 9f), math.lerp(-1f, 1f, y2 / 19f) });
                    }
                }
                //Loop outputs
                for (int x2 = 0; x2 < 3; x2++) {
                    for (int y2 = 0; y2 < 2; y2++) {
                        weights[43200 + y2 * 300 + x2 * 100 + y1 * 10 + x1] = neuralNet.GetNNResult(new float[] { math.lerp(-1f, 1f, x1 / 9f), math.lerp(-1f, 1f, y1 / 19f), math.lerp(-1f, 1f, x2 / 2f), Mathf.Lerp(-1f, 1f, y2 / 1f) });
                    }
                }
            }
        }
    }

    void GetOutputs() {
        //Layer One
        float[,] layerOne = new float[10, 20];
        int x2 = engine.currentTetrominoPos[0]; int y2 = engine.currentTetrominoPos[1];
        for (int x1 = 0; x1 < 4; x1++) {
            for (int y1 = 0; y1 < 4; y1++) {
                try {
                    layerOne[x2, y2] += weights[y2 * 160 + x2 * 16 + y1 * 4 + x1] * (engine.currentTetrominoState[x1, y1] / 16f);
                } catch {
                    continue;
                }
            }
        }

        //Layer Two
        float[,] layerTwo = new float[10, 20];
        for (int x1 = 0; x1 < 10; x1++) {
            for (int y1 = 0; y1 < 20; y1++) {
                try {
                    layerTwo[x1, x1] += weights[3200 + y1 * 2000 + x1 * 200 + y2 * 10 + x2] * ((layerOne[x2, y2] * engine.field[x1][y1]) / 200f);
                } catch {
                    continue;
                }
                
                //Debug.Log(layerTwo[x1, y1]);
            }
        }

        //Layer Three
        float[,] layerThree = new float[3, 2];
        for (int x1 = 0; x1 < 10; x1++) {
            for (int y1 = 0; y1 < 20; y1++) {
                for (x2 = 0; x2 < 3; x2++) {
                    for (y2 = 0; y2 < 2; y2++) {
                        layerThree[x2, y2] += weights[43200 + y2 * 300 + x2 * 100 + y1 * 10 + x1] * (layerTwo[x1, y1] / 200f);
                    }
                }
            }
        }

        /*        // add field
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
                inputField.Add(1f);*/

        float[] outputs = new float[] { layerThree[1, 0], layerThree[0, 0], layerThree[1, 1], layerThree[2, 0], layerThree[0, 1], layerThree[2, 1] };
        if (rendered) {
            uiEngine.UpdateControllerActivations(outputs);
        }
        DoMove(outputs);

    }
}
