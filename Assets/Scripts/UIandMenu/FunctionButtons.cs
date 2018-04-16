using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunctionButtons : MonoBehaviour
{
    public Text myText;

    public void ChangeTextColor()
    {
        myText.color = Color.blue;
    }
    public void ChangeTextColor2()
    {
        myText.color = Color.gray;
    }
}
