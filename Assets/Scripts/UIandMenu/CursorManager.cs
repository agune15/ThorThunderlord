using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour {

    public List<CursorAndName> cursorTypes;
    public static Dictionary<string, Texture2D> cursorTextures = new Dictionary<string, Texture2D>();
    public static string currentCursorTexture = "default";

    private void Start()
    {
        foreach(CursorAndName cursor in cursorTypes)
        {
            cursorTextures.Add(cursor.name, cursor.texture);
        }

        cursorTypes.Clear();

        SetAndStoreCursor("default", Vector2.zero, CursorMode.Auto);  //Set cursor to default
    }

    public static Texture2D GetCursorTexture (string cursorName)
    {
        return cursorTextures[cursorName];
    }

    //Extension method of Cursor.SetCursor
    public static void SetAndStoreCursor (string cursorTextureName, Vector2 cursorHotSpot, CursorMode cursorMode)
    {
        currentCursorTexture = cursorTextureName;
        Cursor.SetCursor(cursorTextures[cursorTextureName], cursorHotSpot, cursorMode);
    }

    public static string GetCurrentCursorTextureName ()
    {
        return currentCursorTexture;
    }
}

[System.Serializable]
public class CursorAndName {

    public string name;
    public Texture2D texture;

    public CursorAndName (string cursorName, Texture2D cursorTexture)
    {
        name = cursorName;
        texture = cursorTexture;
    }
}
