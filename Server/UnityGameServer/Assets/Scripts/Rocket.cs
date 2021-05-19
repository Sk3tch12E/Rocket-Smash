using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    //public Collider collider;
    public Rigidbody rb;
    
    //identifiers
    public int playerid;
    public GameObject player;
    public int id;

    //variables
    float speed = 50f;
    public float exposionRadius = 5f;
    //public Transform shotOrigin;
    //public int multiplyer = 0;
    //bool isDead = false;

    public Rocket()
    { 
        
    }

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
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

    private void OnCollisionEnter(Collision collision)
    {
        //explode

        if (collision.transform.tag == "Projectile")
        {
            return;//ignore other projectiles
        }
        else if (collision.transform.tag == "Border")
        {
            Disable();
            return;
        }
        
        //GameObject explode = Instantiate(explosionPrefab, transform.position, transform.rotation);
        Knockback();
        //server send explode(id)
        ServerSend.RocketExplode(playerid,id);
        Destroy(this.gameObject);
    }

    private void Knockback()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, exposionRadius);
        
        foreach (Collider c in colliders)
        {
            if (c.tag == "Player")
            {
                Vector3 direction = (c.transform.position - transform.position).normalized;//Vector3.RotateTowards(c.transform.position,transform.position , 1f, 0f);
                direction.y *= 5f;
                Debug.DrawLine(c.transform.position, transform.position, Color.cyan, 10f);
                //direction.Normalize();
                if (playerid != c.GetComponent<Player>().id)
                {
                    c.GetComponent<Player>().multiplyer += 5;
                }
                c.GetComponent<Player>().hitrecent = true;
                c.GetComponent<Player>().lastPlayerHitBy = player;//set the player to say it was hit by this players rocket
                c.GetComponent<CharacterController>().Move(new Vector3(0, direction.y, 0));
                c.GetComponent<Player>().prevVelocity = direction * c.GetComponent<Player>().multiplyer * Time.deltaTime;  //.AddExplosionForce(100, transform.position, exposionRadius);//c.GetComponent<Player>().multiplyer
                //maybe switch to a rigidbod until it hits the ground? Then can use addexplosionforce
                //return
            }
        }
    }

    public void Initialise(GameObject _player, int _playerID, int _id)//, Vector3 _dir)
    {
        player = _player;
        playerid = _playerID;
        id = _id;
        //direction = transform.forward;
        //transform.forward = _dir;
    }

    private void Disable()
    {
        Destroy(this.gameObject);
    }
}
