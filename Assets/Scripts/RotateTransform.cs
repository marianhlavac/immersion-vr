using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTransform : MonoBehaviour {
    public Vector3 speed = new Vector3(0, 1, 0);

	void Update () {
        transform.Rotate(speed * Time.deltaTime);
	}
}
