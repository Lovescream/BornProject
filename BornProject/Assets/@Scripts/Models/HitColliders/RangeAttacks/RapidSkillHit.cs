using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidSkillHit : HitCollider
{
    #region Properties


    #endregion

    #region Fields

    protected float _unitRatio;
    protected LayerMask _layerMask;

    protected SpriteRenderer _spriter;

    #endregion

    #region MonoBehaviours

    protected override void Update()
    {
        base.Update();

        ShotRapid();
    }
    protected override void FixedUpdate() // Add
    {
        base.FixedUpdate();

        // Rigidbody2D를 사용해 오브젝트 이동
        _rigidbody.velocity = Velocity;
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
            Debug.LogError($"[Rapid] SetInfo({key}, {info}, {hitInfo}): Failed to find a sprite for this.");
            _unitRatio = 1f;
            return;
        }
        // 플레이어가 바라보는 방향을 Velocity로 설정
        Velocity = this.transform.right * info.Speed;

        _spriter.sprite = sprite;
        _unitRatio = sprite.textureRect.size.x / sprite.pixelsPerUnit;
        _layerMask = 1 << Main.CreatureLayer | 1 << Main.WallLayer; ; // 타겟 레이어 설정
    }

    #endregion

    private void ShotRapid()
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(CurrentPosition, Range, _layerMask);        
    }

}
