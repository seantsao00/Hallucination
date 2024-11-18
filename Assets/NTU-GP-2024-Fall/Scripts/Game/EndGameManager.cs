using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour {
    public void EndGame() {
        GameStateManager.Instance.CurrentGameState = GameState.End;
        SceneManager.LoadScene("EndGame");
    }
}
