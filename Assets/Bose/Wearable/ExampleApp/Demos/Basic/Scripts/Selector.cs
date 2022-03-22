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

    //The name of the logging files - MODIFY FOR EACH PARTICIPANT
    private static string baseFileName = "log.csv";
    private static string filename;
    private static string baseGazePathFilename = "gazepath.csv";
    private static string gazePathFilename;

    private static Coach coach;

    //This makes sure the Selector class can identify the start of the experiment
    private static Boolean isExperimentStarted;
    private static Boolean isWelcomeAudioDone;

    //This piece of information is useful to give correct names to the log files
    public static string currentLayout = "horizontal";

    //This method is called at every update. It checks whether the user's gaze is hovering over any item and acts accordingly
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

                LogOnCSV("[HOVER]", _hitInfo.collider.gameObject.name);

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
            LogOnCSV("[STARTING PRESS]", "N/A");
            coach.startCoachCountdown = true;

            return;
        }

        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            FindObjectOfType<AudioManager>().Play("MenuButtonPress");
            LogOnCSV("[PRESS]", _hitInfo.collider.gameObject.name);
            //print("PRESS");

            coach.startCoachCountdown = true;
        }
        else
        {
            LogOnCSV("[PRESS]", "N/A");
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
        //update the browing and selection data filename
        currentLayout = layout.ToString();
        baseFileName = currentLayout + "_log.csv";
        print(baseFileName);

        //update the gaze path directional data filename
        currentLayout = layout.ToString();
        baseGazePathFilename = currentLayout + "gazepath.csv";
    }

    public static void InitLogging()
    {
        // Log the browing and selection data
        filename = Application.persistentDataPath + "/" + currentLayout + "_" + baseFileName;
        System.IO.File.WriteAllLines(filename, new string[] {
            "InteractionType,Time,TimeMS,Item,isCorrectItem",
            "[START]" + "," + DateTime.Now.ToString() + "," + DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + "," + "N/A" + "," + "N/A"
        });

        // Log the gaze path directional data
        gazePathFilename = Application.persistentDataPath + "/" + currentLayout + "_" + baseGazePathFilename;
        System.IO.File.WriteAllLines(gazePathFilename, new string[] {
            "X, Y, Z"
        });
    }

    public static void LogOnCSV(string interactionType, string item)
    {
        bool isCorrectItem = false;
        if (coach is not null)
        {
            if(coach.next_instruction is not null)
            {
                isCorrectItem = coach.next_instruction.Equals(item);
            }
        }

        System.IO.File.AppendAllLines(filename, new string[] {
            interactionType + "," + DateTime.Now.ToString() + "," + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString() + "," + item + "," + isCorrectItem
        });
    }

    public static void LogGazePath()
    {
        if (Physics.Raycast(_ray, out _hitInfo, 100))
        {
            Debug.Log(_hitInfo.point.ToString());
            System.IO.File.AppendAllLines(gazePathFilename, new string[] {
                _hitInfo.point.x.ToString() + "," +  _hitInfo.point.y.ToString() + "," +  _hitInfo.point.z.ToString()
            });
        }
        /*else
        {
            LogOnCSV("[PRESS]", "N/A");
        }*/
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

        LogGazePath();

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
        public string next_instruction;

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
            next_instruction = null;

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
                    next_instruction = Give_instruction();
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
                        LogOnCSV("[END]", "N/A");
                        Debug.Log("End of experiment!");
                    }
                    countdown = coachCountdownDuration;
                    startCoachCountdown = false;
                }
            }
        }
    }

}
