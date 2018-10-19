using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelsToAlpha : MonoBehaviour
{
    
    static Vector2 explosionPoint;
    static BoxCollider2D boxCollider;
    static Texture2D newTexture;
    static Color[] textureColors;
    static SpriteRenderer spriteRend;
    static Color zeroAlpha = Color.clear;
    static int radius;

    public static void UpdateTexture(Vector2 position, GameObject objectToExplode, BoxCollider2D boxCol, int explosionRadius)
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

        UpdatePixels();
    }
    
    static void UpdatePixels()
    {
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
        #region UpdatePixels
        newTexture.SetPixels(textureColors);
        newTexture.Apply();
        #endregion
        #region Create sprite && reAdd Polygon collider
        spriteRend.sprite = Sprite.Create(newTexture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f), spriteRend.sprite.pixelsPerUnit);
        Destroy(spriteRend.gameObject.GetComponent<PolygonCollider2D>());
        spriteRend.gameObject.AddComponent<PolygonCollider2D>();
        #endregion
        
    }
}
