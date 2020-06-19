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

    }
    // void Funktionsname(Typ Übergabewert, Typ Übergabewert, ...)


    void ToggleObjects()
    {
        //Gameobjekt ein/ausschalten bei Start:
        cube.SetActive(showCube);
        capsule.SetActive(showCapsule);
        sphere.SetActive(showSphere);
        cylinder.SetActive(showCylinder);
    }
}