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

    bool destroyRocket = false;
    bool explodeRocket = true;

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

        //MULTIPLE OBJECTS DAMAGE
        #region multiple damage
        //if (col.tag == "BombPickup" || (col.gameObject.tag != ignoreTag && col.gameObject.tag != "Bullet" && col.gameObject.tag != "ExplosionFX" && !col.isTrigger))
        //{
        //    Collider2D[] collidersGround = Physics2D.OverlapCircleAll(transform.position, 1.5f, 1 << LayerMask.NameToLayer("Ground"));
        //    var collidersGroundGrouped = collidersGround.GroupBy(c => c.gameObject);
        //    Collider2D[] collidersPickups = Physics2D.OverlapCircleAll(transform.position, 1.5f, 1 << LayerMask.NameToLayer("Pickups"));
        //    bool crateExploded = false;
        //    foreach (Collider2D hit in collidersPickups)
        //    {
        //        if (!crateExploded)
        //        {
        //            crateExploded = true;
        //            // ... find the Bomb script and call the Explode function.
        //            hit.gameObject.GetComponent<Bomb>().Explode();
        //        }
        //        // Destroy the bomb crate.
        //        Destroy(hit.transform.root.gameObject);

        //        destroyRocket = true;
        //        explodeRocket = false;

        //    }

        //    // For each collider...
        //    foreach (var objectHit in collidersGroundGrouped)
        //    {
        //        Collider2D hit = objectHit.FirstOrDefault();
        //        //We use the BoxCollider just for pixel painting accuracy
        //        //The rocket has to explode with the other collider(PolygonCollider)
        //        BoxCollider2D boxCollider = hit.gameObject.GetComponent<BoxCollider2D>();

        //        if (!destroyRocket)
        //        {
        //            GameObject explosion = new GameObject("Explosion");
        //            explosion.transform.position = transform.position;
        //            explosion.tag = "ExplosionFX";
        //            Destroy(explosion, 0.5f);
        //            CircleCollider2D explosionRadius = explosion.AddComponent<CircleCollider2D>();
        //            explosionRadius.radius = 2.5f;
        //        }
        //        //Creates explosion crater setting pixels to alpha 0
        //        PixelsToAlpha.UpdateTexture(new Vector2(transform.position.x, transform.position.y), hit.gameObject, boxCollider, radius);
        //        destroyRocket = true;

        //    }
        //    if (destroyRocket)
        //    {
        //        // Instantiate the explosion and destroy the rocket.
        //        if (explodeRocket)
        //        {
        //            OnExplode();
        //        }
        //        Destroy(gameObject);
        //    }

        //}
        #endregion
        #region single object damage
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

            //Creates explosion crater setting pixels to alpha 0
            PixelsToAlpha.UpdateTexture(new Vector2(transform.position.x, transform.position.y), col.gameObject, boxCollider, radius);
            // Instantiate the explosion and destroy the rocket.
            OnExplode();
            Destroy(gameObject);

        }
        #endregion
    }
}
