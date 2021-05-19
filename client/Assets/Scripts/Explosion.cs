using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ParticleSystem sparks;
    public ParticleSystem smoke;
    public ParticleSystem fire;

    // Update is called once per frame
    void Update()
    {
        if (!sparks.isPlaying && !smoke.isPlaying && !fire.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
