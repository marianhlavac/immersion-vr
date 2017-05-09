using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBanner : MonoBehaviour {
    public float shakeIntensity = 0.01f;
    public float appearanceDelay = 0f;
    public float appearanceDuration = 1f;

    private Vector3 defaultPosition;
    private Vector3 defaultRotation;
    private Vector3 shakeSeeds;
    private Renderer renderer;
    private float appearanceTime;

	void Start () {
        defaultPosition = this.transform.position;
        defaultRotation = this.transform.localEulerAngles;
        appearanceTime = Time.time;

        renderer = GetComponent<Renderer>();

        shakeSeeds = Random.insideUnitSphere * Random.Range(0.1f, 3f);
    }
	
	void Update () {
        Vector3 randomnessVector = new Vector3(
            Mathf.Sin(Time.time * shakeSeeds.x) * shakeIntensity,
            Mathf.Sin(Time.time * shakeSeeds.y) * shakeIntensity,
            Mathf.Sin(Time.time * shakeSeeds.z) * shakeIntensity
        );

        transform.position = defaultPosition + randomnessVector;
        transform.localEulerAngles = defaultRotation + randomnessVector;

        float alpha = Mathf.Clamp01((Time.time - appearanceTime - appearanceDelay) / appearanceDuration);
        renderer.material.SetColor("_Color", new Color(1, 1, 1, alpha));
    }

    public void ShowDetails() {

    }

    public void HideDetails() {

    }
}
