using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelsToAlpha : MonoBehaviour
{

    Vector2 explosionPoint;
    BoxCollider2D boxCollider;
    Texture2D newTexture;
    Color[] textureColors;
    SpriteRenderer spriteRend;
    Color zeroAlpha = Color.clear;
    int radius;

    bool updatePix = false;

    Vector2 position;
    GameObject objectToExplode;
    BoxCollider2D boxCol;
    int explosionRadius;
    Gun.Weapons weaponToExplode;

    public void UpdateTexture(Vector2 position, GameObject objectToExplode, BoxCollider2D boxCol, int explosionRadius, Gun.Weapons weaponToExplode)
    {
        this.position = position;
        this.objectToExplode = objectToExplode;
        this.boxCol = boxCol;
        this.explosionRadius = explosionRadius;
        this.weaponToExplode = weaponToExplode;

        updatePix = true;
    }

    private void FixedUpdate()
    {
        if (updatePix)
        {
            //In case we are launching an AirAttack we need to ensure that rockets are evaluated one by one.
            //Otherwise each rocket will be doing copies of the textures and modifying them at the same time.
            //And the final rendered texture we will use to update the sprite, won't be the result of all the rockets destruction
            if (!GamePlayManager.coroutineExplosionOn)
            {
                GamePlayManager.coroutineExplosionOn = true;
                updatePix = false;
                StartCoroutine("UpdatePixels");
            }
        }
    }

    void LoadData()
    {
        explosionPoint = position;
        boxCollider = boxCol;
        radius = explosionRadius;
        spriteRend = objectToExplode.GetComponent<SpriteRenderer>();
        var tex = spriteRend.sprite.texture;
        newTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        newTexture.filterMode = FilterMode.Bilinear;
        newTexture.wrapMode = TextureWrapMode.Clamp;
        textureColors = tex.GetPixels();
        newTexture.SetPixels(textureColors);
        newTexture.Apply();
        spriteRend.sprite = Sprite.Create(newTexture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f), spriteRend.sprite.pixelsPerUnit);
    }

    IEnumerator UpdatePixels()
    {
        LoadData();
        yield return null;
        #region Modify pixels
        int w = newTexture.width;
        int h = newTexture.height;
        var mousePos = explosionPoint - (Vector2)boxCollider.bounds.min;
        mousePos.x *= w / boxCollider.bounds.size.x;
        mousePos.y *= h / boxCollider.bounds.size.y;
        Vector2Int p = new Vector2Int((int)mousePos.x, (int)mousePos.y);
        Vector2Int start = new Vector2Int();
        Vector2Int end = new Vector2Int();
        start.x = Mathf.Clamp(p.x - radius, 0, w);
        start.y = Mathf.Clamp(p.y - radius, 0, h);
        end.x = Mathf.Clamp(p.x + radius, 0, w);
        end.y = Mathf.Clamp(p.y + radius, 0, h);
        for (int x = start.x; x < end.x; x++)
        {
            for (int y = start.y; y < end.y; y++)
            {
                Vector2 pixel = new Vector2(x, y);
                Vector2 linePos = p;
                if ((pixel - linePos).sqrMagnitude <= radius * radius)
                {
                    textureColors[x + y * w] = zeroAlpha;
                }
            }
        }
        #endregion
        yield return null;
        #region UpdatePixels
        newTexture.SetPixels(textureColors);
        newTexture.Apply();
        #endregion
        yield return null;
        #region Create sprite && reAdd Polygon collider
        spriteRend.sprite = Sprite.Create(newTexture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f), spriteRend.sprite.pixelsPerUnit);
        Destroy(spriteRend.gameObject.GetComponent<PolygonCollider2D>());
        spriteRend.gameObject.AddComponent<PolygonCollider2D>();
        #endregion
        yield return null;
        if (weaponToExplode == Gun.Weapons.Rocket)
        {
            transform.root.GetComponent<Rocket>().OnExplode();
        }
        else if(weaponToExplode == Gun.Weapons.Bomb)
        {
            transform.root.GetComponent<Bomb>().FinalExplosion();
        }
        Destroy(gameObject);
        GamePlayManager.coroutineExplosionOn = false;
    }
}
