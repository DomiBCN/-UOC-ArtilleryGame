using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour
{
    public GameObject explosion;		// Prefab of explosion effect.
    public string ignoreTag;
    public Animator anim;

    private Texture2D newTexture;
    private Color[] textureColors;
    BoxCollider2D hit;
    SpriteRenderer spriteRend;
    Color zeroAlpha = Color.clear;
    public int radius = 40;

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
        //Bullet tag -> if we wouldn't put it, it wouldn' cause any trouble but we don't want to set boxCollider for nothing
        //ExplosionFX tag -> Necessary! When doing an air attack we don't want explosions triggered by other rocket explosions
        else if (col.gameObject.tag != ignoreTag && col.gameObject.tag != "Bullet" && col.gameObject.tag != "ExplosionFX")
        {
            //We use the BoxCollider just for pixel painting accuracy
            //The rocket has to explode with the other collider(PolygonCollider)
            BoxCollider2D boxCollider = col.gameObject.GetComponent<BoxCollider2D>();

            if (col != boxCollider)
            {
                GameObject explosion = new GameObject("Explosion");
                explosion.transform.position = transform.position;
                explosion.tag = "ExplosionFX";
                Destroy(explosion, 0.5f);
                CircleCollider2D explosionRadius = explosion.AddComponent<CircleCollider2D>();
                explosionRadius.radius = 2.5f;

                //Creates explosion crater setting pixels to alpha 0
                PixelsToAlpha.UpdateTexture(new Vector2(transform.position.x, transform.position.y), col.gameObject, boxCollider, radius);
                //StartCoroutine(UpdateTexture(col.gameObject, boxCollider));
                // Instantiate the explosion and destroy the rocket.
                OnExplode();
                Destroy(gameObject);
            }
        }
    }

    #region obsolete
    //IEnumerator UpdateTexture(GameObject objectToExplode, BoxCollider2D boxCollider)
    //{
    //    try
    //    {
    //        spriteRend = objectToExplode.GetComponent<SpriteRenderer>();
    //        var tex = spriteRend.sprite.texture;
    //        newTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
    //        newTexture.filterMode = FilterMode.Bilinear;
    //        newTexture.wrapMode = TextureWrapMode.Clamp;
    //        textureColors = tex.GetPixels();
    //        newTexture.SetPixels(textureColors);
    //        newTexture.Apply();
    //        spriteRend.sprite = Sprite.Create(newTexture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f), spriteRend.sprite.pixelsPerUnit);

    //        updatePixels(boxCollider);
    //    }
    //    catch (System.Exception e)
    //    {
    //        string errorMeesage = e.Message;
    //    }
    //    yield return true;
    //}

    //void updatePixels(BoxCollider2D boxCollider)
    //{
    //    #region Modify pixels
    //    int w = newTexture.width;
    //    int h = newTexture.height;
    //    var mousePos = new Vector2(transform.position.x, transform.position.y) - (Vector2)boxCollider.bounds.min;
    //    mousePos.x *= w / boxCollider.bounds.size.x;
    //    mousePos.y *= h / boxCollider.bounds.size.y;
    //    Vector2Int p = new Vector2Int((int)mousePos.x, (int)mousePos.y);
    //    Vector2Int start = new Vector2Int();
    //    Vector2Int end = new Vector2Int();
    //    //if (!Drawing)
    //    //    lastPos = p;
    //    start.x = Mathf.Clamp(p.x - radius, 0, w);
    //    start.y = Mathf.Clamp(p.y - radius, 0, h);
    //    end.x = Mathf.Clamp(p.x + radius, 0, w);
    //    end.y = Mathf.Clamp(p.y + radius, 0, h);
    //    Vector2 dir = p;
    //    for (int x = start.x; x < end.x; x++)
    //    {
    //        for (int y = start.y; y < end.y; y++)
    //        {
    //            Vector2 pixel = new Vector2(x, y);
    //            Vector2 linePos = p;
    //            //if (Drawing)
    //            //{
    //            //    float d = Vector2.Dot(pixel - lastPos, p) / p.sqrMagnitude;
    //            //    d = Mathf.Clamp01(d);
    //            //    linePos = Vector2.Lerp(lastPos, p, d);
    //            //}
    //            if ((pixel - linePos).sqrMagnitude <= radius * radius)
    //            {
    //                textureColors[x + y * w] = zeroAlpha;
    //            }
    //        }
    //    }
    //    //lastPos = p;
    //    #endregion
    //    #region UpdatePixels
    //    newTexture.SetPixels(textureColors);
    //    newTexture.Apply();
    //    #endregion
    //    #region Create sprite && reAdd Polygon collider
    //    spriteRend.sprite = Sprite.Create(newTexture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f), spriteRend.sprite.pixelsPerUnit);
    //    Destroy(spriteRend.gameObject.GetComponent<PolygonCollider2D>());
    //    spriteRend.gameObject.AddComponent<PolygonCollider2D>();
    //    #endregion

    //    OnExplode();
    //    Destroy(gameObject);
    //}
    #endregion
}
