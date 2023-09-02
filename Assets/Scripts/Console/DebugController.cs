using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{
    private bool isShowingConsole;
    private bool isShowingHelp;
    private string input;

    private Vector2 scroll;

    //Commands
    public static DebugCommand HELP;

    public static DebugCommand PLAYER_KILL;
    public static DebugCommand<int> SET_PLAYER_HEALTH;
    public static DebugCommand<int> SET_PLAYER_MAX_HEALTH;
    public static DebugCommand<int> SET_PLAYER_DAMAGE;

    public static DebugCommand EBG1;
    public static DebugCommand EBG2;
    public static DebugCommand EBG3;
    public static DebugCommand EBG4;

    public List<object> commandList;


    private void OnToggleDebug(InputValue value) => isShowingConsole = !isShowingConsole;

    private void OnEnter(InputValue value)
    {
        if (!isShowingConsole) { return; }

        HandleInput();
        input = "";
    }


    private void Awake()
    {
        PlayerCommands();
        MusicCommands();
        MiscCommands();

        commandList = new List<object>()
        {
            HELP,
            PLAYER_KILL,
            SET_PLAYER_HEALTH,
            SET_PLAYER_MAX_HEALTH,
            SET_PLAYER_DAMAGE,
            EBG1,
            EBG2,
            EBG3,
            EBG4
        };
    }

    private void PlayerCommands()
    {
        PLAYER_KILL = new DebugCommand("player_kill", "Kills the player", "player_kill", () =>
        {
            GameController.Health -= GameController.Health + 1;
        });

        SET_PLAYER_HEALTH = new DebugCommand<int>("set_player_health", "Set the health of the player", "set_player_health <health_amount>", (x) =>
        {
            GameController.Health = x;
            GameController.OnPlayerHeal?.Invoke();
        });

        SET_PLAYER_MAX_HEALTH = new DebugCommand<int>("set_player_max_health", "Set the max health of the player", "set_player_max_health <max_health_amount>", (x) =>
        {
            GameController.MaxHealth = x;
            GameController.OnPlayerHeal?.Invoke();
        });

        SET_PLAYER_DAMAGE = new DebugCommand<int>("set_player_damage", "Set the damage of the player", "set_player_damage <damage_amount>", (x) =>
        {
            GameController.PlayerDamage = x;
        });
    }

    private void MusicCommands()
    {
        EBG1 = new DebugCommand("ebg1", "Hidden", "ebg1", () =>
        {
            AudioManager.Instance.musicSource.Stop();
            AudioManager.Instance.PlayEBGMusic("EBG1");
        });

        EBG2 = new DebugCommand("ebg2", "Hidden", "ebg2", () =>
        {
            AudioManager.Instance.musicSource.Stop();
            AudioManager.Instance.PlayEBGMusic("EBG2");
        });

        EBG3 = new DebugCommand("ebg3", "Hidden", "ebg3", () =>
        {
            AudioManager.Instance.musicSource.Stop();
            AudioManager.Instance.PlayEBGMusic("EBG3");
        });

        EBG4 = new DebugCommand("ebg4", "Hidden", "ebg4", () =>
        {
            AudioManager.Instance.musicSource.Stop();
            AudioManager.Instance.PlayEBGMusic("EBG4");
        });
    }

    private void MiscCommands()
    {
        HELP = new DebugCommand("#help", "Shows all commands", "#help", () =>
        {
            isShowingHelp = !isShowingHelp;
        });
    }

    private void OnGUI()
    {
        if (!isShowingConsole) { return; }

        float y = 0f;
        
        if(isShowingHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");

            Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);
            scroll = GUI.BeginScrollView(new Rect(0, 0, Screen.width, 90), scroll, viewport);

            for(int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;

                string label = $"{command.commandFormat} - {command.commandDescription}";
                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();
            y += 100;
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);

        GUI.SetNextControlName("console");
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
        GUI.FocusControl("console");
    }

    private void HandleInput()
    {
        string[] properties = input.Split(' ');

        for(int i = 0; i < commandList.Count; i++)
        {
            //Get DebugCommandBase object
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            //Check if input contains command id
            if (input.Contains(commandBase.commandId))
            {
                //Check if the type of object fits the cast
                if (commandList[i] as DebugCommand != null)
                {
                    //Cast to normal command and Invoke()
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if(commandList[i] as DebugCommand<int> != null)
                {
                    //Cast to int type command and Invoke()
                    (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                }
            }
        }
    }
}
