using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    #region Inspector

    [SerializeField]
    private string enemyKey;

    #endregion

    #region Properties

    public string Key => enemyKey;

    public Vector2 Position => this.transform.position;

    #endregion

}