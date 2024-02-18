using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Laser : HitCollider {

    #region Properties

    public float Length { get; protected set; }

    #endregion

    #region Fields

    protected float _unitRatio;
    protected LayerMask _layerMask;

    protected SpriteRenderer _spriter;

    #endregion

    #region MonoBehaviours

    protected override void Update() {
        base.Update();

        ShotLaser();
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        this._spriter = this.GetComponent<SpriteRenderer>();

        return true;
    }

    public override void SetInfo(string key, HitColliderInfo info, HitInfo hitInfo) {
        base.SetInfo(key, info, hitInfo);

        Sprite sprite = Main.Resource.LoadSprite(key);
        if (sprite == null) {
            Debug.LogError($"[Laser] SetInfo({key}, {info}, {hitInfo}): Failed to find a sprite for this.");
            _unitRatio = 1f;
            return;
        }
        _spriter.sprite = sprite;
        _unitRatio = sprite.textureRect.size.x / sprite.pixelsPerUnit;
        _layerMask = 1 << Main.CretureLayer | 1 << Main.WallLayer;
    }

    #endregion

    private void ShotLaser() {
        float range = Mathf.Clamp(Range, 0, 1000);
        RaycastHit2D[] hits = Physics2D.RaycastAll(CurrentPosition, this.transform.right, range, _layerMask);
        Length = range;

        var dlfmaajfhfgkwltlqk = hits.Where(x => x.transform.GetComponent<IAttackable>() != Owner);
        if (dlfmaajfhfgkwltlqk.Count() > 0)
            Length = dlfmaajfhfgkwltlqk.Select(x => (x.point - (Vector2)CurrentPosition).magnitude).OrderBy(x => x).FirstOrDefault();

        //for (int i = 0; i < hits.Length; i++) {
        //    if (hits[i].transform.GetComponent<IAttackable>() == Owner) continue;
        //    Length = (hits[i].point - (Vector2)CurrentPosition).magnitude;
        //    break;
        //}
        _spriter.size = new(_unitRatio * Length, _unitRatio);

        //if (hit)
        //    Length = (hit.point - (Vector2)CurrentPosition).magnitude;
    }
}