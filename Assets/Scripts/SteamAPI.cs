using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class SteamAPI : MonoBehaviour {

	public static string apiBaseUrl = "http://api.steampowered.com";
	public static string apiKey = "F3DEFFF53EEA9F6480BF82B654A5C3F3"; // TODO: this must gtfo

    public bool isDownloaded = false;
    public List<GameItem> games;

    IEnumerator Start() {
        string url = makeUrl("IPlayerService", "GetOwnedGames", "v0001", "steamid=76561198006300545&include_appinfo=1&include_played_free_games=1");
        Debug.Log("Downloading game list...");

        WWW www = new WWW(url);
        yield return www;

        GameItemsJson jsonResult = JsonUtility.FromJson<GameItemsJson>(www.text);

        int gi = 0;
        foreach (GameItem game in jsonResult.response.games) {
            games.Add(game);
            if (++gi > 3) break;
        }

        Debug.Log("Downloading game banners...");

        int i = 0;
        foreach (GameItem item in games) {
            WWW bannerwww = new WWW("https://steamdb.info/static/camo/apps/" + item.appid + "/header.jpg");
            yield return bannerwww;

            item.banner = bannerwww.texture;

            Debug.Log("Downloaded banner " + (++i).ToString() + "/" + games.Count.ToString());
        }
    }

    private string makeUrl(string iface, string method, string version, string parameters) {
		return apiBaseUrl + "/" + iface + "/" + method + "/" + 
			version + "?key=" + apiKey + (parameters != "" ? "&" + parameters : "");
	}

	private string makeUrl(string iface, string method, string version) {
		return makeUrl (iface, method, version, "");
	}
}
