using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControllerInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Vector3.zero, new Vector3(0, 1, 0), Input.GetAxis("Mouse X") * 10);
        transform.RotateAround(Vector3.zero, new Vector3(1, 0, 0), Input.GetAxis("Mouse Y") * 10);
    }
}
