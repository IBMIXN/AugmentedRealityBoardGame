using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBeam : MonoBehaviour
{
    public void drawBetweeen(Vector3 startPos, Vector3 endpos)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        // Move to the halfway point between the two positions
        transform.position = (startPos + endpos) / 2;
        // Rotate to face between the two positions
        transform.eulerAngles = new Vector3(0, 0, 90f + Mathf.Rad2Deg * Mathf.Atan2(startPos.y - endpos.y, startPos.x - endpos.x));
        // Be long enough that the circles at the ends of the beam line up with the targets
        float length = (new Vector2(
                (startPos.x - endpos.x) / transform.lossyScale.x,
                (startPos.y - endpos.y) / transform.lossyScale.y)
            ).magnitude;
        // Not that the width is also added to the length to account for the diameter of the circles at the ends of the beams
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, length + rectTransform.sizeDelta.x);
    }
}
