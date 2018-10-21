using UnityEngine;
using System.Collections;
using System.Linq;

public class Rocket : MonoBehaviour
{
    public GameObject explosion;		// Prefab of explosion effect.
    public string ignoreTag;
    public Animator anim;
    //radius of destruction
    public int radius = 40;

    public Transform rocketOrigin;

    void Start()
    {
        
        if(PlayerPrefs.GetInt("Exploded", 0) != 1)
        {
            Camera.main.GetComponent<CameraFollow>().SetPlayerToFollow(transform);
            PlayerPrefs.SetInt("Exploded", 0);
        }
        // Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
        Destroy(gameObject, 5);
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("Exploded", 1);
        //Camera.main.GetComponent<CameraFollow>().tracking = null;
    }

    public void OnExplode()
    {
        // Create a quaternion with a random rotation in the z-axis.
        Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        // Instantiate the explosion where the rocket is with the random rotation.
        Instantiate(explosion, transform.position, randomRotation);
        
        //CAMERA VIBRATION EFFECT
        //anim.SetTrigger("Boom");

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "BombPickup")
        {
            // ... find the Bomb script and call the Explode function.
            col.gameObject.GetComponent<Bomb>().Explode();

            // Destroy the bomb crate.
            Destroy(col.transform.root.gameObject);

            // Destroy the rocket.
            Destroy(gameObject);
        }
        // Otherwise if the player manages to shoot himself...
        //Bullet tag -> if we wouldn't put it, it wouldn' cause any trouble but we don't want to set boxCollider for nothing
        //ExplosionFX tag -> Necessary! When doing an air attack we don't want explosions triggered by other rocket explosions
        else if (col.gameObject.tag != ignoreTag && col.gameObject.tag != "Bullet" && col.gameObject.tag != "ExplosionFX" && col.tag != "PlatformEnd" && !col.isTrigger)
        {
            //We use the BoxCollider just for pixel painting accuracy
            //The rocket has to explode with the other collider(PolygonCollider)
            BoxCollider2D boxCollider = col.gameObject.GetComponent<BoxCollider2D>();

            GameObject explosion = new GameObject("Explosion");
            explosion.transform.position = transform.position;
            explosion.tag = "ExplosionFX";
            Destroy(explosion, 0.5f);
            CircleCollider2D explosionRadius = explosion.AddComponent<CircleCollider2D>();
            explosionRadius.radius = 2.5f;

            //we don't want to remove player pixels
            if (col.gameObject.tag != "Player" && col.gameObject.tag != "Enemy")
            {
                //Pixel destruction evaluation will take a while, and the rocket would keep going, that's why we have to set the rigidbody to static once the rocket has collided
                gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                //Creates explosion crater setting pixels to alpha 0
                transform.GetComponent<PixelsToAlpha>().UpdateTexture(new Vector2(transform.position.x, transform.position.y), col.gameObject, boxCollider, radius, Gun.Weapons.Rocket);
            }
            else
            {
                OnExplode();
                Destroy(gameObject);
            }
            
        }
    }
}
