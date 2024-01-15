using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ResourceManager : MonoBehaviour
{
    public Dictionary<string, Sprite> Enemy { get; private set; } = new();
    public GameObject EnemyPrefab { get; private set; }

    public void Initialize()
    {
        Sprite[] enemys = Resources.LoadAll<Sprite>("Sprites/Items");
        foreach (Sprite sprite in enemys)
        {
            Enemy.Add(sprite.name, sprite);
        }

        EnemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
    }
}