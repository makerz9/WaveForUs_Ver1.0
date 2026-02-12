using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{

    public Rigidbody2D rb;


    // Start is called before the first frame update
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Water"))
        {
            rb.AddForce(Vector2.up * 5, ForceMode2D.Force);
        }
    }



}
