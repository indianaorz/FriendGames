using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostControl : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    public Vector2 startButtonOffset;

    public bool gameStarted = false;

    private void OnGUI()
    {
        if (!isServer
            || !isLocalPlayer)
        {
            return;
        }
        if (!gameStarted)
        {
            if (GUI.Button(new Rect(startButtonOffset.x, startButtonOffset.y, 120, 36), "Start Game"))
            {
                CmdStartGame();
            }
        }
        else if (GUI.Button(new Rect(startButtonOffset.x, startButtonOffset.y, 120, 36), "End Game"))
        {
            CmdEndGame();
        }
    }

    [Command]
    private void CmdStartGame()
    {
        PlayerControl[] players = GameObject.FindObjectsOfType<PlayerControl>();
        int detectives = 1;
        int innocent = 1;
        int traitors = 1;
        if (players.Length == 3)
        {
            detectives = 1;
            innocent = 1;
            traitors = 1;
        }
        if (players.Length == 4)
        {
            detectives = 1;
            innocent = 2;
            traitors = 1;
        }
        if (players.Length == 5)
        {
            detectives = 1;
            innocent = 3;
            traitors = 1;
        }
        if (players.Length == 6)
        {
            detectives = 1;
            innocent = 3;
            traitors = 2;
        }
        foreach (PlayerControl player in players)
        {
            bool success = false;
            while (!success)
            {
                int random = Random.Range(0, 3);
                if (random == 0
                    && detectives > 0)
                {
                    player.playerType = PlayerControl.PlayerType.Detective;
                    --detectives;
                    success = true;
                }
                else if (random == 1
                    && innocent > 0)
                {
                    player.playerType = PlayerControl.PlayerType.Innocent;
                    --innocent;
                    success = true;
                }
                else if(traitors > 0)
                {
                    player.playerType = PlayerControl.PlayerType.Traitor;
                    --traitors;
                    success = true;
                }
            }
            player.toldDetective = false;
            player.inspected = false;
            player.isDead = false;
            player.gameOver = false;

        }
        RpcResetGame();
        gameStarted = true;
    }

    [ClientRpc]
    void RpcResetGame()
    {

        PlayerControl[] players = GameObject.FindObjectsOfType<PlayerControl>();
        foreach (PlayerControl player in players)
        {

            player.toldDetective = false;
            player.inspected = false;
            player.isDead = false;
            player.gameOver = false;

        }
    }

    [Command]
    private void CmdEndGame()
    {
        PlayerControl[] players = GameObject.FindObjectsOfType<PlayerControl>();
        
        foreach (PlayerControl player in players)
        {
            player.gameOver = true;
        }

        gameStarted = false;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
