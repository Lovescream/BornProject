using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MonoBehaviour {

    private Vector3 offset = new(0.5f, 0);

    private LaserEx laser;

    void Awake() {
        laser = this.transform.GetComponentInChildren<LaserEx>();
    }

    void Update() {
        float maxLaserSize = 20;

        RaycastHit2D hit = Physics2D.Raycast(this.transform.position + offset, this.transform.right, maxLaserSize, 1 << 5);
        float laserLength = maxLaserSize;
        if (hit) {
            laserLength = (hit.point - (Vector2)(this.transform.position + offset)).magnitude;
        }
        laser.length = laserLength;
    }

}