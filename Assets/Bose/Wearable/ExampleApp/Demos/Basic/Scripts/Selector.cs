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
                LogOnCSV("[PRESS]", DateTime.Now.ToString(), DateTime.UtcNow.Millisecond.ToString(), _hitInfo.collider.gameObject.name);

                //WriteString("[PRESS] " + DateTime.Now + ": " + _hitInfo.collider.gameObject.name);

            } else if (objectName == "Item2")
            {
                FindObjectOfType<AudioManager>().Play("MenuButtonPress");
                LogOnCSV("[PRESS]", DateTime.Now.ToString(), DateTime.UtcNow.Millisecond.ToString(), _hitInfo.collider.gameObject.name);

                //WriteString("[PRESS] " + DateTime.Now + ": " + _hitInfo.collider.gameObject.name);

            }
            else
            {
                LogOnCSV("[PRESS]", DateTime.Now.ToString(), DateTime.UtcNow.Millisecond.ToString(), "N/A");
                //WriteString("[VOID PRESS] " + DateTime.Now);
            }
        }
        else
        {
            LogOnCSV("[PRESS]", DateTime.Now.ToString(), DateTime.UtcNow.Millisecond.ToString(), "N/A");
            //WriteString("[VOID PRESS] " + DateTime.Now);
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

    public static void LogOnCSV(string interactionType, string time, string timeMS, string item)
    {
        string path = "C:/Users/Edune/Desktop/STUDY2021/IND_PROJ/Testing/Unity_testing/LoggingFolder" + "/log.csv";
        StreamWriter writer = new StreamWriter(path, append: true);

        writer.WriteLine(interactionType + "," + time + "," + timeMS + "," + item);
        //writer.Flush();
        writer.Close();
    }

    private void Awake()
    {
        _firstContact = true;

        // Set up the csv file. This should only be done once per file. However, if the file already exists, the code below
        // will just print onto it a new line. 
        // TODO: check the names of all the files that starts with "log" and iterate until you find an available name
        string path = "C:/Users/Edune/Desktop/STUDY2021/IND_PROJ/Testing/Unity_testing/LoggingFolder" + "/log.csv";
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine("InteractionType,Time,TimeMS,Item");
        writer.WriteLine("[START]" + "," + DateTime.Now.ToString() + "," + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString() + "," + "N/A");
        //writer.Flush();
        writer.Close();
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
                LogOnCSV("[SELECTION]", DateTime.Now.ToString(), (DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond).ToString(), _hitInfo.collider.gameObject.name);
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
