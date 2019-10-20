using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    MeshRenderer appearance;
    TetrisEngine engine;

    private void Start()
    {
        appearance = GetComponent<MeshRenderer>();
        engine = GetComponentInParent<TetrisEngine>();

        engine.updateField += UpdateAppearance;
    }
    private void Update()
    {
        //UpdateAppearance();
    }

    void UpdateAppearance()
    {
        appearance.enabled = (engine.viewField[Mathf.RoundToInt(transform.position.x)][Mathf.RoundToInt(transform.position.y)] == 1);
    }
}
