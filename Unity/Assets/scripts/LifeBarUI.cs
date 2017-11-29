using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBarUI : MonoBehaviour
{
    public RectTransform maskBar;
    public RectTransform currentLifeBar;

    private float maxWidth;
    private int maxLife;
    private int currentLife;



    // Use this for initialization
    public void Init(int life)
    {
        maxWidth = maskBar.sizeDelta.x;
        maxLife = life;
        UpdateBar(maxLife);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateBar(int newlife)
    {
        currentLife = newlife;
        float newWidth = (maxWidth * currentLife) / maxLife;
        currentLifeBar.sizeDelta = new Vector2(newWidth, currentLifeBar.sizeDelta.y);
    }
}
