using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour {

    AppManager instance;

    void Awake() {
        // Act as singleton
        if (instance != this) {
            instance = this;
        } else {
            Destroy(instance);
            instance = this;
        }

        SteamAPI.getOwnedGames();
    }
	
}
