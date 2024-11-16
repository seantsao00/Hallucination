using UnityEngine;
using UnityEngine.Assertions;

public class Utils {
    static public GameObject GetFairyObjects() {
        GameObject[] searchResults = GameObject.FindGameObjectsWithTag("Player");
        foreach (var characterObject in searchResults) {
            if (characterObject.name == "Fairy") return characterObject;
        }
        Debug.LogError("Can't find Fairy Object");
        return null;
    }
    static public GameObject GetBearObjects() {
        GameObject[] searchResults = GameObject.FindGameObjectsWithTag("Player");
        foreach (var characterObject in searchResults) {
            if (characterObject.name == "Bear") return characterObject;
        }
        Debug.LogError("Can't find Bear Object");
        return null;
    }

    static public GameObject FindCurrentPlayedCharacter() {
        GameObject[] searchResults = GameObject.FindGameObjectsWithTag("Player");
        Assert.IsTrue(searchResults.Length == 1);
        GameObject currentPlayedCharacter = searchResults[0];
        return currentPlayedCharacter;
    }
}