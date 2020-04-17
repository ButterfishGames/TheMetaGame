using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonsLib : MonoBehaviour
{
    const int SOUTH = 0;
    const int EAST = 1;
    const int WEST = 2;
    const int NORTH = 3;
    const int R_SHOULDER = 4;
    const int R_TRIGGER = 5;
    const int L_SHOULDER = 6;
    const int L_TRIGGER = 7;
    const int SELECT = 8;
    const int START = 9;
    const int L_STICK = 10;
    const int R_STICK = 11;

    public static ButtonsLib singleton;
    
    private Dictionary<string, int> currentSprites = new Dictionary<string, int>();

    private string currentScheme;
    private PlayerInput pInput;

    private void Awake()
    {
        singleton = this;

        pInput = GetComponent<PlayerInput>();
        OnControlsChanged(pInput);
    }

    private void OnControlsChanged(PlayerInput pIn)
    {
        currentScheme = pIn.currentControlScheme;
        
        foreach (InputAction action in pIn.actions)
        {
            if (action.name.Equals("SwitchMode"))
            {
                int temp = SpriteParser(action, pIn.currentControlScheme, true);
                if (temp != -1)
                {
                    if (currentSprites.ContainsKey("SwitchModePos"))
                    {
                        currentSprites["SwitchModePos"] = temp;
                    }
                    else
                    {
                        currentSprites.Add("SwitchModePos", temp);
                    }
                }

                temp = SpriteParser(action, pIn.currentControlScheme, false);
                if (temp != -1)
                {
                    if (currentSprites.ContainsKey("SwitchModeNeg"))
                    {
                        currentSprites["SwitchModeNeg"] = temp;
                    }
                    else
                    {
                        currentSprites.Add("SwitchModeNeg", temp);
                    }
                }
            }
            else
            {
                int temp = SpriteParser(action, pIn.currentControlScheme);
                if (temp != -1)
                {
                    if (currentSprites.ContainsKey(action.name))
                    {
                        currentSprites[action.name] = temp;
                    }
                    else
                    {
                        currentSprites.Add(action.name, temp);
                    }
                }
            }
        }
    }

    private int SpriteParser(InputAction action, string scheme)
    {
        return SpriteParser(action, scheme, false);
    }

    private int SpriteParser(InputAction action, string scheme, bool positive)
    {
        if (scheme.Equals("KeyboardAndMouse"))
        {
            return -1;
        }

        string path = "";
        foreach (InputBinding binding in action.bindings)
        {
            if (binding.groups.Contains(scheme))
            {
                if (binding.isPartOfComposite)
                {
                    if (binding.name.Equals("positive") && positive)
                    {
                        path = binding.path;
                    }
                    else if (binding.name.Equals("negative") && !positive)
                    {
                        path = binding.path;
                    }
                }
                else
                {
                    path = binding.path;
                }
            }
        }

        if (path.Contains("buttonSouth")) return SOUTH;
        else if (path.Contains("buttonEast")) return EAST;
        else if (path.Contains("buttonWest")) return WEST;
        else if (path.Contains("buttonNorth")) return NORTH;
        else if (path.Contains("rightShoulder")) return R_SHOULDER;
        else if (path.Contains("rightTrigger")) return R_TRIGGER;
        else if (path.Contains("leftShoulder")) return L_SHOULDER;
        else if (path.Contains("leftTrigger")) return L_TRIGGER;
        else if (path.Contains("select")) return SELECT;
        else if (path.Contains("start")) return START;
        else if (path.Contains("leftStick")) return L_STICK;
        else if (path.Contains("rightStick")) return R_STICK;
        else return -1;
    }

    public string DialogueAction(string action)
    {
        string output = "";

        if (currentScheme.Equals("KeyboardAndMouse"))
        {
            string path = "";
            bool found = false;
            bool? positive = null;
            if (action.Contains("SwitchMode"))
            {
                if (action.Contains("Pos"))
                {
                    positive = true;
                }
                else
                {
                    positive = false;
                }

                action = "SwitchMode";
            }

            for (int i = 0; i < pInput.actions[action].bindings.Count && !found; i++)
            {
                if (pInput.actions[action].bindings[i].groups.Contains(currentScheme))
                {
                    if (positive == null)
                    {
                        path = pInput.actions[action].bindings[i].path;
                        found = true;
                    }
                    else if (positive == true && pInput.actions[action].bindings[i].name.Equals("positive"))
                    {
                        path = pInput.actions[action].bindings[i].path;
                        found = true;
                    }
                    else if (positive == false && pInput.actions[action].bindings[i].name.Equals("negative"))
                    {
                        path = pInput.actions[action].bindings[i].path;
                        found = true;
                    }
                }
            }

            if (found)
            {
                string temp = path.Substring(path.IndexOf("/") + 1);
                if (temp.Length > 1)
                {
                    output = temp.Substring(0, 1).ToUpper() + temp.Substring(1);
                }
                else
                {
                    output = temp.ToUpper();
                }
            }
        }
        else
        {
            output = "<sprite=" + currentSprites[action] + ">";

            if (currentScheme.Equals("DualShock"))
            {
                if (currentSprites[action] == 8)
                {
                    output += "Share";
                }
                else if (currentSprites[action] == 9)
                {
                    output += "Options";
                }
            }
        }

        return output;
    }

    public string DialogueExpl(string input)
    {
        string output = "";

        if (currentScheme.Equals("KeyboardAndMouse"))
        {

        }

        return output;
    }
}
