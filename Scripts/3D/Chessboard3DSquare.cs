using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard3DSquare : MonoBehaviour
{
    public Vector2Int position;
    AppearanceManager3D appearanceManager3D;
    Vector3 base_position;
    MeshRenderer meshRenderer;

    int highlighted = 0;

    Material current_material;
    Material current_highlight_material;
    Material current_kill_material;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        base_position = transform.position;
        appearanceManager3D = FindObjectOfType<AppearanceManager3D>();
        appearanceManager3D.Board[position.x, position.y] = this;
    }

    public void SetMaterial(Material material, Material highlighted_material, Material kill_material)
    {
        current_material = material;
        current_highlight_material = highlighted_material;
        current_kill_material = kill_material;

        if (appearanceManager3D.highlighted[position.x, position.y] == 1)
        {
            meshRenderer.material = highlighted_material;
        }
        else if (appearanceManager3D.highlighted[position.x, position.y] == 2)
        {
            meshRenderer.material = kill_material;
        }
        else
        {
            meshRenderer.material = material;
        }
    }

    void LateUpdate()
    {
        float target_displacement = appearanceManager3D.target_displacement[position.x, position.y];
        float target_delta = target_displacement + 1 - transform.position.y;
        if (target_delta == 0) { return; }
        transform.position += Vector3.up * Mathf.Log(target_delta, 2) * Time.deltaTime * 10;

        if (appearanceManager3D.highlighted[position.x, position.y] != highlighted)
        {
            if (appearanceManager3D.highlighted[position.x, position.y] == 1)
            {
                meshRenderer.material = current_highlight_material;
            }
            else if (appearanceManager3D.highlighted[position.x, position.y] == 2)
            {
                meshRenderer.material = current_kill_material;
            }
            else
            {
                meshRenderer.material = current_material;
            }
            highlighted = appearanceManager3D.highlighted[position.x, position.y];
        }
    }
}
