using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library : MonoBehaviour {
    public float radius = 10;
    public float bottomOffset;
    public float horizontalOffset;
    public Vector2 bannerSize;
    public GameObject bannerPrefab;

	void Start () {
        DrawGrid(10, 4, -1, 12);
        DrawGrid(2, 4, 1, 28);
    }

    void DrawGrid (int columns, int lines, int direction, int angleOffset) {
        float circleLength = radius * Mathf.PI;
        float warpBannerWidth = (circleLength / 2) * (bannerSize.x) / circleLength;
        float stepAngle = (warpBannerWidth + horizontalOffset) / circleLength * 2.0f * Mathf.PI;
        float insetRadius = Mathf.Cos(Mathf.Asin((bannerSize.x / 2) / radius)) * radius;

        for (int i = 0; i < columns; i++) {
            float angle = i * stepAngle * direction + Mathf.Deg2Rad * angleOffset;

            for (int o = 0; o < lines; o++) {
                Vector3 position = new Vector3(
                    Mathf.Sin(angle) * insetRadius,
                    o * (bannerSize.y * 1.25f) + bottomOffset,
                    Mathf.Cos(angle) * insetRadius
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
