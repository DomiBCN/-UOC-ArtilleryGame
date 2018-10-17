using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour
{
    public Camera groundCam;

    public GameObject explosion;		// Prefab of explosion effect.
    public string ignoreTag;
    public Animator anim;

    void Start()
    {
        // Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
        Destroy(gameObject, 5);
    }


    void OnExplode()
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
        //// If it hits an enemy...
        //if(col.tag == "Enemy")
        //{
        //	// ... find the Enemy script and call the Hurt function.
        //	col.gameObject.GetComponent<Enemy>().Hurt();

        //	// Call the explosion instantiation.
        //	OnExplode();

        //	// Destroy the rocket.
        //	Destroy (gameObject);
        //}
        // Otherwise if it hits a bomb crate...
        //else 
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
        else if (col.gameObject.tag != ignoreTag)
        {
            GameObject explosion = new GameObject("Explosion");
            explosion.transform.position = transform.position;
            explosion.tag = "ExplosionFX";
            Destroy(explosion, 0.5f);
            CircleCollider2D explosionRadius = explosion.AddComponent<CircleCollider2D>();
            explosionRadius.radius = 2.5f;
            
            //creates screenshot of collided object with a spriteMask for crater effect -> replace sprite with the screenshot
            //CameraGround.TakeScreenShoot_Static(groundCam.pixelWidth, groundCam.pixelHeight, col.gameObject);
            
            // Instantiate the explosion and destroy the rocket.
            OnExplode();
            Destroy(gameObject);
        }
    }
}
