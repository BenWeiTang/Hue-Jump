using System;
using UnityEngine;

public class BoundaryController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        var rb = col.GetComponent<Rigidbody2D>();
        var position = rb.position;
        position = new Vector2(-position.x, position.y);
        rb.position = position;
    }
}
