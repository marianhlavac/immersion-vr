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
    public TextAsset subtitleTextFile = null;

    public GameObject subtitlesObject;

    private TutorialPhase phase;
    private SubtitleCue[] subtitleCues;
    private int cuePosition = -1;
    private int currentAudioCue = -1;
    
    void Start () {
        phase = startingPhase;

        if (subtitleTextFile == null) {
            throw new Exception("Subtitle Text File hasn't been specified.");
        }

        subtitleCues = StxtReader.ReadFromString(subtitleTextFile.text);

        StartTutorial();
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.F1)) {
            ExitTutorial();
        }
	}

    public void StartTutorial() {
        PlayCue(subtitleCues[++cuePosition]);

        if (cuePosition < subtitleCues.Length) {
            float timeToNextCue = subtitleCues[cuePosition].length + subtitleCues[cuePosition + 1].offset;
            Invoke("StartTutorial", timeToNextCue);
        }
    }

    public void ExitTutorial() {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Tutorial")) {
            Destroy(obj);
        }
    }

    private void PlayCue(SubtitleCue cue) {
        DisplayCue(cue.text);

        if (currentAudioCue != cue.audioCueIdx) {
            PlayAudioCue((int)cue.audioCueIdx);
        }
    }

    private void DisplayCue(string text) {
        TextMesh subtitleText = subtitlesObject.GetComponentInChildren<TextMesh>();
        GameObject subtitleBackground = subtitlesObject.transform.FindChild("Background").gameObject;

        subtitleText.text = text;
    }

    private void PlayAudioCue (int id) {
        AudioSource source = GetComponent<AudioSource>();

        if (id < 0 || id >= audioCues.Length) {
            throw new CuePlayException("Audio cue with specified ID not found.");
        }

        source.clip = audioCues[id];
        source.Play();
        currentAudioCue = id;
    }
}
