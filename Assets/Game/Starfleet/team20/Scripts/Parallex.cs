using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallex : MonoBehaviour
{
    void Update()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material matMain = meshRenderer.material;
        Vector2 offset = matMain.mainTextureOffset;
        offset.x += Time.deltaTime / 12f;
        matMain.mainTextureOffset = offset;
    }
}
