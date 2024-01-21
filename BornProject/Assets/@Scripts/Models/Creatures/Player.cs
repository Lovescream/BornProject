using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Creature
{
    #region Properties
    public new CreatureData Data => base.Data;

    #endregion

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        CameraController camera = FindObjectOfType<CameraController>();
        if (camera != null) camera.SetTarget(this.transform);

        return true;
    }

    #region Input

    protected void OnMove(InputValue value)
    {
        Velocity = value.Get<Vector2>().normalized * Status[StatType.MoveSpeed].Value;
    }
    protected void OnLook(InputValue value)
    {
        LookDirection = (Camera.main.ScreenToWorldPoint(value.Get<Vector2>()) - this.transform.position).normalized;
    }
    protected void OnFire()
    {
        // TODO.
    }

    #endregion

}