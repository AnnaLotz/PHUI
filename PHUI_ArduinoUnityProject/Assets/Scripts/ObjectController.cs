using UnityEngine;
using Uduino;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.VFX;
using JetBrains.Annotations;

public class ObjectController : MonoBehaviour
{
    //variable die den Zustand steuert
    public bool dreaming = true;

    //Objekte Global anlegen, in unity von Hand in den verweis ziehen
    public GameObject dice;
    public GameObject chip;
    public GameObject marble;
    public GameObject bishop;
    public GameObject pin;
    public GameObject spinningTop;
    
    //Liste einfach nur mit den GameObjects. Per Hand im Inspector verbunden
    public List<GameObject> totemObjects;
    //Liste, in der alle Totems rein kommen (ähnlich Array oder Interface in typescript). Enthält Instanzen der Klasse Totem (siehe weiter unten)
    public List<Totem> totemsList = new List<Totem>();

    //Zahl, welche Seite gerade nach vorne zeigt
    public int activeSide; //von 1-6
    //hiermit kann man steuern, wie schnell das Morphing beginnt
    public float angleSteps = 90f;

    //für den VisualEffectGraph
    public List<VisualEffect> myEffect;

    //Referenzvektor für FindActiveSide(), der beim Zustandswechel veränderbar ist. Die Winkel werden global gespeichert
    public Vector3 referenceVector = Vector3.right;
    public float[] angles;
    

    void Awake()
    {
        UduinoManager.Instance.OnDataReceived += OnDataReceived; //Create the Delegate
        UduinoManager.Instance.alwaysRead = true; // This value should be On By Default
    }

    // Start is called before the first frame update
    void Start()
    {
        angles = new float[6];
        //Objekte nach der Klasse Totem erstellen und in die Totemliste schieben
        totemsList.Add(new Totem(dice, false, 0, 1));
        totemsList.Add(new Totem(chip, false, 0, 2));
        totemsList.Add(new Totem(marble, false, 0, 3));
        totemsList.Add(new Totem(bishop, false, 0, 4));
        totemsList.Add(new Totem(pin, false, 0, 5));
        totemsList.Add(new Totem(spinningTop, false, 0, 6));

        FindActiveSide();
        /*
        //durch die liste aller Totems iterieren
        foreach (Totem gameobj in totemsList) 
        {
            Debug.Log(gameobj);
        }
        */
        dreaming = false; //damit man im Realitätsmodus startet
    }

    // Update is called once per frame
    void Update()
    {
        
        if (dreaming)
        {
            FindActiveSide();
            ToggleObjectsMorphing();
        }
        else 
        {
            ToggleObjectsRotatingOnly();
        }


    }
    void OnDataReceived(string data, UduinoDevice deviceName)
    {
        if (data == "1")
        {
            //kleinsten Winkel im anglesArray finden, die entsprechende Seite wird im switch als Bezugsvektor gesetzt
            for (int i = 0; i < angles.Length; i++) 
            {
                if (angles[i] == Mathf.Min(angles))
                {
                    switch (i)
                    {
                        case 0:
                            referenceVector = transform.up;
                            break;
                        case 1:
                            referenceVector = transform.right;
                            break;
                        case 2:
                            referenceVector = transform.forward;
                            break;
                        case 3:
                            referenceVector = - transform.up;
                            break;
                        case 4:
                            referenceVector = - transform.right;
                            break;
                        case 5:
                            referenceVector = - transform.forward;
                            break;
                        default:
                            referenceVector = transform.up;
                            Debug.Log("Hier ist default case");
                            break;
                    }
                }
            }

            if (dreaming == false) //wechsel zu traum
            {
                FindActiveSide();
                dreaming = true;
                
            }
            else //wechsel zu realität
            {              
                dreaming = false;
            }            
        }
    }

