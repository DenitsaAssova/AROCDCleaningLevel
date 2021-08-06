using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Preprocess : MonoBehaviour
{
   // public TextMeshProUGUI tester;
    RenderTexture renderTexture;
    Vector2 scale = new Vector2(1, 1);
    Vector2 offset = Vector2.zero;
   
    private bool called = false;
    UnityAction<byte[]> callback;
   // Texture2D texture;
    public void ScaleAndCropImage(WebCamTexture webCamTexture, int desiredSize, UnityAction<byte[]> callback)
    {
        called = true;
       // tester.text = "SCale entered";
        this.callback = callback;

        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(desiredSize, desiredSize, 0, RenderTextureFormat.ARGB32);
        }
       // tester.text = "after if for render textture";
        scale.x = (float)webCamTexture.height / (float)webCamTexture.width;
        offset.x = (1 - scale.x) / 2f;
        Graphics.Blit(webCamTexture, renderTexture, scale, offset); //crops 
       // tester.text = "after blit";
        Texture2D texture; // = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        texture = GetRTPixels(renderTexture);
        texture.Apply();
        callback.Invoke(texture.GetRawTextureData<byte>().ToArray());

        // texture.Apply();
        // request = UniversalAsyncGPUReadbackRequest.Request(renderTexture);
        // OnCompleteReadback(request);
        //AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnCompleteReadback);
    }

    /*private void Update()
    {
        if (called == true)
        {
            tester.text = "after blitOnCompleteReadbacl entered";
            if (request.done)
            {
                if (request.hasError)
                {
                    tester.text = "request has error";
                    Debug.Log("GPU readback error detected.");
                    return;
                }
                else
                {
                    foreach (var item in request.GetData<byte>())
                    {
                        Debug.Log(item);
                    }
                    callback.Invoke(request.GetData<byte>().ToArray());
                }
            }
           // called = false;
        }
    }*/
   
    /* void OnCompleteReadback(AsyncGPUReadbackRequest request)
     {
         tester.text = "after blitOnCompleteReadbacl entered";
         if (request.hasError)
         {
             tester.text = "request has error";
             Debug.Log("GPU readback error detected.");
             return;
         }

         callback.Invoke(request.GetData<byte>().ToArray());
     }*/

  

    static public Texture2D GetRTPixels(RenderTexture rt)
    {
        // Remember currently active render texture
        RenderTexture currentActiveRT = RenderTexture.active;

        // Set the supplied RenderTexture as the active one
        RenderTexture.active = rt;
        
        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, rt.mipmapCount > 1);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

        // Restorie previously active render texture
        RenderTexture.active = currentActiveRT;
        return tex;
    }
}
