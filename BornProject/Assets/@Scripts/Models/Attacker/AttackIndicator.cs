using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIndicator : MonoBehaviour {

    #region Properties

    public IAttackable Owner { get; private set; }
    public Transform Point {
        get {
            if (_point == null) Initialize();
            return _point;
        }
        set => _point = value;
    }
    public bool OriginDirection => Owner.Entity.Flip == false;
    public Vector2 MousePoint { get; set; }
    public float ShotAngle {
        get {
            Vector2 direction = ShotDirection;
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
    }
    public Vector2 ShotDirection {
        get {
            Vector2 delta = Point.position - this.transform.position;
            if (delta.sqrMagnitude <= float.Epsilon)
                delta = Point.position - Owner.Entity.transform.position;
            return delta.normalized;
        }
    }
    public Vector2 IndicatorDirection {
        get => _angle;
        set {
            _angle = value;
            float angle = Mathf.Atan2(value.y, OriginDirection ? value.x : -value.x) * Mathf.Rad2Deg;
            this.transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    #endregion

    #region Fields

    private Vector2 _angle;
    private Transform _point;
    private bool _isInitialized = false;

    #endregion

    void Awake() {
        Initialize();
    }

    private void Initialize() {
        if (_isInitialized) return;
        _isInitialized = true;
        Point = this.transform.Find("ShotPoint");
    }

    public void SetInfo(IAttackable owner) {
        Initialize();
        Owner = owner;
    }
}