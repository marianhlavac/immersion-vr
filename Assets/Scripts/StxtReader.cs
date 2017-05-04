using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

enum StxtReadState { Root, CueGroup, CueLineFirst, CueLineNext };

public class StxtReadError : Exception {
    public StxtReadError(string message) : base(message) { }
}

public class StxtReader {
    public static SubtitleCue[] ReadFromFile(string filename) {
        string data;

        try {
            data = File.ReadAllText(filename);
        }
        catch (FileNotFoundException) {
            throw new StxtReadError("File not found");
        }

        return ReadFromString(data);
    }

    public static SubtitleCue[] ReadFromString(string data) {
        List<SubtitleCue> cues = new List<SubtitleCue>();
        string[] lines = Regex.Split(data, "\r\n|\r|\n");

        StxtReadState state = StxtReadState.Root;
        string cueGroup = "";

        // Parse the whole file by lines
        foreach (string line in lines) {
            if (line.StartsWith("#")) {
                // Parse cue group
                if (state != StxtReadState.CueLineFirst) {
                    cueGroup = line.Substring(1);
                    state = StxtReadState.CueLineFirst;
                }
                else {
                    throw new StxtReadError("Syntax error while reading stxt.");
                }
            }
            else {
                // Parse cue
                if (line.Length > 0) {
                    cues.Add(ParseCue(line, cueGroup));
                    state = StxtReadState.CueLineNext;
                }
            }
        }

        // Calculate the absolute offsets
        float offset = 0;
        foreach (SubtitleCue cue in cues) {
            cue.absoluteOffset = offset;
            offset += cue.offset + cue.length;
        }

        return cues.ToArray();
    }

    private static SubtitleCue ParseCue(string line, string activeGroup) {
        string[] parts = line.Split(';');

        if (parts.Length != 4) {
            throw new StxtReadError("Syntax error while reading stxt.");
        }

        return new SubtitleCue(
            parts[3],
            activeGroup,
            Convert.ToSingle(parts[0]),
            0f,
            Convert.ToSingle(parts[1]),
            Convert.ToUInt32(parts[2])
        );
    }
}