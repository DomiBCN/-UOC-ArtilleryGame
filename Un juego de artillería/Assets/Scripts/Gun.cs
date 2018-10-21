using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{

    public enum States { Down, Up, Fire };
    public enum Weapons { Rocket, AirAttack, Bomb };
    [HideInInspector]
    public States state = States.Up;
    [HideInInspector]
    public Weapons currentWeapon = Weapons.Rocket;

    public Rigidbody2D rocket;				// Prefab of the rocket.
    public Sprite attackBarSprite;
    public delegate void GunFired();
    public event GunFired gunFired;

    [SerializeField]
    Transform airAttackSelector;
    Animator anim;                  // Reference to the Animator component.
    float speed = 0f;               // The speed the rocket will fire at.
    float targetSpeed = 0f;
    float airAttackSpeed = 10f;
    Transform attackBar;

    public bool fired = false;

    // Setting up the references.
    void Awake()
    {
        currentWeapon = Weapons.Rocket;
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

        if (fired)
        {
            if (PlayerPrefs.GetInt("Exploded", 0) == 1)
            {
                PlayerPrefs.SetInt("Exploded", 0);
                fired = false;
                gunFired();
            }
        }
    }

    public void FireDown()
    {
        if (currentWeapon == Weapons.Rocket)
        {
            state = States.Down;
        }
        else if (currentWeapon == Weapons.AirAttack)
        {
            state = States.Fire;
        }
    }

    public void FireUp()
    {
        if (currentWeapon == Weapons.Rocket)
        {
            //if it's the end of our turn and we are still charging our rocket
            //we will change 'state' from 'Down' to 'Fire'('GameplayManager(line 58)')
            //If 'state' is not 'Down' we won't shoot anything, because the rocket has already been shoot
            state = state == States.Down ? States.Fire : States.Up;
        }
    }

    public void Fire()
    {
        switch (currentWeapon)
        {
            case Weapons.Rocket:
                FireRocket();
                break;
            case Weapons.AirAttack:
                AirAttack();
                break;
            default:
                break;
        }

        if (gunFired != null && !fired)
        {
            fired = true;
            //gunFired();
        }
    }

    public void Fire(float targetSpeed)
    {
        this.targetSpeed = targetSpeed;
    }

    void FireRocket()
    {
        anim.SetTrigger("Shoot");
        GetComponent<AudioSource>().Play();
        Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, transform.rotation) as Rigidbody2D;
        Rocket bulletScript = bulletInstance.GetComponent<Rocket>();
        bulletScript.rocketOrigin = transform.root.transform;
        //We pass the animator to our rocket->to animate the camera on explode
        bulletScript.anim = anim;

        if (transform.root.GetComponent<PlayerControl>().facingRight)
        {
            bulletInstance.velocity = transform.right.normalized * speed;
        }
        else
        {
            bulletInstance.velocity = new Vector2(-transform.right.x, -transform.right.y).normalized * speed;
            bulletInstance.transform.localScale = new Vector3(bulletInstance.transform.localScale.x * -1, bulletInstance.transform.localScale.y, bulletInstance.transform.localScale.z);
        }
        bulletInstance.GetComponentInChildren<Rocket>().ignoreTag = transform.root.tag;



        targetSpeed = speed = 0;
        attackBar.localScale = Vector3.up * 2 + Vector3.forward;

    }

    void AirAttack()
    {
        bool isEnemy = transform.root.tag == "Enemy";
        float posx = 12;
        float posY = 20;
        for (int i = 0; i < 4; i++)
        {
            //FireRocket();
            anim.SetTrigger("Shoot");
            GetComponent<AudioSource>().Play();

            Rigidbody2D bulletInstance = Instantiate(rocket, new Vector3(airAttackSelector.position.x - (isEnemy ? -posx : posx), posY, airAttackSelector.position.z), airAttackSelector.rotation) as Rigidbody2D;
            Rocket bulletScript = bulletInstance.GetComponent<Rocket>();
            bulletScript.rocketOrigin = transform.root.transform;
           
            bulletInstance.transform.rotation = Quaternion.Euler(0, 0, isEnemy ? -120 : -60);
            //We pass the animator to our rocket->to animate the camera on explode
            bulletScript.anim = anim;

            bulletInstance.velocity = bulletInstance.transform.right.normalized * airAttackSpeed;

            bulletInstance.velocity = transform.right.normalized * airAttackSpeed;


            posx += 2f;
        }
    }
}
