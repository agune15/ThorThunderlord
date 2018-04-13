using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticVariables : MonoBehaviour {

    public void ResetAllStaticVariables ()
    {
        PlayingEndMessage.ResetEndMessage();
    }
}
