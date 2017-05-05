using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInFadeOut : MonoBehaviour {
    public float timeOffset;
    public float timeTransition;
    public float timeAppearance;
    public float moveOffset = 5;
    public bool isTextTransition = false;
    public bool selfDestructAfterEnd = true;

    private float smoothness = 0.1f;
    private float defaultZ = 0;

    void Start() {
        defaultZ = transform.position.z;
    }
	
	void Update () {

        float alpha = 0;

        if (Time.time >= timeOffset + timeTransition + timeAppearance * 2) {
            alpha = 0;
            Destroy(gameObject);
            Destroy(this);
            return;
        }
        else if (Time.time >= timeOffset + timeTransition + timeAppearance) {
            float velocity = 0;
            alpha = 1 - Mathf.SmoothDamp(
                Time.time - timeOffset - timeTransition - timeAppearance,
                timeTransition, ref velocity, smoothness) / timeTransition;
        }
        else if(Time.time >= timeOffset + timeTransition) {
            alpha = 1;
        } else if (Time.time >= timeOffset) {
            float velocity = 0;
            alpha = Mathf.SmoothDamp(Time.time - timeOffset, timeTransition, ref velocity, smoothness) / timeTransition;
        }

        if (isTextTransition) {
            TextMesh textMesh = GetComponent<TextMesh>();
            textMesh.color = new Color(
                textMesh.color.r,
                textMesh.color.g,
                textMesh.color.b,
                alpha);
        } else {
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.color = new Color(
                renderer.material.color.r,
                renderer.material.color.g,
                renderer.material.color.b,
                alpha);
        }

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            defaultZ + moveOffset * (-1 + alpha));
	}
}
 