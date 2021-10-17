using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    Rigidbody2D rb;
    Animator anim;
    Vector2 movement;

    public GameObject HoldingWeapon;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown){
            HoldingWeapon.GetComponent<Animator>().SetTrigger("HasUsed");
        }
    }

    void FixedUpdate() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        anim.SetBool("IsRunning", movement.magnitude > 1e-9);
        if (movement.x != 0)
            transform.localScale = new Vector3(movement.x, 1, 1);
    }
}
