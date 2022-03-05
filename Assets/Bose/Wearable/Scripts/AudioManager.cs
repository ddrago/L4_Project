using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public void Play(string name)
    {
        Sound s  = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound file " + name + " not found!");
            return;
        }
        //Debug.Log("Now playing: " + s.name);
        s.source.Play();
    }

    public void PlayAfter(string name, float delay)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound file " + name + " not found!");
            return;
        }

        s.source.PlayDelayed(delay);
    }

    public float GetDuration(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound file " + name + " not found!");
            return 0;
        }
        Debug.LogWarning(s.source.time);
        // Why the fuck is it zero

        return s.source.time;
    }
}
