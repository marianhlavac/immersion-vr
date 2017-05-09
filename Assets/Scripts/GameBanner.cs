using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBanner : MonoBehaviour {
    public float shakeIntensity = 0.01f;
    public float appearanceDelay = 0f;
    public float appearanceDuration = 1f;
    public GameObject detailRig;
    public GameObject library;
    public GameObject playButton;
    public GameObject closeButton;

    public GameItem game;

    private Vector3 defaultPosition;
    private Vector3 defaultRotation;
    private Vector3 shakeSeeds;
    private Renderer renderer;
    private float appearanceTime;
    private LaserPointer laserPointer;
    private Vector3 positionOffset;
    private Vector3 positionOffsetTarget;
    private Vector3 detailScale = Vector3.zero;
    private Vector3 detailScaleTarget = Vector3.zero;

    void Start () {
        defaultPosition = this.transform.localPosition;
        defaultRotation = this.transform.localEulerAngles;
        appearanceTime = Time.time;

        renderer = GetComponent<Renderer>();
        laserPointer = GameObject.FindGameObjectWithTag("LaserPointer").GetComponent<LaserPointer>();

        shakeSeeds = Random.insideUnitSphere * Random.Range(0.1f, 3f);
    }
	
	void Update () {
        Vector3 randomnessVector = new Vector3(
            Mathf.Sin(Time.time * shakeSeeds.x) * shakeIntensity,
            Mathf.Sin(Time.time * shakeSeeds.y) * shakeIntensity,
            Mathf.Sin(Time.time * shakeSeeds.z) * shakeIntensity
        );

        transform.localPosition = defaultPosition + randomnessVector + positionOffset;
        transform.localEulerAngles = defaultRotation + randomnessVector;

        float alpha = Mathf.Clamp01((Time.time - appearanceTime - appearanceDelay) / appearanceDuration);
        renderer.material.SetColor("_Color", new Color(1, 1, 1, alpha));

        if (laserPointer.pointingAt == gameObject) {
            OnHover();

            if (laserPointer.isBeaming) {
                ShowDetail();
            }
        } else {
            OnNoHover();
        }

        if (laserPointer.pointingAt == closeButton && laserPointer.isBeaming) {
            HideDetail();
        }

        if (laserPointer.pointingAt == playButton && laserPointer.isBeaming) {
            HideDetail();
            game.RunGame();
        }

        positionOffset += (positionOffsetTarget - positionOffset) / 4.0f;
        detailScale += (detailScaleTarget - detailScale) / 4.0f;

        detailRig.transform.localScale = detailScale;
    }

    private void OnHover() {
        positionOffsetTarget = transform.forward * -0.5f;
    }

    private void OnNoHover() {
        positionOffsetTarget = Vector2.zero;
    }

    public void ShowDetail() {
        Library lib = library.GetComponent<Library>();
        lib.HideAllDetails();
        detailScaleTarget = Vector3.one;
    }

    public void HideDetail() {
        detailScaleTarget = Vector3.zero;
    }
}
