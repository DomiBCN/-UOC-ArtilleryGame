using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public Rigidbody2D rocket;				// Prefab of the rocket.
    public Sprite attackBarSprite;

    
    Animator anim;                  // Reference to the Animator component.
    float speed = 0f;               // The speed the rocket will fire at.
    Transform attackBar;
    enum States { Down, Up, Fire };

    States state = States.Up;

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
        state = States.Fire;
    }

    public void Fire()
    {
        anim.SetTrigger("Shoot");
        GetComponent<AudioSource>().Play();
        Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, transform.rotation) as Rigidbody2D;
        bulletInstance.velocity = transform.right.normalized * speed;
        speed = 0;
        attackBar.localScale = Vector3.up * 2 + Vector3.forward;
    }
}