    //wird im RecieveIMUValues Script aufgerufen, hier werden die Gyro-Daten bearbeitet
    public void handleIMUData(float _x, float _y,float _z, float _w, float _speedFactor) 
    {
        this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, new Quaternion(_x, _y, _z, _w), Time.deltaTime * _speedFactor); //objekt-rotation

    }
   

    public void FindActiveSide() {
        //Berechne winkel zwischen lokalem Vektor und Referenzvektor
        angles[0] = Vector3.Angle(transform.up, referenceVector);
        angles[1] = Vector3.Angle(transform.right, referenceVector);
        angles[2] = Vector3.Angle(transform.forward, referenceVector);
        angles[3] = Vector3.Angle(- transform.up, referenceVector);
        angles[4] = Vector3.Angle(- transform.right, referenceVector);
        angles[5] = Vector3.Angle(- transform.forward, referenceVector);

            
        //wenn der winkel zwischen -45 und 45 liegt, ist das die front-facing-side
        for (int i = 0; i < angles.Length; i++)
        {
            if (angles[i] < 45 && angles[i] > -45)
            {
                activeSide = i; //nur die Variable beschreiben
            }

            if (dreaming)
            {
                if (angles[i] < angleSteps && angles[i] > -angleSteps)
                {
                    totemsList[i].visibilityPercentage = 1 - (Math.Abs(angles[i]) / angleSteps); //sichtbarkeits-Wert berechnen
                }
                else
                {
                    totemsList[i].visibilityPercentage = 0; //wenn eh nicht sichtbar, zu 0% sichtbar sein
                }
            }

        }
    }

    
    void ToggleObjectsMorphing()
    {
        int index = 0; //eigener index für die forEach-Schleife

        //durch alle objekte in der totemListe durch iterieren
        foreach (Totem gameobj in totemsList) 
        {
            Vector3 scale = Vector3.Lerp(Vector3.zero, gameobj.initialScale, gameobj.visibilityPercentage); //skalierungsvektor erstellen, beachtet die ursprüungliche Skalierung
            totemObjects[index].transform.localScale = scale; //skalierung des aktuellen Totems auf den neu berechneten Scale bringen
            myEffect[index].SetFloat("DreamingColorValue", 0); //für den VisualEffectGraph: dort als Variable nutzbar            

            index++;
        }
    }

    void ToggleObjectsRotatingOnly()
    {
        int index = 0; //eigener index für die forEach-Schleife
        //Debug.Log(activeSide);
        foreach (Totem gameobj in totemsList)
        {

            if (index == (activeSide))
            {                
                Vector3 scale = Vector3.Lerp(Vector3.zero, gameobj.initialScale, gameobj.visibilityPercentage); //skalierungsvektor erstellen, beachtet die ursprüungliche Skalierung
                totemObjects[index].transform.localScale = scale; //skalierung des aktuellen Totems auf den neu berechneten Scale bringen
                myEffect[index].SetFloat("DreamingColorValue", 1); //für den VisualEffectGraph: dort als Variable nutzbar 
            }
            else 
            {
                Vector3 scale = Vector3.Lerp(Vector3.zero, gameobj.initialScale, 0); //skalierungsvektor erstellen, beachtet die ursprüungliche Skalierung
                totemObjects[index].transform.localScale = scale; //skalierung des aktuellen Totems auf den neu berechneten Scale bringen
            }
                       

            /*
            //erstmal alle ausschalten
            gameobj.isActiveBool = false;
            //das Objekt, welches mit der Zahl von activeSide übereinstimmt, aktivieren/anzeigen
            if (gameobj.cubeSide == activeSide)
                gameobj.isActiveBool = true;
            gameobj.ToggleVisibility();
            */
            index++;
        }
    }


    //**********************************************************************************************
    
    public class Totem 
    {
        public GameObject gameObject; //welches gameObject liegt in der Klasse, also welches Totem
        public bool isActiveBool; //Objekt an oder aus
        public float visibilityPercentage; //Prozent, wie stark das Objekt nach oben zeigt
        public int cubeSide; //zu welcher Seitenzahl gehört das Objekt
        public Vector3 initialScale; //ursprüngliche Skalierung speichern, wird im constructor gesetzt

        //constructor in Unity:
        public Totem(GameObject _gameObject, bool _isActiveBool, float _visibilityPercentage, int _cubeSide)
        {
            gameObject = _gameObject;
            isActiveBool = _isActiveBool;
            visibilityPercentage = _visibilityPercentage;
            cubeSide = _cubeSide;
            initialScale = gameObject.transform.localScale;
        }

        /*
        public void ToggleVisibility()
        {
            gameObject.SetActive(isActiveBool);
        }
        */

    }
    


}