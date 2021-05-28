using System.Linq;
using UnityEngine;
using System;
using Unity.Mathematics;

public class TetrisAgent : MonoBehaviour
{
    public event Action<int, GameObject, int, Genome> died;
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
        weights = new float[1200];
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
        died?.Invoke(engine.score.score, gameObject, engine.dropCounter, neuralNet.genome);
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

        //Loop board
        for (int x1 = 0; x1 < 10; x1++) {
            for (int y1 = 0; y1 < 20; y1++) {
                //Loop controller
                for (int x2 = 0; x2 < 3; x2++) {
                    for (int y2 = 0; y2 < 2; y2++) {
                        weights[y2 * 600 + x2 * 200 + y1 * 10 + x1] = neuralNet.GetNNResult(new float[] { math.lerp(-1f, 1f, x1 / 9f), math.lerp(-1f, 1f, y1 / 19f), math.lerp(-1f, 1f, x2 / 2f), Mathf.Lerp(-1f, 1f, y2 / 1f) });
                    }
                }
            }
        }
    }

    void GetOutputs() {

        float[,] layerThree = new float[3, 2];
        for (int x1 = 0; x1 < 10; x1++) {
            for (int y1 = 0; y1 < 20; y1++) {
                for (int x2 = 0; x2 < 3; x2++) {
                    for (int y2 = 0; y2 < 2; y2++) {
                        layerThree[x2, y2] += weights[y2 * 600 + x2 * 200 + y1 * 10 + x1] * (engine.viewField[x1][y1] / 200f);
                    }
                }
            }
        }

        float[] outputs = new float[] { layerThree[1, 0], layerThree[0, 0], layerThree[1, 1], layerThree[2, 0], layerThree[0, 1], layerThree[2, 1] };
        if (rendered) {
            uiEngine.UpdateControllerActivations(outputs);
        }
        DoMove(outputs);

    }
}
