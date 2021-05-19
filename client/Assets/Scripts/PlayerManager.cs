using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public int multiplier;
    public int score = 0;
    public MeshRenderer model;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        multiplier = 0;

    }

    public void setMultiplier(int _mul)
    {
        multiplier = _mul;
    }

    public void Die()
    {
        model.enabled = false;
    }
    public void Respawn()
    {
        model.enabled = true;
        multiplier = 0;
    }
}
