using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Racket : NetworkBehaviour
{
    public float speed = 30;
    public Rigidbody2D rigidbody2d;

    [Client]
    void FixedUpdate()
    {
        if (!isOwned) return;
           
        rigidbody2d.velocity = new Vector2(0, Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
    }
}
