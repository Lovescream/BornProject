using System;
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
    public float SpriteWidthRatio => _spriter == null ? 0 : (_spriter.sprite.textureRect.width / _spriter.sprite.pixelsPerUnit);
    public bool Flip {
        get => _flip;
        set {
            _flip = value;
            if (_spriter != null) _spriter.flipX = value;
            if (_horizontalAxis != null) _horizontalAxis.localScale = new(value ? -1 : 1, 1, 1);
        }
    }

    #endregion

    #region Fields

    private bool _flip;
    private bool _initialized;

    // Components.
    protected Transform _horizontalAxis;
    protected SpriteRenderer _spriter;
    protected Collider2D _collider;
    protected Animator _animator;

    public event Action OnDisabled;

    #endregion

    #region MonoBehaviours

    protected virtual void Awake() {
        if (_autoInitialize) Initialize();
    }
    protected virtual void OnDisble() {
        OnDisabled?.Invoke();
    }

    #endregion

    public virtual bool Initialize() {
        if (_initialized) return false;

        OnDisabled = null;

        _horizontalAxis = this.transform.Find("HorizontalAxis");
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
        Bounds b = spriter.sprite.bounds;
        box.size = b.size * ColliderRatio;
        box.offset = new((Flip ? -1 : 1) * b.center.x, b.center.y);
    }
}