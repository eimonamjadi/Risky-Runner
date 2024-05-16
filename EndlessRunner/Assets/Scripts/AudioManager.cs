using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MMSingleton<AudioManager>
{
	public Sound[] sounds;
    public List<Sound> pausedSounds;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Sound s in sounds) {
        	s.source = gameObject.AddComponent<AudioSource>();
        	s.source.clip = s.clip;
        	s.source.loop = s.loop;
        }

        PlaySound("MainTheme");
    }

    public void PlaySound(string name) {
    	foreach(Sound s in sounds) {
        	if(s.name == name) {
        		s.source.Play();
                s.source.volume = s.volume;

            }
        }
    }

    public void PauseSounds()
    {
        pausedSounds = new List<Sound>();
        foreach (Sound s in sounds)
        {
            if (s.source.isPlaying)
            {
                s.source.Pause();
                pausedSounds.Add(s);
            }
        }
    }

    public void ResumeSounds()
    {
        foreach(Sound s in pausedSounds)
        {
            s.source.Play();
        }
    }
}
