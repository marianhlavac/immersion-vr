using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFadeInFadeOut : MonoBehaviour {
    public float offset;
    public float transitionLength;
    public float appearanceLength;
    public float defaultZ;
    public float ZMoveOffset = 10;

    private float smoothness = 0.1f;
	
	void Update () {
        TextMesh textMesh = GetComponent<TextMesh>();

        float alpha = 0;

        if (Time.time >= offset + transitionLength + appearanceLength * 2) {
            alpha = 0;
            return;
        }
        else if (Time.time >= offset + transitionLength + appearanceLength) {
            float velocity = 0;
            alpha = 1 - Mathf.SmoothDamp(
                Time.time - offset - transitionLength - appearanceLength,
                transitionLength, ref velocity, smoothness) / transitionLength;
        }
        else if(Time.time >= offset + transitionLength) {
            alpha = 1;
        } else if (Time.time >= offset) {
            float velocity = 0;
            alpha = Mathf.SmoothDamp(Time.time - offset, transitionLength, ref velocity, smoothness) / transitionLength;
        }

        textMesh.color = new Color(
            textMesh.color.r,
            textMesh.color.g,
            textMesh.color.b,
            alpha);

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            defaultZ + ZMoveOffset * (-1 + alpha));
	}
}
 