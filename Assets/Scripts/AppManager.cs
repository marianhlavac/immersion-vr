using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour {

    AppManager instance;

    public GameObject libraryObject;

    private Library library;
    private bool needsLibraryUpdate = true;
    private SteamAPI steamAPI;

    void Awake() {
        // Act as singleton
        if (instance != this) {
            instance = this;
        } else {
            Destroy(instance);
            instance = this;
        }

        steamAPI = GetComponent<SteamAPI>();
        library = libraryObject.GetComponent<Library>();
    }

    private void Update() {
        if (steamAPI.isDownloaded && needsLibraryUpdate) {
            needsLibraryUpdate = false;
            library.SetGamesSource(steamAPI.games.ToArray());
        }
    }
	
}
