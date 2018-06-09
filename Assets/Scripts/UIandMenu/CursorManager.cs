using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour {

    public List<CursorAndName> cursorTypes;
    public static Dictionary<string, Texture2D> cursorTextures = new Dictionary<string, Texture2D>();

    private void Start()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);  //Set cursor to default

        foreach(CursorAndName cursor in cursorTypes)
        {
            cursorTextures.Add(cursor.name, cursor.texture);
        }

        cursorTypes.Clear();
    }

    public static Texture2D GetCursorTexture (string cursorName)
    {
        return cursorTextures[cursorName];
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
