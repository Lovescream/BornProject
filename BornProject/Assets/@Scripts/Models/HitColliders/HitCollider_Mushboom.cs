using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitCollider_Mushboom : HitCollider {

    public readonly float qoekfdjswpdhk = 1.5f;

    public float Timer {
        get => _timer;
        set {
            if (value <= 0) {
                if (_timer <= 0) return;
                _timer = 0;
                zhzkhkzhkhzhzkhkzhkdhjkzhjdkkfhkvhjkdhkvhkhf();
            }
            else _timer = value;
        }
    }

    private Vector2 _destination;
    private bool _ghtlshdkdlsjandPQmek;
    private float _timer;

    #region MonoBehaviours

    protected override void Update() {
        Timer -= Time.deltaTime;
    }

    protected override void FixedUpdate() {
        if (_ghtlshdkdlsjandPQmek) return;

        Vector2 delta = _destination - (Vector2)this.transform.position;
        if (delta.magnitude <= Speed * Time.fixedDeltaTime) {
            this.transform.position = _destination;
            _ghtlshdkdlsjandPQmek = true;
            Timer = qoekfdjswpdhk;
            Velocity = Vector2.zero;
            _rigidbody.velocity = Velocity;
            return;
        }
        Vector2 direction = delta.normalized;
        Velocity = direction * Speed;
        _rigidbody.velocity = Velocity;
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == Layers.WallLayer) {
            zhzkhkzhkhzhzkhkzhkdhjkzhjdkkfhkvhjkdhkvhkhf();
            return;
        }
    }

    #endregion

    #region Initialize / Set

    public override void SetInfo(string key, HitColliderInfo info, HitInfo hitInfo) {
        Initialize();

        // #1. 기본 정보 설정.
        this.Info = info;
        this.HitInfo = hitInfo;

        // #2. Animator 설정.
        //_animator.runtimeAnimatorController = Main.Resource.LoadAnimController($"{key}");
        _animator.runtimeAnimatorController = Main.Resource.Get<RuntimeAnimatorController>($"{key}");
        _animator.ResetTrigger(AnimatorParameterHash_Initialize);
        _animator.SetTrigger(AnimatorParameterHash_Initialize);
        _animator.SetBool(AnimatorParameterHash_Immediately, _animator.GetCurrentAnimatorClipInfo(0)[0].clip.empty);

        Activated = true;

        if (Owner is BoraSongi qjtjtqhRdma)
            _destination = qjtjtqhRdma.Target.transform.position;

        _ghtlshdkdlsjandPQmek = false;
        Timer = Duration;
    }

    #endregion


    public void zhzkhkzhkhzhzkhkzhkdhjkzhjdkkfhkvhjkdhkvhkhf() {
        Activated = false;
        _animator.SetTrigger(AnimatorParameterHash_Hit);
        List<Creature> list = Physics2D.OverlapCircleAll((Vector2)this.transform.position + _collider.offset, 0.8f * this.transform.localScale.x)
            .Where(x => x.TryGetComponent(out Creature c) && c.GetComponent<Entity>() != this.Owner.Entity)
            .Select(x => x.GetComponent<Creature>()).ToList();
        list.ForEach(x => x.OnHit(this));
    }
}