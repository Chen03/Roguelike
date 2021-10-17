using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject HoldingWeapon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 position = transform.position,
            direction = new Vector2(1, 0);
        if (Input.anyKeyDown) {
            HoldingWeapon.GetComponent<Animator>().SetTrigger("HasUsed");
            var result = RaycastTest(position, direction, 30, 5, 10);
            foreach(Collider2D c in result) {
                c.GetComponent<HealthSystem>().GetHit(1);
            }
        }

        Debug.DrawRay(position, direction * 5, Color.green);
    }

    HashSet<Collider2D> RaycastTest(Vector2 position, Vector2 direction,
        float angle, float distance, int accuarcy) {
        HashSet<Collider2D> list = new HashSet<Collider2D>();
        direction = Quaternion.Euler(0, 0, angle) * direction;
        accuarcy = System.Math.Max(accuarcy, 2);
        
        RaycastHit2D[] result;
        for (int i = 0; i <= accuarcy; ++i) {
            Debug.DrawRay(position, direction * distance, Color.red, 0.2f);
            result = Physics2D.RaycastAll(position, direction, distance);
            foreach (RaycastHit2D r in result) {
                if (r.collider.CompareTag("Mob"))
                    list.Add(r.collider);
            }
            direction = Quaternion.Euler(0, 0, -angle * 2 / accuarcy) * direction;
        }

        return list;
    }
}
