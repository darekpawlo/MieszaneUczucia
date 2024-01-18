using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ImageManager
{
    private static string basePath;

    public static void SaveImage(string name, byte[] bytes)
    {
        File.WriteAllBytes(basePath + name, bytes);
    }

    public static byte[] LoadImage(string name)
    {
        basePath = Application.persistentDataPath + "/Images/";
        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

        byte[] bytes = new byte[0];
        if(File.Exists(basePath + name))
        {
            bytes = File.ReadAllBytes(basePath + name);
        }
        return bytes;
    }

    public static Sprite BytesToSprite(byte[] bytes)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public static Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public static byte[] TextureToByte(Texture2D texture)
    {        
        return texture.EncodeToPNG();
    }

    public static string TextureToString(Texture2D texture)
    {
        return Convert.ToBase64String(TextureToByte(texture));
    }

    public static Texture2D CreateReadableTexture(Texture2D original)
    {
        // Stworzenie nowej tekstury czytelnej
        Texture2D readableTexture = new Texture2D(original.width, original.height);
        RenderTexture currentRT = RenderTexture.active;

        // Stworzenie tymczasowego RenderTexture i kopiowanie oryginalnej tekstury do niego
        RenderTexture tempRT = RenderTexture.GetTemporary(original.width, original.height);
        Graphics.Blit(original, tempRT);
        RenderTexture.active = tempRT;

        // Kopiowanie pikseli z RenderTexture do nowej, czytelnej tekstury
        readableTexture.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(tempRT);

        return readableTexture;
    }

    public static Texture2D SpriteToTexture(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                        (int)sprite.textureRect.y,
                                                        (int)sprite.textureRect.width,
                                                        (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }

}
