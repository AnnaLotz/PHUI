using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    //Objekte Global anlegen, in unity von Hand in den verweis ziehen
    public GameObject cube;
    public GameObject capsule;
    public GameObject sphere;
    public GameObject cylinder;
    public GameObject rect;
    public GameObject longCylinder;

    //Liste, in der alle Totems rein kommen
    public List<Totem> totemsList = new List<Totem>();

    public int activeSide; //von 1-6


    // Start is called before the first frame update
    void Start()
    {
        //alle gameObjects in die liste schieben
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
            gameobj.isActiveBool = false;
            if (gameobj.cubeSide == activeSide)
                gameobj.isActiveBool = true;
            gameobj.ToggleVisibility();
        }
    }



    //**********************************************************************************************
    public class Totem 
    {
        public GameObject gameObject;
        public bool isActiveBool;
        public float visibilityPercentage;
        public int cubeSide;

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