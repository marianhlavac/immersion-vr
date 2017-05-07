using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    LineRenderer line;
	public GameObject beamSource;

	private bool isBeamed = false;
	private Renderer renderer;

	// Use this for initialization
	void Start () {
        line = GetComponent<LineRenderer>();
		renderer = GetComponent<Renderer> ();

		SteamVR_TrackedController controller = beamSource.GetComponent<SteamVR_TrackedController>();

		controller.TriggerClicked += BeamUp;
		controller.TriggerUnclicked += BeamDown;

		isBeamed = false;
		line.material.SetColor(0, new Color (0.9f, 0.1f, 0.1f, 0.2f));
	}

    void FixedUpdate() {
        RaycastHit hit;

		if (Physics.Raycast(beamSource.transform.position, beamSource.transform.forward, out hit, 500.0f)) {
            line.SetPositions(new Vector3[] {
				beamSource.transform.position,
                hit.point,
            });	
        } else {
            line.SetPositions(new Vector3[] {
				beamSource.transform.position,
				beamSource.transform.forward * 15,
            });
        }
    }

	private void BeamUp(object sender, ClickedEventArgs e) {
		isBeamed = true;
		line.material.SetColor(0, new Color (1f, 0.1f, 0.1f, 1f));

	}

	void BeamDown(object sender, ClickedEventArgs e) {
		isBeamed = false;
		line.material.SetColor(0, new Color (0.9f, 0.1f, 0.1f, 0.2f));
	}
}
