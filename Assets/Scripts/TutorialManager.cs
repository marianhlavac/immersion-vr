using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

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

    [Header("Links")]
    public GameObject leftTrackedController;
    public GameObject rightTrackedController;
    public GameObject head;
    public GameObject rig;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject subtitlesObject;

    [Header("Prefabs")]
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
    private LaserPointer laserPointer = null;
    private GameObject targetInstance = null;
    private bool laserGiven = false;

    void Start () {
        phase = startingPhase;

        if (subtitleTextFile == null) {
            throw new Exception("Subtitle Text File hasn't been specified.");
        }

        subtitleCues = StxtReader.ReadFromString(subtitleTextFile.text);
        tutorialRunning = true;

        // Skip tutorial by command args
        string[] args = Environment.GetCommandLineArgs();

        foreach (string arg in args) {
            if (arg == "-launcher") {
                ExitTutorial();
                Debug.Log("Going straight to the launcher.");
            }
        }

        hideAllHints();
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.F1) /* || fakin combination press */) {
            InvokeCueAction(ScenarioCueAction.GotoLibrary);
            InvokeCueAction(ScenarioCueAction.Skip);
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
            ExitTutorialAfter(5f);
        }
    }

    public void ExitTutorial() {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Tutorial")) {
            Destroy(obj);
        }
    }

    public void ExitTutorialAfter(float time) {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Tutorial")) {
            Destroy(obj, time);
        }
    }

    private void PlayCue(ScenarioCue cue) {
        if (cue.isAction) {
            InvokeCueAction(cue.action);
        } else {
            DisplayCue(cue.text);

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
                if (!laserGiven) GiveLaser();
                Instantiate(launcherRigPrefab);

                subtitlesObject.transform.localPosition = new Vector3(8.3f, 9.4f, -14);
                subtitlesObject.transform.localEulerAngles = new Vector3(18, 0, 0);
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
               
            // Gives a laser pointer to the user.
			case ScenarioCueAction.GiveLaser:
                GiveLaser();

                targetInstance = Instantiate<GameObject>(targetPrefab);
                targetInstance.transform.parent = rig.transform;
                break;

            case ScenarioCueAction.HighlightMenu:
                highlightButtonOnBothControllers(EVRButtonId.k_EButton_ApplicationMenu, "Menu");
                break;

            case ScenarioCueAction.HighlightSide:
                Destroy(targetInstance, 3);
                highlightButtonOnBothControllers(EVRButtonId.k_EButton_Grip, "Boční tlačítko");
                break;

            case ScenarioCueAction.HighlightSystem:
                highlightButtonOnBothControllers(EVRButtonId.k_EButton_System, "Systém");
                break;

            case ScenarioCueAction.HighlightTouchpad:
                highlightButtonOnBothControllers(EVRButtonId.k_EButton_SteamVR_Touchpad, "Dotyková plocha");
                break;

            case ScenarioCueAction.HighlightTrigger:
                highlightButtonOnBothControllers(EVRButtonId.k_EButton_SteamVR_Trigger, "Spoušť");
                break;

            case ScenarioCueAction.Skip:
                FadeInFadeOut logoFaderC = GameObject.Find("ArcadeLogo").GetComponent<FadeInFadeOut>();
                logoFaderC.Hide();
                FadeInFadeOut stFaderC = GameObject.Find("SkippableText").GetComponent<FadeInFadeOut>();
                stFaderC.Hide();
                ExitTutorial();
                break;

            case ScenarioCueAction.WaitForUserRaise:
                pauseUntil = areControllersRaised;
                break;

            case ScenarioCueAction.WaitForUserTargetHit:
                pauseUntil = isTargetHit;
                break;

            case ScenarioCueAction.WaitForSide:
                pauseUntil = isSidePressed;
                break;

            case ScenarioCueAction.WaitForUserColor:
                pauseUntil = isColorSelected;
                break;
        }
    }

    private void GiveLaser() {
        GameObject laserPointerObject = Instantiate<GameObject>(laserPointerPrefab);
        laserPointer = laserPointerObject.GetComponent<LaserPointer>();
        laserPointer.beamSource = rightTrackedController;
        laserGiven = true;
    }

    private void hideAllHints() {
        ControllerButtonHints.HideAllTextHints(leftHand.GetComponent<Hand>());
        ControllerButtonHints.HideAllTextHints(rightHand.GetComponent<Hand>());
    }

    private void highlightButtonOnBothControllers(EVRButtonId button, string text) {
        hideAllHints();
        ControllerButtonHints.ShowTextHint(leftHand.GetComponent<Hand>(), button, text);
        ControllerButtonHints.ShowTextHint(rightHand.GetComponent<Hand>(), button, text);
    }

    private bool areControllersRaised() {
        float expectedHeight = head.transform.position.y * raiseThreshold;

        return leftTrackedController.transform.position.y >= expectedHeight &&
            rightTrackedController.transform.position.y >= expectedHeight;
    }

    private bool isTargetHit() {
        return laserPointer.isBeaming && laserPointer.pointingAt == targetInstance.gameObject;
    }

    private bool isSidePressed() {
        SteamVR_TrackedController leftTrackedItem = leftTrackedController.GetComponent<SteamVR_TrackedController>();
        SteamVR_TrackedController rightTrackedItem = leftTrackedController.GetComponent<SteamVR_TrackedController>();

        return leftTrackedItem.gripped || rightTrackedItem.gripped;
    }

    private bool isColorSelected() {
        SteamVR_TrackedController leftTrackedItem = leftTrackedController.GetComponent<SteamVR_TrackedController>();
        SteamVR_TrackedController rightTrackedItem = leftTrackedController.GetComponent<SteamVR_TrackedController>();

        return leftTrackedItem.padPressed || rightTrackedItem.padPressed;
    }
}
