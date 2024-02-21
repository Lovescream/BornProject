using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SmashSkillHit : HitCollider{
    #region Properties

    public float AttackRadius { get; private set; }

    #endregion

    #region Fields

    protected float _unitRatio;
    protected LayerMask _LayerMask;

    protected SpriteRenderer _spriter;

    #endregion

    #region MonoBehaviours

    protected override void Update()
    {
        base.Update();

        AttackSmash();
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        // 여기에서 필요한 초기화 수행
        this._spriter = this.GetComponent<SpriteRenderer>();

        return true;
    }

    public override void SetInfo(string key, HitColliderInfo info, HitInfo hitInfo)
    {
        base.SetInfo(key, info, hitInfo);

        Sprite sprite = Main.Resource.LoadSprite(key);
        if (sprite == null)
        {
            Debug.LogError($"[Smash] SetInfo({key}, {info}, {hitInfo}): Failed to find a sprite for this.");
            _unitRatio = 1f;
            return;
        }
        _unitRatio = sprite.textureRect.size.x / sprite.pixelsPerUnit;
        // 공격 범위 설정
        AttackRadius = info.Range;
        _LayerMask = 1 << Main.CreatureLayer; // 타겟 레이어 설정
    }

    #endregion

    private void AttackSmash()
    {
        
    }


}
