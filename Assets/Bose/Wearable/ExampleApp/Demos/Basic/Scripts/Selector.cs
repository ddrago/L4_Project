using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.IO;
using UnityEngine;

public class Selector : MonoBehaviour
{

    [SerializeField]
    private static Ray _ray;

    private static RaycastHit _hitInfo;
    private static bool _firstContact;

    public static void Press()
    {
        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            string objectName = _hitInfo.collider.gameObject.name;
            if (objectName == "Item1")
            {

                FindObjectOfType<AudioManager>().Play("MenuButtonPress");
                //Debug.Log(_hitInfo.collider.gameObject.name);
                //Debug.Log(DateTime.Now);
                WriteString(DateTime.Now + ": " + _hitInfo.collider.gameObject.name);

            } else if (objectName == "Item2")
            {
                FindObjectOfType<AudioManager>().Play("MenuButtonPress");
                //Debug.Log(_hitInfo.collider.gameObject.name);
                //Debug.Log(DateTime.Now);
                WriteString(DateTime.Now + ": " + _hitInfo.collider.gameObject.name);

            } else
            {
                Debug.Log("New phone, who dis");
            }
        }
        else
        {
            Debug.Log("Oh no! I'm nodding at nothing!");
        }
    }

    public static void WriteString(string log)
    {
        string path = "C:/Users/Edune/Desktop/STUDY2021/IND_PROJ/Testing/Unity_testing/LoggingFolder" + "/test.txt";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(log);
        writer.Close();
        StreamReader reader = new StreamReader(path);
        //Print the text from the file
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

    private void Awake()
    {
        _firstContact = true;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        _ray = ray;

        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            Debug.DrawLine(_ray.origin, _hitInfo.point, Color.green);
            if (_firstContact)
            {
                FindObjectOfType<AudioManager>().Play("MenuSelectionChange");
                _firstContact = false;
            }
        }
        else
        {
            Debug.DrawLine(_ray.origin, _ray.origin + _ray.direction * 100, Color.red);
            _firstContact = true;
        }
    }
}
