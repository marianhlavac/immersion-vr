using System;

public class SubtitleCue {
    public float offset;
    public float absoluteOffset;
    public float length;
    public uint audioCueIdx;
    public string text;
    public string group;

    public SubtitleCue(string text, string group, float offset, float absoluteOffset, float length, uint audioCueIdx) {
        // Convert newline symbols to new lines
        text = text.Replace("\\n", "\n");

        this.text = text;
        this.group = group;
        this.offset = offset;
        this.absoluteOffset = absoluteOffset;
        this.length = length;
        this.audioCueIdx = audioCueIdx;
    }
}
