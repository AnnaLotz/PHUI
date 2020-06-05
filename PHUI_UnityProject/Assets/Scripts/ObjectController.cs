using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    //Objekte Global anlegen, in unity von Hand in den verweis ziehen
    public GameObject cube;
    public GameObject capsule;
    public GameObject sphere;
    public GameObject cylinder;

    public bool parentIsRotate = true;

    //range von 0 bis 90 als speed festlegen
    [Range(0f, 90f)]
    public float speed;

    private Transform cubeTransform;

    // Start is called before the first frame update
    void Start()
    {
        //Gameobjekt ein/ausschalten bei Start:
        capsule.SetActive(true);

        // transformComponent ist standartmäßig drin, alle anderen muss man sich manuell holen
        cubeTransform = cube.transform;

    }



    // Update is called once per frame
    void Update()
    {
        //Übergabewerte Objekt und Drehgeschwindigkeit
        //f weil float Wert braucht das halt
        //Rotate(cube, 10f);
        Rotate(capsule, 20f);
        Rotate(sphere, 30f);
        Rotate(cylinder, 50f);

        if (parentIsRotate)
            Rotate(transform.gameObject, speed);
    }

    // void Funktionsname(Typ Übergabewert, Typ Übergabewert, ...)
    void Rotate(GameObject trans, float time)
    {
        //rotation abhängig vom aktuellen Transform des Objekts
        trans.transform.RotateAround(trans.transform.position, trans.transform.up, Time.deltaTime * time);
    }
}
