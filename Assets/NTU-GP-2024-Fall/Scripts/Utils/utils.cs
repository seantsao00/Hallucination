using UnityEngine;
using UnityEngine.Assertions;

public class Utils {
    static public GameObject FindCurrentPlayedCharacter() {
        GameObject[] searchResults = GameObject.FindGameObjectsWithTag("Player");
        Assert.IsTrue(searchResults.Length == 1);
        GameObject currentPlayedCharacter = searchResults[0];
        return currentPlayedCharacter;
    }
}