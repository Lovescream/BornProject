using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserSkillHit : HitCollider {

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

    protected override void FixedUpdate()
    {
        // 레이저의 컨셉에 따라 삭제할 수 있음.
        if (_enabled == false && gameObject.activeSelf)
        {
            this.transform.SetParent(null);
            _enabled = true;
        }
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
        _unitRatio = 1;
        _layerMask = 1 << Main.CreatureLayer | 1 << Main.WallLayer;
    }

    #endregion

    private void ShotLaser()
    {
        float range = Mathf.Clamp(Range, 0, 1000);
        RaycastHit2D[] hits = Physics2D.RaycastAll(CurrentPosition, this.transform.right, range, _layerMask);
        Length = range;

        // Assuming 'creaturesLayerNumber' is the layer number for "Creatures"
        int creaturesLayerNumber = LayerMask.NameToLayer("Creatures");

        var relevantHits = hits.Where(hit => hit.transform.gameObject.layer != creaturesLayerNumber &&
                                             hit.transform.GetComponent<IAttackable>() != Owner);

        if (relevantHits.Any())
            Length = relevantHits.Select(hit => (hit.point - (Vector2)CurrentPosition).magnitude)
                                  .OrderBy(distance => distance)
                                  .FirstOrDefault();

        _spriter.size = new Vector2(_unitRatio * Length, _spriter.size.y);
    }
}