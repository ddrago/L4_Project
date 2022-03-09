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
    private static string baseFileName = "log.csv";
    private static string filename;

    private static Coach coach;

    //This makes sure the Selector class can identify the start of the experiment
    private static Boolean isExperimentStarted;
    private static Boolean isWelcomeAudioDone;

    //This piece of information is useful to give correct names to the log files
    public static string currentLayout = "horizontal";
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
        Debug.Log("Press");
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

    public void restart()
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
        isWelcomeAudioDone = false;
    }

    internal void updateLogFilename(Spawner.Layout layout)
    {
        //update filename
        currentLayout = layout.ToString();
        baseFileName = currentLayout + "_log.csv";
        print(baseFileName);
    }

    public static void InitLogging()
    {
        filename = Application.persistentDataPath + "/" + baseFileName;

        System.IO.FileInfo theSourceFile = new System.IO.FileInfo(filename);
        //System.IO.File.WriteAllText(filename, "[TEST]" + filename);
        System.IO.File.WriteAllLines(filename, new string[] {
            "InteractionType,Time,TimeMS,Item",
            "[START]" + "," + DateTime.Now.ToString() + "," + DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + "," + "N/A"
        });
/*        if (System.IO.File.Exists(filename))
        {
            System.IO.StreamReader reader = theSourceFile.OpenText();
            string text = reader.ReadLine();
            print(text);
        }*/
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
        isWelcomeAudioDone = false;
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
        public static List<string> instructions;

        public static System.Random rnd = new System.Random();

        public static string instruction_log_filename = "instructions.txt";


        public Coach(float countdownDuration)
        {
            startCoachCountdown = false;
            countdown = coachCountdownDuration;
            coachCountdownDuration = countdownDuration;

            instructions = new List<string>(instructions_to_give);
            instructions = instructions.OrderBy(a => rnd.Next()).ToList();
            Debug.Log(string.Join(",", instructions.ToArray()));

            instruction_log_filename = currentLayout.ToString() + "_" + "instructions.txt";
            LogInstructions();
        }

        public void LogInstructions()
        {
            //update filename
            instruction_log_filename = Application.persistentDataPath + "/" + instruction_log_filename;
            Debug.Log(instruction_log_filename);

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
                        if (isWelcomeAudioDone)
                        {
                            FindObjectOfType<AudioManager>().Play("NowPress");
                            FindObjectOfType<AudioManager>().PlayAfter(next_instruction, 1);
                        }
                        else
                        {
                            FindObjectOfType<AudioManager>().Play("ExperimentStart");
                            FindObjectOfType<AudioManager>().PlayAfter("NowPress", 2.5f);
                            FindObjectOfType<AudioManager>().PlayAfter(next_instruction, 3.5f);
                            isWelcomeAudioDone = true;

                        }
                    }
                    else
                    {
                        FindObjectOfType<AudioManager>().Play("ExperimentEnd");
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
