using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveRenderTextureTiles : MonoBehaviour
{
    public RenderTexture renderTexture;
    public int tileSize = 4096; // ajuste de acordo com seu PC
    public string outputFolder = "TerrenoTiles";

    [ContextMenu("Salvar RenderTexture em Tiles")]
    void SaveTextureInTiles()
    {
        // Cria a pasta se não existir
        string folderPath = Path.Combine(Application.dataPath, outputFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        int width = renderTexture.width;
        int height = renderTexture.height;

        int xTiles = Mathf.CeilToInt((float)width / tileSize);
        int yTiles = Mathf.CeilToInt((float)height / tileSize);

        Debug.Log($"RenderTexture {width}x{height} será dividida em {xTiles}x{yTiles} tiles de até {tileSize}x{tileSize}.");

        for (int y = 0; y < yTiles; y++)
        {
            for (int x = 0; x < xTiles; x++)
            {
                int currentWidth = Mathf.Min(tileSize, width - x * tileSize);
                int currentHeight = Mathf.Min(tileSize, height - y * tileSize);

                Texture2D tileTexture = new Texture2D(currentWidth, currentHeight, TextureFormat.RGBA32, false);

                // Lê a área do tile
                tileTexture.ReadPixels(
                    new Rect(x * tileSize, y * tileSize, currentWidth, currentHeight),
                    0, 0
                );
                tileTexture.Apply();

                byte[] bytes = tileTexture.EncodeToPNG();
                string fileName = Path.Combine(folderPath, $"Tile_{x}_{y}.png");
                File.WriteAllBytes(fileName, bytes);
            }
        }

        RenderTexture.active = currentRT;
        Debug.Log($"Todos os tiles salvos em: {folderPath}");
    }
}
