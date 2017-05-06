using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBanner : MonoBehaviour {
    public float shakeIntensity = 0.01f;

    private Vector3 defaultPosition;
    private Vector3 defaultRotation;
    private Vector3 shakeSeeds;

	void Start () {
        defaultPosition = this.transform.position;
        defaultRotation = this.transform.localEulerAngles;

        shakeSeeds = Random.insideUnitSphere * Random.Range(0.1f, 3f);
    }
	
	void Update () {
        Vector3 randomnessVector = new Vector3(
            Mathf.Sin(Time.time * shakeSeeds.x) * shakeIntensity,
            Mathf.Sin(Time.time * shakeSeeds.y) * shakeIntensity,
            Mathf.Sin(Time.time * shakeSeeds.z) * shakeIntensity
        );

        this.transform.position = defaultPosition + randomnessVector;
        this.transform.localEulerAngles = defaultRotation + randomnessVector;

    }

    public void ShowDetails() {

    }

    public void HideDetails() {

    }
}
