using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public static class SteamAPI {

	public static string apiBaseUrl = "http://api.steampowered.com";
	public static string apiKey = "F3DEFFF53EEA9F6480BF82B654A5C3F3"; // TODO: this must gtfo

	private static string makeUrl(string iface, string method, string version, string parameters) {
		return apiBaseUrl + "/" + iface + "/" + method + "/" + 
			version + "?key=" + apiKey + (parameters != "" ? "&" + parameters : "");
	}

	private static string makeUrl(string iface, string method, string version) {
		return makeUrl (iface, method, version, "");
	}


    public static IEnumerator makeRequest(string URL) {
        WWW www = new WWW(URL);
        Debug.Log("Yielding");
        yield return www;

        Debug.Log("Yielding done");
        Debug.Log(www.text);
    }

    public static void getOwnedGames() {
        string url = makeUrl("IPlayerService", "GetOwnedGames", "v0001", "steamid=76561198006300545&include_appinfo=1&include_played_free_games=1");

        makeRequest(url);
	}
}
