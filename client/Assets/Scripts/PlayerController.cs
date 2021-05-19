using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Transform camTransform;
    public GameObject rocket;
    public int ping = 0;
    public int t1;

    private void Awake()
    {
        instance = this;
    }

    public int ID()
    {
        return this.gameObject.GetComponent<PlayerManager>().id;
    }

    void FixedUpdate()
    {
        SendInputToServer();
    }

    private void Update()
    {        
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            ClientSend.PlayerShoot(camTransform.rotation);//forward
            t1 = DateTime.Now.Millisecond;
            //GameObject g = Instantiate(rocket, transform.position, camTransform.rotation);
            //g.transform.forward = camTransform.forward;
        }
    }

    private void SendInputToServer() //move to update for more responsive input
    {
        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space)
        };
        ClientSend.PlayerMovement(inputs);
    }

    public void PingRecived()
    {
        int t2 = DateTime.Now.Millisecond;
        int ping = t2 - t1;
        GameManager.instance.UpdatePing(ping);
    }
}
