using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZerolizeDungeon;

public class Debris : Entity {

    public Room Room { get; protected set; }
    public Direction Direction { get; protected set; }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.GetComponent<Creature>() == null) return;
        Bounds bounds = this.GetComponent<BoxCollider2D>().bounds;
        BoxCollider2D box = collision.transform.GetComponent<BoxCollider2D>();

        float x = Direction switch {
            Direction.Right => bounds.center.x - bounds.extents.x - box.size.x / 2 - box.offset.x,
            Direction.Left => bounds.center.x + bounds.extents.x + box.size.x / 2 - box.offset.x,
            _ => box.transform.position.x
        };
        float y = Direction switch {
            Direction.Top => bounds.center.y - bounds.extents.y - box.size.y / 2 - box.offset.y,
            Direction.Bottom => bounds.center.y + bounds.extents.y + box.size.y / 2 - box.offset.y,
            _ => box.transform.position.y
        };

        box.transform.position = new(x, y);
    }

    public void SetInfo(Room room, Direction direction) {
        this.Room = room;
        this.Direction = direction;

        this.transform.SetParent(Room.transform);
        this.transform.localPosition = Direction switch {
            Direction.Top => new(15, 28),
            Direction.Right => new(28, 15),
            Direction.Bottom => new(15, 2),
            Direction.Left => new(2, 15),
            _ => new(0, 0)
        };
    }

}