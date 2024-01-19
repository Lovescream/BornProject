using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Thing
{
    public Creature Owner { get; protected set; } // 투사체 주인.
    public float Duration { get; protected set; } // 투사체 지속시간.
    public float Damage { get; protected set; } // 투사체 데미지.
    public Vector2 Velocity { get; set; } // 투사체 속도.

    protected Rigidbody2D _rigidbody; // 충돌처리를 위한 Rigidbody.
    private Coroutine _coDestroy; // 

    // Rigidbody가 붙은 투사체에 대한 속도.
    protected virtual void FixedUpdate()
    {
        _rigidbody.velocity = Velocity;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Creature creature = collision.GetComponent<Creature>();
        if (!creature.IsValid() || !this.IsValid()) return;
        if (creature == Owner) return;
        if (creature.State.Current == CreatureState.Dead) return;

        creature.OnHit(Owner, Damage, new() { time = 0.1f, speed = 10f, direction = (creature.transform.position - this.transform.position).normalized });

        _rigidbody.velocity = Velocity = Vector2.zero;
        if (this.IsValid()) Main.Object.DespawnProjectile(this);
    }

    protected void OnDisable()
    {
        StopAllCoroutines(); // 모든 코루틴 중단.
        _coDestroy = null;
    }

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _rigidbody = this.GetComponent<Rigidbody2D>();

        return true;
    }

    public virtual Projectile SetInfo(Creature owner) // Creature에 대한 투사체 정보.
    {
        this.Owner = owner;
        Duration = 5; // TODO::
        Damage = 10; // TODO::

        if (_coDestroy != null) StopCoroutine(this._coDestroy); // 코루틴 중복 방지.
        _coDestroy = StartCoroutine(CoDestroy());

        return this;
    }

    private IEnumerator CoDestroy()
    {
        yield return new WaitForSeconds(Duration);
        _coDestroy = null;
        // Main.Object.DespawnProjectile(this); // 오브젝트가 비활성화 시. TODO:
    }
}