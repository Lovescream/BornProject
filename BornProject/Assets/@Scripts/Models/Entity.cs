using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    #region For Develop
    [SerializeField]
    private bool _autoInitialize = true;
    [SerializeField]
    [ConditionalInspector("_autoInitialize", false)]
    private string _key = "";

    public string ManualInitializingKey => _key;

    #endregion

    #region Properties

    public virtual float ColliderRatio => 0.8f;

    #endregion

    #region Fields

    private bool _initialized;

    // Components.
    protected SpriteRenderer _spriter;
    protected Collider2D _collider;
    protected Animator _animator;

    #endregion

    #region MonoBehaviours

    protected virtual void Awake() {
        if (_autoInitialize) Initialize();
    }

    #endregion

    public virtual bool Initialize() {
        if (_initialized) return false;

        _spriter = this.GetComponent<SpriteRenderer>();
        _collider = this.GetComponent<Collider2D>();
        _animator = this.GetComponent<Animator>();

        _spriter.RegisterSpriteChangeCallback(FitColliderOnChangedSprite);

        _initialized = true;
        return true;
    }

    private void FitColliderOnChangedSprite(SpriteRenderer spriter) {
        if (spriter.sprite == null) return;
        if (_collider is not BoxCollider2D box) return;
        box.size = spriter.sprite.bounds.size * ColliderRatio;
        box.offset = (Vector2)(spriter.bounds.center - this.transform.position);
    }
}