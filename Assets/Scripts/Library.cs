using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library : MonoBehaviour {
    public float radius = 10;
    public float bottomOffset;
    public Vector2 bannerSize;
    public GameObject bannerPrefab;

	void Start () {
        int columns = 25;
        int lines = 4;

		for (int i = 0; i < columns; i++) {
            float angle = i / (float)columns * 2.0f * Mathf.PI;

            for (int o = 0; o < lines; o++) {
                Vector3 position = new Vector3(
                    Mathf.Sin(angle) * radius,
                    o * (bannerSize.y * 1.25f) + bottomOffset,
                    Mathf.Cos(angle) * radius
                );

                GameObject banner = (GameObject)Instantiate(bannerPrefab);
                banner.transform.position = position;
                banner.transform.parent = this.transform;
                banner.transform.Rotate(new Vector3(0, Mathf.Rad2Deg * angle, 0));
            }
        }
	}
	
	void Update () {
		
	}
}
