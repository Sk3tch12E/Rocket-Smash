using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;

public class Player : MonoBehaviour
{
    public CharacterController controller;
    public GameObject rocket;
    //identifiers
    public int id;
    public string username;

    //variables
    public Transform shotOrigin;
    public int multiplyer = 0;
    bool isDead = false;
    public bool canShoot = true;
    //Moving
    public float g = -12f;
    public float acceleration = 1f;
    public float jumpHeight = 9f;
    public float groundSpeed = 10f;
    public float jumpSpeed = 10f;
    public float terminalVel = 1000f;
    public float rocketJumpSpeed = 15f;
    public Vector3 prevVelocity;
    public bool hitrecent = false;
    int rocketid = 0;

    private bool[] inputs;
    private float yVelocity = 0f;

    //Scoreing
    public int score = 0;
    public GameObject lastPlayerHitBy;

    private void Start()
    {
        prevVelocity = new Vector3(0, 0, 0);

        float dt = Time.deltaTime;

        g *= dt * dt; //persec persec
        acceleration *= dt;
        jumpHeight *= dt;
        groundSpeed *= dt;
        jumpSpeed *= dt;
        rocketJumpSpeed *= dt;
        controller.detectCollisions = true;
    }

    private void Update()
    {
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        inputs = new bool[5];
    }

    public void FixedUpdate()
    {
        if (!isDead)
        {
            Vector2 inputDirection = Vector2.zero;
            if (inputs[0])
            {
                inputDirection.y += 1;
            }
            if (inputs[1])
            {
                inputDirection.y -= 1;
            }
            if (inputs[2])
            {
                inputDirection.x -= 1;
            }
            if (inputs[3])
            {
                inputDirection.x += 1;
            }
            if (controller.enabled)
            {
                Move(inputDirection);
            }
        }
    }

    #region Character movement
    private void Move(Vector2 _inputDirection)
    {
        //if player grounded
        //if player jump is queued
        //make player jump
        //
        //move on ground -> accelerate -> friction
        //
        //else
        //move in air -> accelerate

        Vector3 pos = GetComponent<Transform>().position;
        Vector3 newVelocity = prevVelocity; //vector to store new velocity
        //Direction the player wishes to move in
        Vector3 wishDir = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        if (controller.isGrounded)
        {
            yVelocity = 0; //reset y velocity on the ground
            if (inputs[4]) //if space pressed
            {
                yVelocity = jumpHeight;// Mathf.Sqrt(jumpSpeed * -2f * g);//set y to the jump speed using this equation for more control
                //AirMove(ref newVelocity, wishDir);  //now considered to be in the air
            }
            else
            {
                GroundMove(ref newVelocity, wishDir, pos);
            }
        }
        else
        {
            yVelocity += g;
            AirMove(ref newVelocity, wishDir);
        }
        newVelocity.y = yVelocity;

        //draw lines for debugging        
        Debug.DrawRay(pos, wishDir * 10, Color.blue);
        Debug.DrawRay(pos, transform.forward * 2, Color.red);
        Debug.DrawRay(pos, newVelocity * 10, Color.green);

        controller.Move(newVelocity);
        prevVelocity = newVelocity;

        //send pos and rot
        //send pos to all
        ServerSend.PlayerPosition(this);
        //send rot to all but sender
        ServerSend.PlayerRotation(this);

        //Debug.Log(prevVelocity.magnitude);
    }
    //modified char controller inspired by Quake and halflife
    private void Accelerate(ref Vector3 _velocity, Vector3 _wishDir, float _maxVel)
    {
        float projespeed = Vector3.Dot(_velocity, _wishDir);//Vector Projection of currentvel onto accelDir
        float accelVel = acceleration;
        //If necessary, truncate accelerated velocity so the vector projection does not exceed max velocity
        if (projespeed + accelVel > _maxVel)
        {
            accelVel = _maxVel - projespeed;
        }
        _velocity = _velocity + _wishDir * accelVel;
        //_velocity = Vector3.ClampMagnitude(_velocity, _maxVel);
    }
    private void GroundMove(ref Vector3 _velocity, Vector3 _wishDir, Vector3 _position)
    {
        //dont use the accelerate function. Allows for better responsivness
        _velocity = _wishDir * groundSpeed * 0.6f; //*friction(0.6)
        //Debug.Log("Ground");
        if (hitrecent)
        {
            hitrecent = false;
        }
    }

    private void AirMove(ref Vector3 _velocity, Vector3 _wishDir)
    {
        if (!hitrecent)
        {
            Accelerate(ref _velocity, _wishDir, jumpSpeed);//send terminal velocity as they are moving in air if they jumped
        }
        else
        {
            Accelerate(ref _velocity, _wishDir, terminalVel);
        }
        //Debug.Log("air");
    }

    public void SetInputs(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }
    #endregion

    public void Shoot(Quaternion _viewDir)
    {
        if (canShoot)
        {
            //spawn rocket
            //Debug.DrawRay(shotOrigin.position, _viewDir.eulerAngles * 10f, Color.green, 2f);
            GameObject rcket = Instantiate(rocket, shotOrigin.position, _viewDir);
            rcket.GetComponent<Rocket>().Initialise(this.gameObject, id, rocketid);
            rocketid += 1;
            //send spawn rocket
            ServerSend.RocketSpawn(rcket);
            canShoot = false;


            //hit scan method
            //if(Physics.Raycast(shotOrigin.position, _viewDir, out RaycastHit hit, 25f)) //using raycast atm. Switch to creating a projectile
            //{
            //    if (hit.collider.CompareTag("Player"))
            //    {
            //        hit.collider.GetComponent<Player>().Hit(10);
            //    }
            //}
            StartCoroutine(Reload());
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {        
        if (hit.gameObject.tag == "Border")
        {
            Debug.Log("Out of bounds");
            Kill();
        }
    }

    private void Kill()
    {
        //kill the player and credit kill
        canShoot = false;
        Debug.Log($"can shoot: {canShoot}");
        StartCoroutine(Respawn());
        int x = Random.Range(0, 50);
        int z = Random.Range(0, 50);
        transform.position = new Vector3(x, 50, z);


        if (lastPlayerHitBy != null && lastPlayerHitBy.GetComponent<Player>().id != id)
        {
            lastPlayerHitBy.GetComponent<Player>().AddScore();
            ServerSend.SetScore(lastPlayerHitBy.GetComponent<Player>().id, lastPlayerHitBy.GetComponent<Player>().score);
        }
        else
        {
            score--;//remove score for jumping off constantly
            ServerSend.SetScore(id, score);
        }

        lastPlayerHitBy = null; // reset so jumping off doesnt credit kill        

        controller.enabled = false;
        ServerSend.playerDead(this);
        Debug.Log($"can shoot: {canShoot}");
    }

    private IEnumerator Respawn()//runs over frames not within a single frame
    {
        
        yield return new WaitForSecondsRealtime(5f);//wait 5 sec before respawning
        canShoot = true;
        multiplyer = 0;
        prevVelocity = new Vector3(0, 0, 0);
        controller.enabled = true;
        ServerSend.playerRespawned(this);
    }
    private IEnumerator Reload()//runs over frames not within a single frame
    {
        if (controller.enabled)
        {
            yield return new WaitForSecondsRealtime(.5f);//wait 5 sec before respawning
            canShoot = true;
        }
    }

    public void AddScore()
    {
        score++;
    }
}
