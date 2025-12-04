using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContarPixels : MonoBehaviour
{
    public Camera minhaCamera;
    public RenderTexture renderTexture;

    void Start()
    {
        // Garante que a câmera renderize
        StartCoroutine(ContarPixelsDepoisDoFrame());
    }

    private System.Collections.IEnumerator ContarPixelsDepoisDoFrame()
    {
        // Espera o próximo frame para garantir que a RenderTexture foi preenchida
        yield return new WaitForEndOfFrame();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();

        Color32[] pixels = tex.GetPixels32();
        int contagemPixelsAtivos = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 0) // conta pixels que não são transparentes
                contagemPixelsAtivos++;
        }

        Debug.Log("Pixels ocupados na cena: " + contagemPixelsAtivos);

        RenderTexture.active = currentRT;
    }
}

