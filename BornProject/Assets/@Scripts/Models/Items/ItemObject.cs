using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemObject : MonoBehaviour
{
    private Rigidbody2D _rigid;
    private CircleCollider2D _collider;
    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Inventory.Instance.AddItem(item);
            //collision.gameObject.GetComponent<Inventory>().AddItem(item);
            //Destroy(gameObject);
        }
    }
}

