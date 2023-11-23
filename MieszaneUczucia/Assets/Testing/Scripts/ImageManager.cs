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
}
