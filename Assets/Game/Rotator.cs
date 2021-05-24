using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 degreesPerSecond;

    void FixedUpdate()
    {
        transform.Rotate(degreesPerSecond * Time.deltaTime);
    }
}
