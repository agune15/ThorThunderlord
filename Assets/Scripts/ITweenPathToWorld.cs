using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(iTweenPath))]
public class ITweenPathToWorld : MonoBehaviour {

    public List<Vector3> pathPoints = new List<Vector3>();
    Vector3 maxVector = Vector3.zero;
    Vector3 minVector = Vector3.zero;

	// Use this for initialization
	void Start () {
        //encontrar centro entre puntos y calcular el offset de cada uno respecto a este
        foreach (Vector3 point in iTweenPath.GetPath("lightBolt_aroundHammer_path"))
        {
            if (point.x > maxVector.x) maxVector.x = point.x;
            if (point.x < minVector.x) minVector.x = point.x;

            if (point.y > maxVector.y) maxVector.y = point.y;
            if (point.y < minVector.y) minVector.y = point.y;

            if (point.z > maxVector.z) maxVector.z = point.z;
            if (point.z < minVector.z) minVector.z = point.z;
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
