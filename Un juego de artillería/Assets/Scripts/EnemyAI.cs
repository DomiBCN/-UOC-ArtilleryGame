using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : PlayerControl
{

    Transform hero;
    Gun gun;
    float shootAngle = 30;

    // Use this for initialization
    void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player").transform;
        gun = GetComponentInChildren<Gun>();
    }

    float CalculeVelocity(Transform hero, float shootAngle)
    {
        var dir = hero.position - transform.position;
        var h = dir.y;
        dir.y = 0;
        var dist = dir.magnitude;
        var a = shootAngle * Mathf.Deg2Rad;

        dist += h / Mathf.Tan(a);

        return Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a)) * Random.Range(1.2f, 1.8f);
    }

    void LateUpdate()
    {
        if (hasTurn)
        {
            if (facingRight)
            {
                Flip();
            }
            if(tilt != shootAngle)
            {
                if(tilt < shootAngle)
                {
                    RotateUpDown();
                }
                else
                {
                    RotateDownDown();
                }
            }
            else
            {
                gun.Fire(CalculeVelocity(hero, shootAngle));
                RotateUpUp();
                RotateDownUp();
            }
        }
        else
        {
            shootAngle = Random.Range(25, 45);
        }
    }
}
