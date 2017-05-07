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
    public float raiseThreshold = 0.75f;

    public GameObject subtitlesObject;
    public GameObject laserPointerPrefab;
    public GameObject launcherRigPrefab;
    public GameObject targetPrefab;

    private TutorialPhase phase;
    private ScenarioCue[] subtitleCues;
    private int cuePosition = -1;
    private int currentAudioCue = -1;
    private Func<bool> pauseUntil = null;
    private bool tutorialRunning = false;
    private float nextCueTime = 0;
    private GameObject leftController;
    private GameObject rightController;
    private GameObject head;
    private GameObject rig;

    void Start () {
        phase = startingPhase;

        GameObject vrRig = GameObject.Find("SteamVRRig");
        leftController = vrRig.transform.Find("LeftController").gameObject;
        rightController = vrRig.transform.Find("RightController").gameObject;
        head = vrRig.transform.Find("Camera (eye)").gameObject;

        rig = GameObject.Find("TutorialRig");

        if (subtitleTextFile == null) {
            throw new Exception("Subtitle Text File hasn't been specified.");
        }

        subtitleCues = StxtReader.ReadFromString(subtitleTextFile.text);
        tutorialRunning = true;

        // Skip tutorial by command args
        string[] args = System.Environment.GetCommandLineArgs();

        foreach (string arg in args) {
            if (arg == "-launcher") {
                ExitTutorial();
                Debug.Log("Going straight to the launcher.");
            }
        }
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.F1)) {
            ExitTutorial();
        }

        if (pauseUntil != null) {
            if (pauseUntil()) {
                pauseUntil = null;
            }
        }

        if (tutorialRunning && nextCueTime <= Time.time && pauseUntil == null) {
            AdvanceTutorial();
        }
	}

    public void AdvanceTutorial() {
        PlayCue(subtitleCues[++cuePosition]);

        if (cuePosition < subtitleCues.Length - 1) {
            nextCueTime = Time.time + subtitleCues[cuePosition].length + subtitleCues[cuePosition + 1].offset;
        } else {
            tutorialRunning = false;
        }
    }

    public void ExitTutorial() {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Tutorial")) {
            Destroy(obj);
        }
    }

    private void PlayCue(ScenarioCue cue) {
        if (cue.isAction) {
            InvokeCueAction(cue.action);
        } else {
            DisplayCue("[" + cue.absoluteOffset.ToString() + "] " + cue.text);

            if (currentAudioCue != cue.audioCueIdx) {
                PlayAudioCue((int)cue.audioCueIdx);
            }
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

    /**
     * Invokes a specified cue action.
     */
    private void InvokeCueAction (ScenarioCueAction action) {
        Debug.Log("Invoking action " + action.ToString());

        switch (action) {
            // Ends the tutorial and goes to the launcher library.
            case ScenarioCueAction.GotoLibrary:
                Instantiate(launcherRigPrefab);
                break;

            // Shows arcade logo
            case ScenarioCueAction.ShowLogo:
                FadeInFadeOut logoFader = GameObject.Find("ArcadeLogo").GetComponent<FadeInFadeOut>();
                logoFader.Show();
                break;

            // Hides the arcade logo
            case ScenarioCueAction.HideLogo:
                FadeInFadeOut logoFaderB = GameObject.Find("ArcadeLogo").GetComponent<FadeInFadeOut>();
                logoFaderB.Hide();
                break;

            // Shows skip text
            case ScenarioCueAction.ShowSkipTxt:
                FadeInFadeOut stFader = GameObject.Find("SkippableText").GetComponent<FadeInFadeOut>();
                stFader.Show();
                break;

            // Hides the skip text
            case ScenarioCueAction.HideSkipTxt:
                FadeInFadeOut stFaderB = GameObject.Find("SkippableText").GetComponent<FadeInFadeOut>();
                stFaderB.Hide();
                break;

            case ScenarioCueAction.WaitForUserRaise:
                pauseUntil = areControllersRaised;
                break;
               
            // Gives a laser pointer to the user.
            case ScenarioCueAction.GiveLaser:
                GameObject laserPointer = Instantiate<GameObject>(laserPointerPrefab);
                laserPointer.transform.parent = rightController.transform;


                GameObject target = Instantiate<GameObject>(targetPrefab);
                target.transform.parent = rig.transform;
                break;
        }
    }

    private bool areControllersRaised() {
        float expectedHeight = head.transform.position.y * raiseThreshold;

        return leftController.transform.position.y >= expectedHeight &&
            rightController.transform.position.y >= expectedHeight;
    }
}
