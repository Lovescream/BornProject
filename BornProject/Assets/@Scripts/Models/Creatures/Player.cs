using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Creature
{
    #region Properties
    public new CreatureData Data => base.Data;

    #endregion

    #region Input

    protected void OnMove(InputValue value)
    {
        Velocity = value.Get<Vector2>().normalized * Status[StatType.MoveSpeed].Value;
    }
    protected void OnLook(InputValue value)
    {
        LookDirection = (Camera.main.ScreenToWorldPoint(value.Get<Vector2>()) - this.transform.position).normalized;
        //AimDirection();
    }
    protected void OnFire()
    {
        // TODO.
    }


    protected override void SetStatus(bool isFullHp = true)
    {
        this.Status = new(Data);
        if (isFullHp)
        {
            Hp = Status[StatType.HpMax].Value;
        }

    }
    //private void AimDirection()
    //{
    //    float rotZ = Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg;
    //    _armPivot.rotation = Quaternion.Euler(0, 0, rotZ);
    //    _weaponSprite.flipY = (Mathf.Abs(rotZ) > 90) ? true : false;
    //    _weaponAnimation.flipY = (Mathf.Abs(rotZ) > 90) ? true : false;
    //}

    #endregion

}