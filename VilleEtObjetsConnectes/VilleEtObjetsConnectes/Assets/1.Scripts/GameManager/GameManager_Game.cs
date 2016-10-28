using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager_Game : GameManagerState
{

    [SerializeField]
    Vector2 initialGamePosition = Vector2.zero;

    [SerializeField]
    string gameSceneName = "Game";

    [SerializeField]
    float matchDuration = 3f;


    public override void OnActivation(string previousState, string info = "")
    {
        base.OnActivation(previousState, info);
        SceneManager.LoadScene(gameSceneName);
        Player[] players = GameManager.Instance.Players;

       for (int i =0; i < players.Length; i++)
        {
            players[i].MoveTo(initialGamePosition, 0, true);
            players[i].gameObject.SetActive(true);
        }

       gameManager.LockMatch();
       gameManager.coordonateRequest.StartRequest();

       gameManager.gameTimer = matchDuration;
    }


    void Update()
    {
        foreach (Player p in gameManager.Players)
        {
            Vector3 position = p.transform.position;

            if (!Physics.Raycast(position, Vector3.down, Mathf.Infinity))
            {
                p.ValidPosition(false);
            }
            else
            {
                p.ValidPosition(true);
                MapManager.Instance.Conquer(new Vector2(position.x, position.z), p.TeamId);
            }
        }

        gameManager.gameTimer -= Time.deltaTime;

        if(gameManager.gameTimer <= 0)
        {
            this.stateMachine.ChangeState("End");
        }
    }

    /// <summary>
    /// Can't launch Tutorial in game state
    /// </summary>
    /// <returns></returns>
    public override bool LaunchTutorial()
    {
        return false;
    }

    /// <summary>
    /// Can't launch game if the game is started
    /// </summary>
    /// <returns></returns>
    public override bool LaunchGame()
    {
        return false;
    }



}
