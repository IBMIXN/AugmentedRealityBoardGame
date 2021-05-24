using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScrollingBGScript : MonoBehaviour
{
    private float x_speed;
    private float y_speed;

    public MeshRenderer renderer;
    public float x_rebound = 0.25f;
    public float y_rebound = 0.25f;

    void Start()
    {
        // Syncs up with music length
        x_speed = (x_rebound / 13) * 2;
        y_speed = 0;
    }

    void Update() {
        if (Mathf.Abs(this.renderer.material.mainTextureOffset.x) > x_rebound)
        {
            x_speed *= -1;
        }

        if (Mathf.Abs(this.renderer.material.mainTextureOffset.y) > y_rebound)
        {
            y_speed *= -1;
        }

        this.renderer.material.mainTextureOffset += new Vector2(Time.deltaTime * x_speed, Time.deltaTime * y_speed);
    }
}
