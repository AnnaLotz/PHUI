using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Range(-90f, 90f)]
    public float gx;

    [Range(-90f, 90f)]
    public float gy;

    [Range(-90f, 90f)]
    public float gz;

    //smooth transition to the new position, higher = fast movement
    float smooth = 20f;

    void Start()
    {

    }

    void Update()
    {
        //slerp ist für eine smoothe übergangsanimation von aktueller zur taget-rotation
        Quaternion target = Quaternion.Euler(gx, gy, gz);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        
    }

}
