using System;
using UnityEngine;

public class GameItem {
    public string name;
    public Texture2D banner;
    public MovieTexture gameplay;
    public string description;
    public string steamid;

    public void RunGame() {
        Application.OpenURL("steam://run/" + steamid.ToString());
    }
}
