using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum State {MainMenu = 0, inGame = 1, Pause = 2, GameOver = 3 };

    private Scene _scene;
    private State GameState = State.inGame;
    private HologramManager hologramManager;
    
    public static GameManager instance = null;
    

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        
        InitGame();
    }

    private void Update()
    {
        _scene = SceneManager.GetActiveScene();
        hologramManager = new HologramManager();
    }

    private void InitGame()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution(1920, 1080, true);
        Application.targetFrameRate = 144;
    }
    public State getGameState()
    {
        return GameState;
    }
    public void setGameState(State state)
    {
        GameState = state;
        if(GameState == State.inGame)
        {
            Time.timeScale = 1;
        }
        else if (GameState == State.Pause)
        {
            Time.timeScale = 0;
        }
        else if(GameState == State.GameOver)
        {
            gameExit();
        }
    }
    
    public void gameExit()
    {
        SceneManager.LoadScene(0);
    }
}
