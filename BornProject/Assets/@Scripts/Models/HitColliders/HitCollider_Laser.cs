using System.Linq;
using UnityEngine;

public class HitCollider_Laser : HitCollider {

    #region Fields

    protected LayerMask _layerMask;

    #endregion

    #region MonoBehaviours

    protected override void Update() {
        base.Update();

        SetSpriteWidth();
    }

    protected override void FixedUpdate() {
        
    }

    #endregion

    #region Initialize / Set

    public override void SetInfo(string key, HitColliderInfo info, HitInfo hitInfo) {
        base.SetInfo(key, info, hitInfo);

        _spriter.drawMode = SpriteDrawMode.Sliced;
        if (_collider is BoxCollider2D box) box.autoTiling = true;
        _spriter.RegisterSpriteChangeCallback(SetSpriteHeight);

        _layerMask = 1 << Main.CreatureLayer | 1 << Main.WallLayer;
    }

    #endregion

    private float GetLaserLength() => Physics2D.RaycastAll(CurrentPosition, this.transform.right, Mathf.Clamp(Range, 0, 1000), _layerMask)
            .Where(x => x.transform.GetComponent<IAttackable>() != Owner)
            .Select(x => ((Vector3)x.point - CurrentPosition).magnitude)
            .OrderBy(x => x)
            .DefaultIfEmpty(-1)
            .FirstOrDefault();
    
    private void SetSpriteWidth() {
        float length = GetLaserLength();
        _spriter.size = new(length >= 0 ? length : Mathf.Clamp(Range, 0, 1000), _spriter.size.y);
    }
    private void SetSpriteHeight(SpriteRenderer spriter) {
        Sprite sprite = spriter.sprite;
        if (sprite == null) return;
        spriter.size = new(spriter.size.x, sprite.textureRect.size.y / sprite.pixelsPerUnit);
    }
}