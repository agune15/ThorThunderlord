using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectThroughPath : MonoBehaviour {

    public string pathName;
    float travelPercentage = 0;
    public float travelTime;
    [SerializeField] Vector3[] pathPoints;

    private void Start()
    {
        pathPoints = iTweenPath.GetPath(pathName);
    }

    private void Update () {
        if (travelPercentage < 1)
        {
            travelPercentage += travelTime * Time.deltaTime;
        }
        else travelPercentage = 0;

        iTween.PutOnPath(this.gameObject, pathPoints, travelPercentage);
	}
}
