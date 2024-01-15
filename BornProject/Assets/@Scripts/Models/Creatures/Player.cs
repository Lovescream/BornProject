using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Creature {

    // Temp;
    private float moveSpeed = 3;

    protected void OnMove(InputValue value) {
        Velocity = value.Get<Vector2>().normalized * moveSpeed;
    }

    protected void OnLook(InputValue value) {
        LookDirection = (Camera.main.ScreenToWorldPoint(value.Get<Vector2>()) - this.transform.position).normalized;
    }

}