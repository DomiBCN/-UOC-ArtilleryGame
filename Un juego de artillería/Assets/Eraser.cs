using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Eraser : MonoBehaviour {

    private Texture2D m_Texture;
    private Color[] m_Colors;
    RaycastHit2D[] hitAll;
    RaycastHit2D hit;
    SpriteRenderer spriteRend;
    Color zeroAlpha = Color.clear;
    int erSize = 20;
    public Vector2Int lastPos;
    public bool Drawing = false;

    public BoxCollider2D collider;
    void Start()
    {
        spriteRend = gameObject.GetComponent<SpriteRenderer>();
        var tex = spriteRend.sprite.texture;
        m_Texture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        m_Texture.filterMode = FilterMode.Bilinear;
        m_Texture.wrapMode = TextureWrapMode.Clamp;
        m_Colors = tex.GetPixels();
        m_Texture.SetPixels(m_Colors);
        m_Texture.Apply();
        spriteRend.sprite = Sprite.Create(m_Texture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f), spriteRend.sprite.pixelsPerUnit);
    }

    void ExplodeSprite(GameObject objectToExplode)
    {
        spriteRend = objectToExplode.GetComponent<SpriteRenderer>();
        var tex = spriteRend.sprite.texture;
        m_Texture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        m_Texture.filterMode = FilterMode.Bilinear;
        m_Texture.wrapMode = TextureWrapMode.Clamp;
        m_Colors = tex.GetPixels();
        m_Texture.SetPixels(m_Colors);
        m_Texture.Apply();
        spriteRend.sprite = Sprite.Create(m_Texture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f), spriteRend.sprite.pixelsPerUnit);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            hitAll = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            hit = hitAll.ToList().Find(h => h.collider != null && h.collider == collider);
            //hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider == collider)
            {
                UpdateTexture();
                Drawing = true;
            }
        }
        else
            Drawing = false;
    }

    public void UpdateTexture()
    {
        int w = m_Texture.width;
        int h = m_Texture.height;
        var mousePos = hit.point - (Vector2)hit.collider.bounds.min;
        mousePos.x *= w / hit.collider.bounds.size.x;
        mousePos.y *= h / hit.collider.bounds.size.y;
        Vector2Int p = new Vector2Int((int)mousePos.x, (int)mousePos.y);
        Vector2Int start = new Vector2Int();
        Vector2Int end = new Vector2Int();
        if (!Drawing)
            lastPos = p;
        start.x = Mathf.Clamp(Mathf.Min(p.x, lastPos.x) - erSize, 0, w);
        start.y = Mathf.Clamp(Mathf.Min(p.y, lastPos.y) - erSize, 0, h);
        end.x = Mathf.Clamp(Mathf.Max(p.x, lastPos.x) + erSize, 0, w);
        end.y = Mathf.Clamp(Mathf.Max(p.y, lastPos.y) + erSize, 0, h);
        Vector2 dir = p - lastPos;
        for (int x = start.x; x < end.x; x++)
        {
            for (int y = start.y; y < end.y; y++)
            {
                Vector2 pixel = new Vector2(x, y);
                Vector2 linePos = p;
                if (Drawing)
                {
                    float d = Vector2.Dot(pixel - lastPos, dir) / dir.sqrMagnitude;
                    d = Mathf.Clamp01(d);
                    linePos = Vector2.Lerp(lastPos, p, d);
                }
                if ((pixel - linePos).sqrMagnitude <= erSize * erSize)
                {
                    m_Colors[x + y * w] = zeroAlpha;
                }
            }
        }
        lastPos = p;
        m_Texture.SetPixels(m_Colors);
        m_Texture.Apply();
        spriteRend.sprite = Sprite.Create(m_Texture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f), spriteRend.sprite.pixelsPerUnit);
        Destroy(spriteRend.gameObject.GetComponent<PolygonCollider2D>());
        spriteRend.gameObject.AddComponent<PolygonCollider2D>();
    }
}
