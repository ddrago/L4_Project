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

    //This makes sure the Selector class can identify the start of the experiment
    private static Boolean isExperimentStarted;

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

                // There is an object positioned at the center of a participant's point of view that should 
                // Allow to just look without being bombarded with feedback.
                if (_hitInfo.collider.gameObject.name != "NoFeedbackZone")
                {
                    FindObjectOfType<AudioManager>().Play("MenuSelectionChange");
                    FindObjectOfType<AudioManager>().Play(_hitInfo.collider.gameObject.name);
                }

                //print("SELECTION");
                LogOnCSV("[SELECTION]", DateTime.Now.ToString(), DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, _hitInfo.collider.gameObject.name);

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
        //The experiment starts when the first press is done!
        if (isExperimentStarted == false)
        {
            isExperimentStarted = true;
            FindObjectOfType<AudioManager>().Play("MenuButtonPress");
            LogOnCSV("[STARTING PRESS]", DateTime.Now.ToString(), DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, "N/A");
            coach.startCoachCountdown = true;

            return;
        }

        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            FindObjectOfType<AudioManager>().Play("MenuButtonPress");
            LogOnCSV("[PRESS]", DateTime.Now.ToString(), DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, _hitInfo.collider.gameObject.name);
            //print("PRESS");

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

        //The experiment has not started yet
        isExperimentStarted = false;
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

        public static List<string> instructions_to_give = new List<string>(new string[] { "Music", "News", "Podcasts", "Sports", "Music", "News", "Podcasts", "Sports" });
        public static List<string> instructions = new List<string>();

        public static System.Random rnd = new System.Random();

        public static string instruction_log_filename = "instructions.txt";


        public Coach(float countdownDuration)
        {
            startCoachCountdown = false;
            countdown = coachCountdownDuration;
            coachCountdownDuration = countdownDuration;

            instructions = instructions_to_give.OrderBy(a => rnd.Next()).ToList();
            //print(string.Join(",", instructions.ToArray()));

            LogInstructions();
        }

        public void LogInstructions()
        {
            instruction_log_filename = Application.persistentDataPath + "/" + instruction_log_filename;

            System.IO.FileInfo theSourceFile = new System.IO.FileInfo(instruction_log_filename);
            System.IO.File.WriteAllText(instruction_log_filename, string.Join(",", instructions.ToArray()));
            if (System.IO.File.Exists(instruction_log_filename))
            {
                System.IO.StreamReader reader = theSourceFile.OpenText();
                string text = reader.ReadLine();
                print(text);
            }
        }

        public string Give_instruction()
        {
            int c = instructions.Count();
            if ( c > 0) 
            {
                string next_instruction = instructions[0];
                instructions.RemoveAt(0);
                return next_instruction;
            }
            else
            {
                return null;
            }
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
                    string next_instruction = Give_instruction();
                    if (next_instruction is not null)
                    {
                        FindObjectOfType<AudioManager>().Play("NowPress");
                        FindObjectOfType<AudioManager>().PlayAfter(next_instruction, 1);
                    }
                    else
                    {
                        //maybe play a "End of Experiment" sound?
                        LogOnCSV("[END]", DateTime.Now.ToString(), DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, "N/A");
                        Debug.Log("End of experiment!");
                    }
                    countdown = coachCountdownDuration;
                    startCoachCountdown = false;
                }
            }
        }
    }

}
