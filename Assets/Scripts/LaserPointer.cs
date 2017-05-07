using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    LineRenderer line;

	// Use this for initialization
	void Start () {
        line = GetComponent<LineRenderer>();
	}

    void FixedUpdate() {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.up, out hit, 500.0f)) {
            line.SetPositions(new Vector3[] {
                new Vector3(0, 0, 0),
                hit.point,
            });
        } else {
            line.SetPositions(new Vector3[] {
                new Vector3(0, 0, 0),
                transform.up * 10,
            });
        }
    }
}
