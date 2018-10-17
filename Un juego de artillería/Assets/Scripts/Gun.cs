using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{

    public Camera groundCamera;

    public enum States { Down, Up, Fire };
    public States state = States.Up;

    public Rigidbody2D rocket;				// Prefab of the rocket.
    public Sprite attackBarSprite;
    public delegate void GunFired();
    public event GunFired gunFired;

    Animator anim;                  // Reference to the Animator component.
    float speed = 0f;               // The speed the rocket will fire at.
    float targetSpeed = 0f;
    Transform attackBar;


    // Setting up the references.
    void Awake()
    {
        anim = transform.root.gameObject.GetComponent<Animator>();
        GameObject attackBarObject = new GameObject("Power");
        attackBar = attackBarObject.transform;
        attackBar.SetParent(transform);
        attackBar.localPosition = Vector3.zero;
        attackBar.localRotation = Quaternion.identity;
        attackBar.localScale = Vector3.up * 2 + Vector3.forward;
        SpriteRenderer rend = attackBarObject.AddComponent<SpriteRenderer>();
        rend.sprite = attackBarSprite;
        rend.sortingLayerID = transform.root.GetComponentInChildren<SpriteRenderer>().sortingLayerID;
    }


    void Update()
    {
        if (targetSpeed > 0)
        {
            state = States.Down;
            if (speed >= targetSpeed) state = States.Fire;
        }

        switch (state)
        {
            case States.Down:
                speed += Time.deltaTime * 30;
                attackBar.localScale += Vector3.right * 0.01f;
                attackBar.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Color.red, attackBar.localScale.x);
                break;
            case States.Fire:
                Fire();
                state = States.Up;
                break;
            default:
                break;
        }
    }

    public void FireDown()
    {
        state = States.Down;
    }

    public void FireUp()
    {
        //if it's the end of our turn and we are still charging our rocket
        //we will change 'state' from 'Down', in 'GameplayManager(line 61)', to 'Fire'
        //so if 'state' is not 'Down' we won't shoot anything, because the rocket has already been shoot
        state = state == States.Down ? States.Fire : States.Up;
    }

    public void Fire()
    {
        anim.SetTrigger("Shoot");
        GetComponent<AudioSource>().Play();
        Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, transform.rotation) as Rigidbody2D;

        //We pass the animator to our rocket->to animate the camera on explode
        bulletInstance.GetComponentInChildren<Rocket>().anim = anim;
        //pass groundCamera to rocket -> on explode capture image
        bulletInstance.GetComponentInChildren<Rocket>().groundCam = groundCamera;

        if (transform.root.GetComponent<PlayerControl>().facingRight)
        {
            bulletInstance.velocity = transform.right.normalized * speed;
        }
        else
        {
            bulletInstance.velocity = new Vector2(-transform.right.x, transform.right.y).normalized * speed;
        }
        bulletInstance.GetComponentInChildren<Rocket>().ignoreTag = transform.root.tag;
        
        
        
        targetSpeed = speed = 0;
        attackBar.localScale = Vector3.up * 2 + Vector3.forward;
        if (gunFired != null)
        {
            gunFired();
        }
    }

    public void Fire(float targetSpeed)
    {
        this.targetSpeed = targetSpeed;
    }
}
