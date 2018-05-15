using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour
{

    public enum PlayerType
    {
        Innocent,
        Traitor,
        Detective
    }

    [SyncVar]
    public string playerName;

    [SyncVar]
    public PlayerType playerType;

    [SyncVar]
    public bool isDead = false;


    public Vector2 deathButtonOffset;

    Color aliveColor;
    public Color deadColor;

    public bool inspected = false;

    [SyncVar]
    public bool gameOver = false;

    [SyncVar]
    public bool toldDetective = false;

    void OnGUI()
    {
        GUI.Label(new Rect(Camera.main.WorldToScreenPoint(transform.position).x,
            -Camera.main.WorldToScreenPoint(transform.position).y + Camera.main.scaledPixelHeight, 200, 64), playerName);

        string playerTypeName = "Innocent";
        switch (playerType)
        {
            case PlayerType.Detective:
                playerTypeName = "Detective";
                break;
            case PlayerType.Traitor:
                playerTypeName = "Traitor";
                break;
        }

        //Displayer the player type
        if (isLocalPlayer
            || inspected && isDead
            || gameOver
            || toldDetective)
        {

            GUI.Label(new Rect(Camera.main.WorldToScreenPoint(transform.position).x + 200,
                -Camera.main.WorldToScreenPoint(transform.position).y + Camera.main.scaledPixelHeight, 200, 64), playerTypeName);

        }

        //Name editing at the top
        if (!isLocalPlayer)
        {
            return;
        }
        GUI.Label(new Rect(0, 16, 72, 64), "Edit Name: ");
        playerName = GUI.TextField(new Rect(72, 16, 200, 20), playerName, 25);
        if (GUI.changed)
        {
            CmdUpdatePlayerName(playerName);
        }

        //Button to allow the local player to die
        if (GUI.Button(new Rect(Camera.main.WorldToScreenPoint(transform.position).x + deathButtonOffset.x,
            -Camera.main.WorldToScreenPoint(transform.position).y + Camera.main.scaledPixelHeight + deathButtonOffset.y, 64, 24), "Dead"))
        {
            CmdDie();
        }


        //If the player is the detective they should be able to inspect all the other player's identities
        if (playerType == PlayerType.Detective
            && !isDead)
        {
            //Detective can reveal they're the detective
            if (!toldDetective)
            {
                if (GUI.Button(new Rect(Camera.main.WorldToScreenPoint(transform.position).x - 52,
                    -Camera.main.WorldToScreenPoint(transform.position).y + Camera.main.scaledPixelHeight + deathButtonOffset.y, 48, 24), "Show"))
                {
                    //Command to tell other players they're the detective
                    CmdRevealDetective();
                }
            }

            PlayerControl[] players = GameObject.FindObjectsOfType<PlayerControl>();
            foreach (PlayerControl player in players)
            {
                if (player.isDead && player != this)
                {
                    if (GUI.Button(new Rect(Camera.main.WorldToScreenPoint(player.transform.position).x + deathButtonOffset.x,
                -Camera.main.WorldToScreenPoint(player.transform.position).y + Camera.main.scaledPixelHeight + deathButtonOffset.y, 64, 24), "Inspect"))
                    {
                        player.inspected = true;
                    }
                }
            }

        }
    }

    [Command]
    void CmdRevealDetective()
    {
        toldDetective = true;
    }

    [Command]
    void CmdUpdatePlayerName(string playerName)
    {
        this.playerName = playerName;
    }


    [Command]
    void CmdDie()
    {
        isDead = true;
    }

    public Renderer background;
    // Use this for initialization
    void Start()
    {
        aliveColor = background.material.color;
    }

    bool removedHud = false;

    void Update()
    {
        //Update gui states
        if (isDead)
        {
            background.material.color = deadColor;
        }
        else
        {
            background.material.color = aliveColor;
        }

        //Remove the network manager hud once the game starts
        if (!removedHud)
        {
            GameObject.FindObjectOfType<NetworkManagerHUD>().showGUI = false;
            removedHud = true;
        }
    }
}
