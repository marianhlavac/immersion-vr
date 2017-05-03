using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum TutorialPhase {
    NotRunning,
    Intro, Intro2,
    Skippable,
    PlayArea,
    Controllers,
    ButtonsTrigger, ButtonsSide, ButtonsTouch, ButtonsRest,
    Launcher
};

public class CuePlayException : Exception {
    public CuePlayException(string message) : base(message) { }
}

public class TutorialManager : MonoBehaviour {
    public TutorialPhase startingPhase;
    public AudioClip[] audioCues;

    private TutorialPhase phase;

    // Use this for initialization
    void Start () {
        phase = startingPhase;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AdvanceTutorial () {

    }

    void PlayAudioCue (uint id) {
        AudioSource source = GetComponent<AudioSource>();

        if (id < 0 || id >= audioCues.Length) {
            throw new CuePlayException("Audio cue with specified ID not found.");
        }

        source.clip = audioCues[id];
        source.Play();
    }
}
