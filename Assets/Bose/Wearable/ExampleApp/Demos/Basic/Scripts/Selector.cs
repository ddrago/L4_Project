using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.IO;

public class Selector : MonoBehaviour
{

    [SerializeField]
    private static Ray _ray;

    private static RaycastHit _hitInfo;

    private static bool _firstContact;
    private static string _firstContactName;

    private static string filename = "log.csv";

    public static void Press()
    {
        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            string objectName = _hitInfo.collider.gameObject.name;

            FindObjectOfType<AudioManager>().Play("MenuButtonPress");
            LogOnCSV("[PRESS]", DateTime.Now.ToString(), DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, _hitInfo.collider.gameObject.name);
        }
        else
        {
            LogOnCSV("[PRESS]", DateTime.Now.ToString(), DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, "N/A");
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

    public static void InitLogging()
    {
        filename = Application.persistentDataPath + "/" + filename;

        System.IO.FileInfo theSourceFile = new System.IO.FileInfo(filename);
        //System.IO.File.WriteAllText(filename, "[TEST]" + filename);
        System.IO.File.WriteAllLines(filename, new string[] {
            "InteractionType,Time,TimeMS,Item",
            "[START]" + "," + DateTime.Now.ToString() + "," + DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + "," + "N/A"
        });
        if (System.IO.File.Exists(filename))
        {
            System.IO.StreamReader reader = theSourceFile.OpenText();
            string text = reader.ReadLine();
            print(text);
        }
    }

    public static void LogOnCSV(string interactionType, string time, long timeMS, string item)
    {
        System.IO.File.AppendAllLines(filename, new string[] {
            interactionType + "," + time + "," + (timeMS).ToString() + "," + item
        });
    }

    private void Awake()
    {
        _firstContact = true;
        _firstContactName = "";

        //Set the path to the logging file as the persistent data folders in the android system
        InitLogging();

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

            if (_firstContact || (_hitInfo.collider.gameObject.name != _firstContactName))
            {
                //HERE HAPPENS WHEN THE USER HOVERS WITH THEIR GAZE UPON SOMETHING IN THE SCENE

                // Get the renderer of the object hit by the raycasting
                Renderer target_renderer = _hitInfo.collider.gameObject.GetComponent<Renderer>();
                target_renderer.material.color = new Color(0, 255, 0);

                FindObjectOfType<AudioManager>().Play("MenuSelectionChange");
                print("SELECTION");
                LogOnCSV("[SELECTION]", DateTime.Now.ToString(), DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond, _hitInfo.collider.gameObject.name);

                string objectName = _hitInfo.collider.gameObject.name;
                if (objectName == "Item1")
                {
                    FindObjectOfType<AudioManager>().Play("Test1");
                }
                else if (objectName == "Item2")
                {
                    FindObjectOfType<AudioManager>().Play("Test2");
                }

                // Reset/update the flags
                _firstContact = false;
                _firstContactName = _hitInfo.collider.gameObject.name;

            }
        }
        else
        {
            Debug.DrawLine(_ray.origin, _ray.origin + _ray.direction * 100, Color.red);
            _firstContact = true;
        }
    }
}
