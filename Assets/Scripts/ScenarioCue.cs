using System;

public class ScenarioCueActionNotFoundException : Exception {
}

public enum ScenarioCueAction {
    None, ShowLogo, HideLogo,
    ShowSkipTxt, HideSkipTxt,
    WaitForUserRaise, WaitForUserTargetHit, WaitForSide, WaitForUserColor,
    GiveLaser,
    HighlightTrigger, HighlightSide, HighlightTouchpad, HighlightMenu, HighlightSystem,
    GotoLibrary, Skip
}

public class ScenarioCue {
    public float offset;
    public float absoluteOffset;
    public float length;
    public uint audioCueIdx;
    public string text;
    public ScenarioCueAction action;
    public bool isAction;
    public string group;

    public ScenarioCue(string text, string group, float offset, float absoluteOffset, float length, uint audioCueIdx) {
        // Convert newline symbols to new lines
        text = text.Replace("\\n", "\n");

        this.text = text;
        this.group = group;
        this.offset = offset;
        this.absoluteOffset = absoluteOffset;
        this.length = length;
        this.audioCueIdx = audioCueIdx;
        isAction = false;
    }

    public ScenarioCue(ScenarioCueAction action, string group, float offset, float absoluteOffset, float length, uint audioCueIdx) {
        this.action = action;
        this.group = group;
        this.offset = offset;
        this.absoluteOffset = absoluteOffset;
        this.length = length;
        this.audioCueIdx = audioCueIdx;
        isAction = true;
    }

    public static ScenarioCueAction ActionFromString(string actionName) {
        ScenarioCueAction action = ScenarioCueAction.None;

        try {
            action = (ScenarioCueAction)Enum.Parse(typeof(ScenarioCueAction), actionName, true);
        } catch {
            throw new ScenarioCueActionNotFoundException();
        }
        return action;
    }
}