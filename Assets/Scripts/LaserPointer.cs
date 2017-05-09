using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    LineRenderer line;
	public GameObject beamSource;
    public Color beamColor = new Color(1, 0, 0);
    public GameObject pointingAt;

	public bool isBeaming = false;
	private Renderer renderer;
    private AudioSource sound;

	// Use this for initialization
	void Start () {
        line = GetComponent<LineRenderer>();
		renderer = GetComponent<Renderer> ();
        sound = GetComponent<AudioSource>();

		SteamVR_TrackedController controller = beamSource.GetComponent<SteamVR_TrackedController>();

		controller.TriggerClicked += BeamUp;
		controller.TriggerUnclicked += BeamDown;
	}

    void FixedUpdate() {
        RaycastHit hit;

		if (Physics.Raycast(beamSource.transform.position, beamSource.transform.forward, out hit, 500.0f)) {
            line.SetPositions(new Vector3[] {
				beamSource.transform.position,
                hit.point,
            });
            pointingAt = hit.collider.gameObject;
        } else {
            line.SetPositions(new Vector3[] {
				beamSource.transform.position,
				beamSource.transform.forward * 50,
            });
            pointingAt = null;
        }
    }

    private void Update() {
        float alpha = isBeaming ? Mathf.Sin(Time.time * 2f) * 0.1f + 0.9f : 0;
        float size = isBeaming ? Mathf.Sin(Time.time * 75f) * 0.005f + 0.02f : Mathf.Abs(Mathf.Sin(Time.time * 2f) * 0.0005f + 0.005f);
        Color color = CalculateColor((Input.mousePosition.x - 258f) / 426f, (Input.mousePosition.y - 118f) / 238f, alpha);

        line.material.SetColor("_Color", color);
        line.material.SetColor("_EmissionColor", color);
        line.SetWidth(size, size * 0.85f);
    }

    private Color CalculateColor(float x, float y, float alpha) {
        float angle = Mathf.Atan2(y, x);
        float hue = Mathf.Abs(angle / Mathf.PI);

        float intensity = 1 - Mathf.Clamp01(new Vector2(x, y).magnitude);

        float r = Mathf.Clamp01(1 - 2 * hue + intensity);
        float g = Mathf.Clamp01(-Mathf.Abs(2 * hue - 1) + 1 + intensity);
        float b = Mathf.Clamp01(2 * hue - 1 + intensity);

        return new Color(r, g, b, alpha);
    }

	private void BeamUp(object sender, ClickedEventArgs e) {
		isBeaming = true;
        sound.Play();
    }

	void BeamDown(object sender, ClickedEventArgs e) {
		isBeaming = false;
        sound.Stop();
    }
}
