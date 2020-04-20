using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonsLib : MonoBehaviour
{
    public Sprite[] xboxButtons = new Sprite[16];
    public Sprite[] dualshockButtons = new Sprite[16];
    public Sprite[] switchButtons = new Sprite[16];

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
    const int DPAD_UP = 12;
    const int DPAD_RIGHT = 13;
    const int DPAD_LEFT = 14;
    const int DPAD_DOWN = 15;

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

        GetComponent<OtherInputHandlers>().OnControlsChangedHandle(pIn);
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
            
            List<string> prompts = new List<string>();
            for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
            {
                if (pInput.actions[action].bindings[i].groups.Contains(currentScheme))
                {
                    if (positive == null)
                    {
                        path = pInput.actions[action].bindings[i].overridePath == null ? 
                            pInput.actions[action].bindings[i].path
                            : pInput.actions[action].bindings[i].overridePath;
                        string temp = path.Substring(path.IndexOf("/") + 1);
                        if (temp.Length > 1)
                        {
                            prompts.Add(temp.Substring(0, 1).ToUpper() + temp.Substring(1));
                        }
                        else
                        {
                            prompts.Add(temp.ToUpper());
                        }
                    }
                    else if (positive == true && pInput.actions[action].bindings[i].name.Equals("positive"))
                    {
                        path = pInput.actions[action].bindings[i].overridePath == null ?
                            pInput.actions[action].bindings[i].path
                            : pInput.actions[action].bindings[i].overridePath;
                        string temp = path.Substring(path.IndexOf("/") + 1);
                        if (temp.Length > 1)
                        {
                            prompts.Add(temp.Substring(0, 1).ToUpper() + temp.Substring(1));
                        }
                        else
                        {
                            prompts.Add(temp.ToUpper());
                        }
                    }
                    else if (positive == false && pInput.actions[action].bindings[i].name.Equals("negative"))
                    {
                        path = pInput.actions[action].bindings[i].overridePath == null ?
                            pInput.actions[action].bindings[i].path
                            : pInput.actions[action].bindings[i].overridePath;
                        string temp = path.Substring(path.IndexOf("/") + 1);
                        if (temp.Length > 1)
                        {
                            prompts.Add(temp.Substring(0, 1).ToUpper() + temp.Substring(1));
                        }
                        else
                        {
                            prompts.Add(temp.ToUpper());
                        }
                    }
                }
            }

            output = prompts[0];
            if (prompts.Count > 1)
            {
                for (int i = 1; i < prompts.Count; i++)
                {
                    output += " or " + prompts[i];
                }
            }
        }
        else
        {
            try
            {
                output = "<sprite=" + currentSprites[action] + ">";
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log(action);
            }

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

        if (output.Equals("LeftButton"))
        {
            output = "<sprite=0>Left Mouse Button";
        }
        else if (output.Equals("RightButton"))
        {
            output = "<sprite=1>Right Mouse Button";
        }
        else if (output.Equals("LeftShift"))
        {
            output = "Left Shift";
        }
        else if (output.Equals("RightShift"))
        {
            output = "Right Shift";
        }

        return output;
    }

    public string DialogueSingleAction(string action)
    {
        return DialogueSingleAction(action, 0);
    }

    public string DialogueSingleAction(string action, int ind)
    {
        string output = "";

        if (currentScheme.Equals("KeyboardAndMouse"))
        {
            string path = "";
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

            List<string> prompts = new List<string>();
            for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
            {
                if (pInput.actions[action].bindings[i].groups.Contains(currentScheme))
                {
                    if (positive == null)
                    {
                        path = pInput.actions[action].bindings[i].overridePath == null ?
                            pInput.actions[action].bindings[i].path
                            : pInput.actions[action].bindings[i].overridePath;
                        string temp = path.Substring(path.IndexOf("/") + 1);
                        if (temp.Length > 1)
                        {
                            prompts.Add(temp.Substring(0, 1).ToUpper() + temp.Substring(1));
                        }
                        else
                        {
                            prompts.Add(temp.ToUpper());
                        }
                    }
                    else if (positive == true && pInput.actions[action].bindings[i].name.Equals("positive"))
                    {
                        path = pInput.actions[action].bindings[i].overridePath == null ?
                            pInput.actions[action].bindings[i].path
                            : pInput.actions[action].bindings[i].overridePath;
                        string temp = path.Substring(path.IndexOf("/") + 1);
                        if (temp.Length > 1)
                        {
                            prompts.Add(temp.Substring(0, 1).ToUpper() + temp.Substring(1));
                        }
                        else
                        {
                            prompts.Add(temp.ToUpper());
                        }
                    }
                    else if (positive == false && pInput.actions[action].bindings[i].name.Equals("negative"))
                    {
                        path = pInput.actions[action].bindings[i].overridePath == null ?
                            pInput.actions[action].bindings[i].path
                            : pInput.actions[action].bindings[i].overridePath;
                        string temp = path.Substring(path.IndexOf("/") + 1);
                        if (temp.Length > 1)
                        {
                            prompts.Add(temp.Substring(0, 1).ToUpper() + temp.Substring(1));
                        }
                        else
                        {
                            prompts.Add(temp.ToUpper());
                        }
                    }
                }
            }

            output = prompts[ind];
        }
        else
        {
            try
            {
                output = "<sprite=" + currentSprites[action] + ">";
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log(action);
            }

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

        if (output.Equals("LeftButton"))
        {
            output = "<sprite=0>  LMB";
        }
        else if (output.Equals("RightButton"))
        {
            output = "<sprite=1>  RMB";
        }

        return output;
    }

    public string DialogueExpl(string input)
    {
        string output = "";

        if (currentScheme.Equals("KeyboardAndMouse"))
        {
            if (input == "LStick") output = "WASD or the Arrow Keys";
            else if (input == "RStick") output = "the mouse";
            else if (input == "DPadUp") output = "W or Up Arrow";
            else if (input == "DPadRight") output = "D or Right Arrow";
            else if (input == "DPadLeft") output = "A or Left Arrow";
            else if (input == "DPadDown") output = "S or Down Arrow";
        }
        else
        {
            output = "<sprite=";
            if (input == "LStick") output += L_STICK + ">";
            else if (input == "RStick") output += R_STICK + ">";
            else if (input == "DPadUp") output += DPAD_UP + ">";
            else if (input == "DPadRight") output += DPAD_RIGHT + ">";
            else if (input == "DPadLeft") output += DPAD_LEFT + ">";
            else if (input == "DPadDown") output += DPAD_DOWN + ">";
        }

        return output;
    }

    public string DialogueAttack()
    {
        string[] actions = { "Light", "Medium", "Heavy" };

        Dictionary<string, List<string>> listDict = new Dictionary<string, List<string>>();

        string output = "";

        foreach (string action in actions)
        {
            if (currentScheme.Equals("KeyboardAndMouse"))
            {
                string path = "";

                List<string> prompts = new List<string>();
                for (int i = 0; i < pInput.actions[action].bindings.Count; i++)
                {
                    if (pInput.actions[action].bindings[i].groups.Contains(currentScheme))
                    {
                        Debug.Log("happens");
                        path = pInput.actions[action].bindings[i].path;
                        string temp = path.Substring(path.IndexOf("/") + 1);
                        if (temp.Length > 1)
                        {
                            prompts.Add(temp.Substring(0, 1).ToUpper() + temp.Substring(1));
                        }
                        else
                        {
                            prompts.Add(temp.ToUpper());
                        }
                    }
                }

                listDict[action] = prompts;
            }
            else
            {
                listDict[action] = new List<string>();
                string current = "";
                try
                {
                    current = "<sprite=" + currentSprites[action] + ">";
                }
                catch (KeyNotFoundException e)
                {
                    Debug.Log(action);
                }

                if (currentScheme.Equals("DualShock"))
                {
                    if (currentSprites[action] == 8)
                    {
                        current += "Share";
                    }
                    else if (currentSprites[action] == 9)
                    {
                        current += "Options";
                    }
                }
                listDict[action].Add(current);
            }
        }

        int num = Mathf.Min(listDict["Light"].Count, listDict["Medium"].Count, listDict["Heavy"].Count);
        output = listDict["Light"][0] + ", " + listDict["Medium"][0] + ", and " + listDict["Heavy"][0];
        if (num > 1)
        {
            for (int i = 1; i < num; i++)
            {
                output += " or " + listDict["Light"][i] + ", " + listDict["Medium"][i] + ", and " + listDict["Heavy"][i];
            }
        }

        return output;
    }

    public string DialogueNote(string dir)
    {
        if (currentScheme.Equals("KeyboardAndMouse") || SettingsController.singleton.dInput)
        {
            return DialogueExpl("DPad" + dir);
        }
        else
        {
            switch (dir)
            {
                case "Down":
                    return "<sprite=" + SOUTH + ">";
                case "Right":
                    return "<sprite=" + EAST + ">";
                case "Left":
                    return "<sprite=" + WEST + ">";
                case "Up":
                    return "<sprite=" + NORTH + ">";
                default:
                    return "";
            }
        }
    }

    public Sprite GetSprite(string action, string scheme)
    {
        switch (scheme)
        {
            case "DualShock":
                return dualshockButtons[currentSprites[action]];

            case "Switch":
                return switchButtons[currentSprites[action]];

            default:
                return xboxButtons[currentSprites[action]];
        }
    }

    public string PathParse(string path)
    {
        string output = "";
        if (currentScheme.Equals("KeyboardAndMouse"))
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
        else
        {
            if (path.Contains("buttonSouth")) output = "<sprite=" + SOUTH + ">";
            else if (path.Contains("buttonEast")) output = "<sprite=" + EAST + ">";
            else if (path.Contains("buttonWest")) output = "<sprite=" + WEST + ">";
            else if (path.Contains("buttonNorth")) output = "<sprite=" + NORTH + ">";
            else if (path.Contains("rightShoulder")) output = "<sprite=" + R_SHOULDER + ">";
            else if (path.Contains("rightTrigger")) output = "<sprite=" + R_TRIGGER + ">";
            else if (path.Contains("leftShoulder")) output = "<sprite=" + L_SHOULDER + ">";
            else if (path.Contains("leftTrigger")) output = "<sprite=" + L_TRIGGER + ">";
            else if (path.Contains("select")) output = "<sprite=" + SELECT + ">";
            else if (path.Contains("start")) output = "<sprite=" + START + ">";
        }

        if (currentScheme.Equals("DualShock"))
        {
            if (output.Equals("<sprite=8>"))
            {
                output += "Share";
            }
            else if (output.Equals("<sprite=9>"))
            {
                output += "Options";
            }
        }

        if (output.Equals("LeftButton"))
        {
            output = "<sprite=0>";
        }
        if (output.Equals("RightButton"))
        {
            output = "<sprite=1>";
        }

        return output;
    }
}
