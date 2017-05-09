using System;
using UnityEngine;

[Serializable]
public class GameItemsJson {
    public GameItemsJsonResponse response;
}

[Serializable]
public class GameItemsJsonResponse {
    public int game_count;
    public GameItem[] games;
}

[Serializable]
public class GameItem {
    public int appid;
    public string name;
    public int playtime_forever;
    public string img_icon_url;
    public string img_logo_url;
    public bool has_community_visible_stats;
    public Texture2D banner;

    public void RunGame() {
        Application.OpenURL("steam://run/" + appid.ToString());
    }
}
