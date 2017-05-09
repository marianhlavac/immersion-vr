using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInFadeOut : MonoBehaviour {
    public float timeTransition;
    public float moveOffset = 5;
    public float timeOffset = 0;
    public bool isTextTransition = false;
    public bool selfDestructAfterEnd = true;
    public bool autoStart = false;

    private float smoothness = 0.1f;
    private float defaultZ = 0;
    private bool appearing = true;
    private float started = 0;
    private bool isActive = false;

    void Start() {
        defaultZ = transform.position.z;

        if (isTextTransition) {
            ApplyAlphaOnTextMesh(0);
        }
        else {
            ApplyAlphaOnMaterial(0);
        }

        if (autoStart) {
            Show();
        }
    }

    public void Show() {
        appearing = true;
        started = Time.time - timeOffset;
        isActive = true;
    }

    public void Hide() {
        appearing = false;
        isActive = true;
        started = Time.time - timeOffset;
    }
	
	void Update () {
        if (isActive) {
            float alpha = CalculateAlpha(started, timeTransition, Time.time, appearing);

            if (isTextTransition) {
                ApplyAlphaOnTextMesh(alpha);
            }
            else {
                ApplyAlphaOnMaterial(alpha);
            }

            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                defaultZ + moveOffset * (-1 + alpha));

            if (selfDestructAfterEnd && !appearing && alpha <= 0) {
                Destroy(gameObject);
            }

            if (alpha >= 1 && appearing) {
                isActive = false;
            }
        }
    }

    float CalculateAlpha(float begin, float duration, float current, bool goingUp) {
        float velocity = 0;
        float progress = Mathf.SmoothDamp(current - begin, duration, ref velocity, smoothness) / duration;

        progress = Mathf.Clamp(progress, 0, 1);
        return goingUp ? progress : 1 - progress;
    }

    void ApplyAlphaOnTextMesh(float alpha) {
        TextMesh textMesh = GetComponent<TextMesh>();
        textMesh.color = new Color(
            textMesh.color.r,
            textMesh.color.g,
            textMesh.color.b,
            alpha);
    }

    void ApplyAlphaOnMaterial(float alpha) {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = new Color(
            renderer.material.color.r,
            renderer.material.color.g,
            renderer.material.color.b,
            alpha);
    }
}
 