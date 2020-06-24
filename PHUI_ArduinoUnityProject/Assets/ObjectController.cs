using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    //variable die den Zustand steuert
    public bool dreaming = false;

    //Objekte Global anlegen, in unity von Hand in den verweis ziehen
    public GameObject cube;
    public GameObject capsule;
    public GameObject sphere;
    public GameObject cylinder;
    public GameObject rect;
    public GameObject longCylinder;

    //Liste, in der alle Totems rein kommen (ähnlich Array oder Interface in typescript). Enthält Instanzen der Klasse Totem (siehe weiter unten)
    public List<Totem> totemsList = new List<Totem>();

    //Zahl, welche Seite gerade nach vorne zeigt
    public int activeSide; //von 1-6


    // Start is called before the first frame update
    void Start()
    {
        //Objekte nach der Klasse Totem erstellen und in die Totemliste schieben
        totemsList.Add(new Totem(cube, false, 0, 1));
        totemsList.Add(new Totem(capsule, false, 0, 2));
        totemsList.Add(new Totem(sphere, false, 0, 3));
        totemsList.Add(new Totem(cylinder, false, 0, 4));
        totemsList.Add(new Totem(rect, false, 0, 5));
        totemsList.Add(new Totem(longCylinder, false, 0, 6));

        //durch die liste aller Totems iterieren
        foreach (Totem gameobj in totemsList) 
        {
            Debug.Log(gameobj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        FindActiveSide();
        ToggleObjects();
    }

    //wird im RecieveIMUValues Script aufgerufen, hier werden die Gyro-Daten bearbeitet
    public void handleIMUData(float _x, float _y,float _z, float _w, float _speedFactor) 
    {
        if (!dreaming)
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, new Quaternion(_x, _y, _z, _w), Time.deltaTime * _speedFactor);
    }
   

    public void FindActiveSide() {
        //Berechne winkel zwischen lokalem Vektor und globalen up-Vektor
        float angle1 = Vector3.Angle(transform.up, Vector3.right);
        float angle2 = Vector3.Angle(transform.right, Vector3.right);
        float angle3 = Vector3.Angle(transform.forward, Vector3.right);
        float angle4 = Vector3.Angle(- transform.up, Vector3.right);
        float angle5 = Vector3.Angle(- transform.right, Vector3.right);
        float angle6 = Vector3.Angle(- transform.forward, Vector3.right);
        float[] angles = { angle1, angle2, angle3, angle4, angle5, angle6 };
        
        //wenn der winkel zwischen -45 und 45 liegt, ist das die front-facing-side
        for (int i = 0; i < angles.Length; i++)
        {           
            if (angles[i] < 45 && angles[i] > -45)
            {
                activeSide = i + 1;     
            }
        }
        Debug.Log(activeSide + " is ActiveSide");
    }

    void ToggleObjects()
    {
        //Gameobjekt ein/ausschalten:
        foreach (Totem gameobj in totemsList)
        {
            //erstmal alle ausschalten
            gameobj.isActiveBool = false;
            //das Objekt, welches mit der Zahl von activeSide übereinstimmt, aktivieren/anzeigen
            if (gameobj.cubeSide == activeSide)
                gameobj.isActiveBool = true;
            gameobj.ToggleVisibility(); //Aufruf einer Methode in der Klasse
        }
    }



    //**********************************************************************************************
    public class Totem 
    {
        public GameObject gameObject; //welches gameObject liegt in der Klasse, also welches Totem
        public bool isActiveBool; //Objekt an oder aus
        public float visibilityPercentage; //Prozent, wie stark das Objekt nach oben zeigt
        public int cubeSide; //zu welcher Seitenzahl gehört das Objekt

        //constructor in Unity:
        public Totem(GameObject _gameObject, bool _isActiveBool, float _visibilityPercentage, int _cubeSide)
        {
            gameObject = _gameObject;
            isActiveBool = _isActiveBool;
            visibilityPercentage = _visibilityPercentage;
            cubeSide = _cubeSide;
        }

        public void ToggleVisibility() 
        {
            gameObject.SetActive(isActiveBool);
        }
    }
    


}