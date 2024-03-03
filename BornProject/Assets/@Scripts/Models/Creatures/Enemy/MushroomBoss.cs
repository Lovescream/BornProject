using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBoss : Boss
{
    private int attackIndex = 0;

    public override void Attack()
    {
        switch (attackIndex)
        {
            case 0:
                MushroomBomb();
                break;
            case 1:
                SporeShot();
                break;
            case 2:
                SporeSpread();
                break;
            case 3:
                MushroomMine();
                break;
        }

        attackIndex = (attackIndex + 1) % 4;

        base.Attack();
    }

    private void MushroomBomb()
    {
        Debug.Log("MushroomBomb 실행됨");
    }

    private void SporeShot()
    {
        Debug.Log("SporeShot 실행됨");
    }

    private void SporeSpread()
    {
        Debug.Log("SporeSpread 실행됨");
    }

    private void MushroomMine()
    {
        Debug.Log("MushroomMine 실행됨");
    }
}
