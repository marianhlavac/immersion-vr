using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

enum StxtReadState { Root, CueGroup, CueLineFirst, CueLineNext };

public class StxtReadError : Exception {
    public StxtReadError(string message) : base(message) { }
}

public class StxtReader {
    public static ScenarioCue[] ReadFromFile(string filename) {
        string data;

        try {
            data = File.ReadAllText(filename);
        }
        catch (FileNotFoundException) {
            throw new StxtReadError("File not found");
        }

        return ReadFromString(data);
    }

    public static ScenarioCue[] ReadFromString(string data) {
        List<ScenarioCue> cues = new List<ScenarioCue>();
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
                    try {
                        cues.Add(ParseCue(line, cueGroup));
                    } catch (ScenarioCueActionNotFoundException) {
                        throw new StxtReadError("Error while reading stxt: Non-existent cue action.");
                    }
                    state = StxtReadState.CueLineNext;
                }
            }
        }

        // Calculate the absolute offsets
        float offset = 0;
        foreach (ScenarioCue cue in cues) {
            cue.absoluteOffset = offset;
            offset += cue.offset + cue.length;
        }

        return cues.ToArray();
    }

    private static ScenarioCue ParseCue(string line, string activeGroup) {
        string[] parts = line.Split(':');
        string[] parameters = parts[0].Split(';');

        if (parts.Length != 2 || parameters.Length != 3) {
            throw new StxtReadError("Syntax error while reading stxt.");
        }

        if (parts[1].StartsWith("%")) {
            return new ScenarioCue(
                ScenarioCue.ActionFromString(parts[1].Substring(1)),
                activeGroup,
                Convert.ToSingle(parameters[0]),
                0f,
                Convert.ToSingle(parameters[1]),
                Convert.ToUInt32(parameters[2])
            );
        } else {
            return new ScenarioCue(
                parts[1],
                activeGroup,
                Convert.ToSingle(parameters[0]),
                0f,
                Convert.ToSingle(parameters[1]),
                Convert.ToUInt32(parameters[2])
            );
        }
        
    }
}