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

    private GameObject[] shapes;

    //bools um shapes ein/aus zu schalten
    public bool showCube;
    public bool showCapsule;
    public bool showSphere;
    public bool showCylinder;


    public bool parentIsRotate = true;

    //range von 0 bis 90 als speed festlegen
    [Range(0f, 90f)]
    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        showCube = true;
        showCapsule = false;
        showSphere = false;
        showCylinder = false;
        ToggleObjects();

    }

    // Update is called once per frame
    void Update()
    {
        ToggleObjects();

        //Übergabewerte Objekt und Drehgeschwindigkeit
        //f weil float Wert braucht das halt
        Rotate(cube, 10f);
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

    void ToggleObjects()
    {
        //Gameobjekt ein/ausschalten bei Start:
        cube.SetActive(showCube);
        capsule.SetActive(showCapsule);
        sphere.SetActive(showSphere);
        cylinder.SetActive(showCylinder);
    }
}
