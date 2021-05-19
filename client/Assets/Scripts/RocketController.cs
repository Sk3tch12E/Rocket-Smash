using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RocketController : MonoBehaviour
{
    //use same rocket class as in
    public int playerid;
    public int id;
    float speed = 50f;

    //public Vector3 direction;

    public void Initialize(int _pid, int _rid)
    {
        playerid = _pid;
        id = _rid;
    }
    void Start()
    {
        //collider = this.GetComponent<Collider>();

        //direction = transform.forward;
        //direction.magnitude = speed;
        //speed *= Time.deltaTime;

    }
    void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (transform.position.x > 500 || transform.position.x < -500)
        { Disable(); }
        else if (transform.position.y > 500 || transform.position.y < -500)
        { Disable(); }
        else if (transform.position.z > 500 || transform.position.z < -500)
        { Disable(); }
    }
    public void Disable()
    {
        //GameManager.instance.rockets.Remove(this.gameObject);
        GameManager.instance.DisableRocket(this.gameObject);
        Destroy(this.gameObject);
    }
}
