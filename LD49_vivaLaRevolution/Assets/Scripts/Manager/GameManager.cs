using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public UnityEvent<string> onGameEnd;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

    }

    public void EndGame(bool isWin)
    {
        string s = isWin ? "You revolutionized the country! \nThe unstable times are over and better days are to come" : "Noone is left to revolutionize... For now! \nOnly a matter of time until the next revolution will start in these unstable times...";
        onGameEnd?.Invoke(s);
    }


    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
