using UnityEngine;
using System.Collections;
using EquilibreGames;

public class GameManagerState : State {

    [SerializeField]
    protected GameManager gameManager;


    public virtual bool LaunchTutorial()
    {
        this.stateMachine.ChangeState("Tutorial");
        return true;
    }

    public virtual bool LaunchGame()
    {
        this.stateMachine.ChangeState("Game");
        return true;
    }

    public virtual bool LaunchConnect()
    {
        this.stateMachine.ChangeState("Connect");
        return true;
    }
}
