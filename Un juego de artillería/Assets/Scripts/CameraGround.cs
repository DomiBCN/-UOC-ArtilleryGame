using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGround : MonoBehaviour
{
    static CameraGround instance;

    Camera groundCam;
    bool takeScreenshot;

    static GameObject objectToUpdate;
    static SpriteRenderer spriteRend;

    private void Awake()
    {
        instance = this;
        groundCam = gameObject.GetComponent<Camera>();
    }
    
    private void OnPostRender()
    {
        if (takeScreenshot)
        {
            takeScreenshot = false;
            RenderTexture renderTexture = groundCam.targetTexture;

            Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);

            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            image.ReadPixels(rect, 0, 0);
            image.Apply();

            Sprite newSprite = Sprite.Create(image, rect, new Vector2(0.5f, 0.5f), spriteRend.sprite.pixelsPerUnit);
            
            
            spriteRend.sprite = newSprite;
            Destroy(objectToUpdate.GetComponent<PolygonCollider2D>());
            objectToUpdate.AddComponent<PolygonCollider2D>();
            
            RenderTexture.ReleaseTemporary(renderTexture);
            groundCam.targetTexture = null;
            objectToUpdate.layer = LayerMask.NameToLayer("Ground");


            byte[] byteArray = image.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/CameraScreenShoot.png", byteArray);
        }
    }

    void TakeScreenShoot(int width, int height)
    {
        groundCam.targetTexture = RenderTexture.GetTemporary(width, height, 24);
        takeScreenshot = true;
    }

    public static void TakeScreenShoot_Static(int width, int height, GameObject objectCollided)
    {
        objectToUpdate = objectCollided;
        objectToUpdate.layer = LayerMask.NameToLayer("GroundScreenShot");
        spriteRend = objectCollided.GetComponent<SpriteRenderer>();
        
        //adjust camera to sprite position and size
        instance.transform.position = new Vector3(spriteRend.transform.position.x, spriteRend.transform.position.y, instance.groundCam.transform.position.z);
        instance.groundCam.orthographicSize = System.Convert.ToSingle(spriteRend.sprite.bounds.size.x * instance.groundCam.pixelHeight / instance.groundCam.pixelWidth * 0.5);
        
        instance.TakeScreenShoot(instance.groundCam.pixelWidth, instance.groundCam.pixelHeight);
    }
}
