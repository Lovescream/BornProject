using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Thing
{
    public Creature Owner { get; protected set; } // 발사체 주인.

    // public new Creature.Status; // Creature의 Status에서 지속시간과 데미지 가져오는 방법 TODO.
    public float Duration { get; protected set; } // 발사체 지속시간.
    public float Damage { get; protected set; } // 발사체 데미지.
    public Vector2 Velocity { get; set; } // 발사체 속도.

    protected Rigidbody2D _rigidbody; // 충돌처리를 위한 Rigidbody.
    private Coroutine _coDestroy; // 코루틴이 null인지 활성화 되어있는지 확인하기 위한 필드.

    // Rigidbody가 붙은 발사체에 대한 속도.
    protected virtual void FixedUpdate()
    {
        _rigidbody.velocity = Velocity;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Creature creature = collision.GetComponent<Creature>();
        // if (!creature.IsValid() || !this.IsValid()) return; // TODO:
        if (creature == Owner) return; // Creature가 발사체의 주인이라면.
        // if (creature.State.Current == CreatureState.Dead) return; // Creature의 현재 상태가 죽었다면.

        // creature.OnHit(Owner, Damage, new() { time = 0.1f, speed = 10f, direction = (creature.transform.position - this.transform.position).normalized }); // Creature 공격.

        _rigidbody.velocity = Velocity = Vector2.zero;
        // if (this.IsValid()) Main.Object.DespawnProjectile(this); // IsValid가 뭔지 확인.
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

    public virtual Projectile SetInfo(Creature owner) // Creature에 대한 발사체 정보.
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
        yield return new WaitForSeconds(Duration); // 발사체 지속시간만큼 대기.
        _coDestroy = null;
        // Main.Object.DespawnProjectile(this); // 오브젝트가 비활성화 시.
    }
}