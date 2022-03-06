using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using static System.Random;
using System.Linq;
using System.IO;

public class Selector : MonoBehaviour
{

    [SerializeField]
    private static Ray _ray;

    private static RaycastHit _hitInfo;

    //These two help make sure that the selection doesn't repeat every frame
    private static bool _firstContact;
    private static string _firstContactName;

    //The name of the logging file - MODIFY FOR EACH PARTICIPANT
    private static string filename = "log.csv";

    private static Coach coach;

    public void Select()
    {
        // Deal with the user's gaze
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
                LogOnCSV("[SELECTION]", DateTime.Now.ToString(), DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, _hitInfo.collider.gameObject.name);

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

    public static void Press()
    {
        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            string objectName = _hitInfo.collider.gameObject.name;

            FindObjectOfType<AudioManager>().Play("MenuButtonPress");
            LogOnCSV("[PRESS]", DateTime.Now.ToString(), DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, _hitInfo.collider.gameObject.name);
            print("PRESS");

            coach.startCoachCountdown = true;
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

        //Set up the coach
        coach = new Coach(1);
    }

    // Update is called once per frame
    void Update()
    {
        Select();

        coach.afterCountdownCoach();
    }


    //This class give the percitipant timely audio feedback on what menu item to select next.
    public class Coach
    {
        public Boolean startCoachCountdown;
        private static float countdown;
        private static float coachCountdownDuration;

        public static List<string> instructions_to_give = new List<string>(new string[] {"Music", "Music", "News", "News", "Podcasts", "Podcasts", "Sports", "Sports"});
        public static List<string> instructions = new List<string>();

        public static System.Random rnd = new System.Random();


        public Coach(float countdownDuration)
        {
            print("");
            startCoachCountdown = false;
            countdown = coachCountdownDuration;
            coachCountdownDuration = countdownDuration;

            instructions = instructions_to_give.OrderBy(a => rnd.Next()).ToList();
            print(string.Join(",", instructions.ToArray()));
        }

        public string Give_instruction()
        {
            //TODO do something about going over 8 instructions
            string next_instruction = instructions[0];
            instructions.RemoveAt(0);
            return next_instruction;
        }

        public void afterCountdownCoach()
        {
            // Deal with the coach countdown
            if (startCoachCountdown == true)
            {
                if (countdown > 0)
                {
                    countdown -= Time.deltaTime;
                }
                else
                {
                    print("NOW PRESS AUDIO");

                    FindObjectOfType<AudioManager>().Play("NowPress");
                    FindObjectOfType<AudioManager>().PlayAfter("Music", 1);
                    countdown = coachCountdownDuration;
                    startCoachCountdown = false;
                }
            }
        }
    }

}
