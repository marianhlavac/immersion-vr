using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour {

    AppManager instance;

    void Awake() {
        if (instance != this) {
            instance = this;
        } else {
            Destroy(instance);
            instance = this;
        }

        string ownedGames = SteamAPI.getOwnedGames(delegate (string result) {
            print(result);
        });

        print(ownedGames);
    }
	
}
