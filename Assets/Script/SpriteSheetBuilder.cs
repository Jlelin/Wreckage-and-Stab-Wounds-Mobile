using System.IO;
using System.Linq;
using UnityEngine;

public class SpriteSheetBuilder : MonoBehaviour
{
    [Header("Configurações")]
    public string sourceFolder = "Assets/TerrenoTiles";
    public int tilesPerRow = 32;
    public int tileWidth = 256;
    public int tileHeight = 256;

    [ContextMenu("Gerar SpriteSheet")]
    void GenerateSpriteSheet()
    {
        string[] files = Directory.GetFiles(sourceFolder, "*.png");
        if (files.Length == 0)
        {
            Debug.LogError("Nenhum arquivo .png encontrado na pasta.");
            return;
        }

        int totalTiles = files.Length;
        int rows = Mathf.CeilToInt((float)totalTiles / tilesPerRow);

        Texture2D spriteSheet = new Texture2D(tilesPerRow * tileWidth, rows * tileHeight, TextureFormat.RGBA32, false);

        // Cria um array de tuplas (posição, Texture2D, nome original)
        var tiles = files.Select(f =>
        {
            string fileName = Path.GetFileName(f);
            int pos = 0;
            int idx = fileName.IndexOf("Tile_");
            if (idx >= 0)
            {
                // Pega X e Y do nome Tile_X_Y.png
                string[] parts = Path.GetFileNameWithoutExtension(fileName).Substring(idx + 5).Split('_');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int x) &&
                    int.TryParse(parts[1], out int y))
                {
                    // Calcula a posição linear conforme a ordem de baixo para cima da esquerda para a direita
                    pos = (y * tilesPerRow) + x;
                }
                else
                {
                    Debug.LogWarning($"Não foi possível parsear {f}, usando posição 0");
                }
            }

            byte[] data = File.ReadAllBytes(f);
            Texture2D tex = new Texture2D(tileWidth, tileHeight, TextureFormat.RGBA32, false);
            tex.LoadImage(data);

            return (pos, tex, fileName);
        })
        .OrderBy(t => t.pos)
        .ToArray();

        // Monta o sprite sheet na ordem correta
        for (int i = 0; i < totalTiles; i++)
        {
            int row = i / tilesPerRow;
            int col = i % tilesPerRow;

            int xPos = col * tileWidth;
            int yPos = row * tileHeight;

            Debug.Log($"Tile {tiles[i].fileName} (array index {i}) será colocado em: row={row}, col={col}, xPos={xPos}, yPos={yPos}");

            spriteSheet.SetPixels(xPos, yPos, tileWidth, tileHeight, tiles[i].tex.GetPixels());
        }

        spriteSheet.Apply();

        string savePath = Path.Combine(Application.dataPath, "campodeguerra.png");
        File.WriteAllBytes(savePath, spriteSheet.EncodeToPNG());

        Debug.Log($"SpriteSheet gerado com sucesso em: {savePath}");
    }
}
