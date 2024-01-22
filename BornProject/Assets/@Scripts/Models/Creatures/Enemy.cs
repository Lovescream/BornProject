using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature {
    public new CreatureData Data { get; private set; }
    public Transform PlayerTransform;

    protected override void SetStateEvent()
    {
        base.SetStateEvent();
        State.AddOnStay(CreatureState.Idle, () =>
        {
            Velocity = Vector2.zero;
        });
    }
    protected override void SetStatus(bool isFullHp = true)
    {
        this.Status = new(Data);
        if (isFullHp)
        {
            Hp = Status[StatType.HpMax].Value;
        }

        //OnChangedHp -= ShowHpBar; // Hp바 만들때 사용예정.
        //OnChangedHp += ShowHpBar;
    }

    void Update()
    {
        // 플레이어가 범위 내에 있는지 확인.
        if (SightPlayerInRange())
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMoving();
        }
    }

    // 플레이어 쪽으로 이동.
    private void MoveTowardsPlayer()
    {
        // 플레이어를 따라가기 위한 Transform이 있는지 확인.
        if (PlayerTransform != null)
        {
            // 플레이어 쪽으로의 방향 계산.
            Vector2 direction = (PlayerTransform.position - transform.position).normalized;
            // 적을 플레이어 쪽으로 이동.
            transform.position = Vector2.MoveTowards(transform.position, PlayerTransform.position, Data.MoveSpeed * Time.deltaTime);
        }
    }

    // 적의 이동을 멈추는 메소드.
    private void StopMoving()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    // 플레이어가 범위 내에 있는지 감지하고, 감지된 경우 플레이어의 Transform을 PlayerTransform에 할당
    public bool SightPlayerInRange()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, Data.Sight);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // 플레이어의 Transform을 PlayerTransform에 할당
                PlayerTransform = hitCollider.transform;
                return true;
            }
        }

        // 플레이어를 찾지 못한 경우, PlayerTransform을 null로 설정하고 false 반환
        PlayerTransform = null;
        return false;
    }
}