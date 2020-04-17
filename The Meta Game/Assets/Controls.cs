// GENERATED AUTOMATICALLY FROM 'Assets/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""c54f513d-cb57-43bd-8251-ea7e2750b87c"",
            ""actions"": [
                {
                    ""name"": ""MoveH"",
                    ""type"": ""Value"",
                    ""id"": ""e1968b76-1d8b-43b5-b509-6d4cbe67bf5e"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveV"",
                    ""type"": ""Button"",
                    ""id"": ""428f3145-d540-4c4d-b15c-afef1be609ac"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""6802d903-af04-42c0-95fe-eb499ccf3263"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Menu"",
                    ""type"": ""Button"",
                    ""id"": ""cf6a1db4-ccc4-4766-a2a1-ffcc7503b9b9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""7994feba-407e-4a70-a8c4-019f67dbed39"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookX"",
                    ""type"": ""Button"",
                    ""id"": ""79f2801e-c3a3-48a7-a5f5-5eea1e4d1dc2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookY"",
                    ""type"": ""Button"",
                    ""id"": ""1143faf7-4cc2-416c-a913-eb08691660e7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Button"",
                    ""id"": ""96410634-137c-4011-be3f-0222e50c23c5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Light"",
                    ""type"": ""Button"",
                    ""id"": ""058ccb57-d95c-4278-8de1-5e62324dfeca"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Medium"",
                    ""type"": ""Button"",
                    ""id"": ""94b20717-f215-42ef-8ffc-b6924fb0119b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Heavy"",
                    ""type"": ""Button"",
                    ""id"": ""7c07b63e-713d-4341-8238-5cd86bec5ace"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwitchMode"",
                    ""type"": ""Button"",
                    ""id"": ""aab59098-9c87-4d83-9c46-c810c8daa9b2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LStick"",
                    ""type"": ""Value"",
                    ""id"": ""5bef8b01-794b-4dc2-ab38-72d4b793dca3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DPad"",
                    ""type"": ""Value"",
                    ""id"": ""0f3cb9dc-e0c9-49ac-b025-64e64edcf61b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UpNote"",
                    ""type"": ""Button"",
                    ""id"": ""5e49e0c6-5dae-4fb9-bd65-d71bb0f42558"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""dUpNote"",
                    ""type"": ""Button"",
                    ""id"": ""0d3eb96e-362a-44f7-8b74-9ab8642a649f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftNote"",
                    ""type"": ""Button"",
                    ""id"": ""93961454-9f62-41ec-955a-c47b7da60333"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""dLeftNote"",
                    ""type"": ""Button"",
                    ""id"": ""6dc76ab2-adf7-4cb4-8fa5-3da0e10e621c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightNote"",
                    ""type"": ""Button"",
                    ""id"": ""1d5cc52a-2b83-4f92-baed-164af98e1978"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""dRightNote"",
                    ""type"": ""Button"",
                    ""id"": ""7d1b3abf-9aeb-48c6-aa5d-07312859fe40"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DownNote"",
                    ""type"": ""Button"",
                    ""id"": ""55fd097c-3089-4d6f-82f6-c618324de557"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""dDownNote"",
                    ""type"": ""Button"",
                    ""id"": ""6b6285c1-d2ce-480c-b527-8af4e6fbdf32"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""078bd4f6-322b-4e99-8e39-47657f3fd971"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UpJump"",
                    ""type"": ""Button"",
                    ""id"": ""f4f8f7ef-bb43-4501-b88b-c58aa228db51"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""68703882-9843-463d-b865-df3c11d20499"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""32330ca9-e84e-49fa-a25b-1f7444dbc2d1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Horizontal_Key"",
                    ""id"": ""6f5d142b-25aa-4769-8955-1a55d23d64cf"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveH"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""cef670f6-dfca-4320-84e7-197d5ef9bc8a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""MoveH"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""1b157b5e-7465-4253-9ec0-850e560e2f4f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""MoveH"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Horizontal_Key"",
                    ""id"": ""9de7e047-1e0b-4eaa-804a-7073a8e2fd3d"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveH"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""3d1f3276-c93b-43af-a0eb-7790a5cc4fb8"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""MoveH"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""d6ec2bdb-a6cf-47fb-9453-ca64342d12ec"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""MoveH"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0329697f-d383-4b20-bd72-9d8398fb9683"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c156a981-1fb0-45cc-961c-d8e34e705ee0"",
                    ""path"": ""<SwitchProControllerHID>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d6613e60-0cc0-474a-a4a9-ff2de8c74d1c"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Xbox"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""20e25309-79ea-4630-b628-1585e4b1ab0c"",
                    ""path"": ""<DualShockGamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd6e34d9-1570-4c6a-a95a-2e037dd12a3c"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e2c0612-dd68-4ea8-8020-77111e8d4030"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Xbox"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82619040-cce8-4c29-b7de-cd91cd44f09f"",
                    ""path"": ""<SwitchProControllerHID>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""affdfcff-ed9a-4350-b370-bfbc164785f1"",
                    ""path"": ""<DualShockGamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""01f5050b-a176-4f36-ac8a-0fd6d30fddee"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3c0d5a9f-3629-4a4d-bc49-464bb7f2d255"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""35f3c4b2-0259-4070-b134-fdf75bcc1f0e"",
                    ""path"": ""<SwitchProControllerHID>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e233b134-ab57-4fd8-9c5e-ee0aeabc7bbf"",
                    ""path"": ""<DualShockGamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""20299457-f12b-42e2-803a-b67cf949dd15"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""LookX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9001b887-b4d7-4fb0-b135-6625fa1910d0"",
                    ""path"": ""<Gamepad>/rightStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Xbox"",
                    ""action"": ""LookX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c4d98ebc-ca22-4bf2-9e31-f154a6e4fc1e"",
                    ""path"": ""<DualShockGamepad>/rightStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""LookX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""081e5156-e641-46aa-8e3e-928972989722"",
                    ""path"": ""<SwitchProControllerHID>/rightStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""LookX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""13e0eaa5-2348-4318-a062-e43441ba6dfb"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox;Gamepad"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dd0f2fd2-89c6-4d40-8af2-5ea017bb2574"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4fbe8fd9-a0d8-40c9-9550-abbd0fb7a7a2"",
                    ""path"": ""<SwitchProControllerHID>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""08684874-dff9-40a3-b9c2-a158e7d8e292"",
                    ""path"": ""<DualShockGamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""110bb571-814e-4651-b60b-16e01c7883df"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox;Gamepad"",
                    ""action"": ""Light"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b5861abd-df2d-4e0d-87af-4355973ad71c"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Light"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""02d88c56-9e57-4b96-9436-09645e83d7d1"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Light"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7451e3d7-ee4b-4c8a-87e4-b1b124b17ffa"",
                    ""path"": ""<SwitchProControllerHID>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""Light"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""69f5bd0c-996f-436f-a3fd-8e35d44338f7"",
                    ""path"": ""<DualShockGamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""Light"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0556b9db-c350-47e6-ae88-1c91b9f355ca"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox;Gamepad"",
                    ""action"": ""Medium"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""74995455-0726-40fc-a3cf-50e46a4efa11"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Medium"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea5a8831-94ff-423d-994c-30c4e07597d9"",
                    ""path"": ""<Keyboard>/o"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Medium"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""70d96188-648d-4506-b5bc-9b723653ed54"",
                    ""path"": ""<SwitchProControllerHID>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""Medium"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6b59e498-d68f-4ccd-b21b-4e539833c103"",
                    ""path"": ""<DualShockGamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""Medium"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0dd0bfc9-3cd4-428e-ad38-b9bd471cb6e2"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox;Gamepad"",
                    ""action"": ""Heavy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""83ca37d6-35bc-4ec0-b475-42c5774c625c"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Heavy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ba74b918-f91a-4e93-843f-c3b8d0f507e6"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Heavy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""55ca16c8-d7bb-41ea-81e2-a269ade7169e"",
                    ""path"": ""<SwitchProControllerHID>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""Heavy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f9ced8f5-d3e2-4e8f-8c39-663d98e4f483"",
                    ""path"": ""<DualShockGamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""Heavy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Switch_Key"",
                    ""id"": ""1201ace6-2f1d-44b7-9ffd-ad112acb4dcb"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""6778af1a-cb13-469d-9073-2e207100914d"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""e355e0b2-5473-4f05-926e-f7500f679e6d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Switch_Gamepad"",
                    ""id"": ""7ec7e611-343d-4ca2-8660-30cc561d2e47"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""0c421c3f-aa9d-4969-af23-7275ce6dd802"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox;Gamepad"",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""e14cd7f0-6dd2-48a7-8b67-91833d1a5bb9"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox;Gamepad"",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Switch_Dualshock"",
                    ""id"": ""178ad828-c477-4f53-ac5e-22d5a4ff6bfc"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""e44d2f9c-4d49-42d5-ae1e-711b1ce4f12a"",
                    ""path"": ""<DualShockGamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""f5622528-2c52-473b-97c8-693a585113fc"",
                    ""path"": ""<DualShockGamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Switch_Dualshock"",
                    ""id"": ""a62f810d-398c-440d-a268-a213a2e01e18"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""5409aa5f-9f83-4451-994f-abd605a1d5d9"",
                    ""path"": ""<SwitchProControllerHID>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""856ed677-3688-438e-a524-b4e6de338c1c"",
                    ""path"": ""<SwitchProControllerHID>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""SwitchMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Vertical_Key"",
                    ""id"": ""76b937b8-95dd-417d-a307-ba4566558891"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveV"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""7e163a8f-a81f-4565-9cb9-dd219082cd08"",
                    ""path"": ""<Keyboard>/#(S)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""MoveV"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""a1395294-30b7-445a-9f42-71af514b97f7"",
                    ""path"": ""<Keyboard>/#(W)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""MoveV"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Vertical_Key"",
                    ""id"": ""dd6c338f-8243-4b86-b001-0e6aa0b0aef5"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveV"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""ed0be4d3-bd4c-4a4e-a60f-f69de5b62ded"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""MoveV"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""e845e496-309f-4a5e-9812-49c344b85193"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""MoveV"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a4822b04-6ba1-4b20-9098-505ee51fb15f"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""LookY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6e2243f0-0292-452d-8701-9338ae7e336f"",
                    ""path"": ""<Gamepad>/rightStick/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox;Gamepad"",
                    ""action"": ""LookY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c46f0840-1f25-49d7-a4a0-3833c53777f3"",
                    ""path"": ""<DualShockGamepad>/rightStick/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""LookY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea0b3db0-907c-4ea8-986d-178da5511b07"",
                    ""path"": ""<SwitchProControllerHID>/rightStick/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""LookY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2a4b188b-06c5-4634-8824-154fb6715cc7"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox;Gamepad"",
                    ""action"": ""LStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48f9c805-5891-4428-b236-2b0b0995a080"",
                    ""path"": ""<DualShockGamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""LStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""11428a55-d5b4-4bb1-904d-832407d24854"",
                    ""path"": ""<SwitchProControllerHID>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""LStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""91202d88-0f8a-4c19-80e0-097cae7c6d0d"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox;Gamepad"",
                    ""action"": ""DPad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4aa244d5-0bd9-4e79-a586-a0fdf50b2909"",
                    ""path"": ""<DualShockGamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""DPad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8f1f24a3-1b7b-419d-ad95-8dff1872f50a"",
                    ""path"": ""<SwitchProControllerHID>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""DPad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f4635758-d6b7-4714-b91b-8627fe2632f9"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""UpNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b2de89ea-fc14-423f-9143-5b4bd9ecdbbf"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""UpNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d9c3a2c1-1308-431b-837a-9741eb03a9c8"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Xbox;Switch;DualShock"",
                    ""action"": ""UpNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""42117292-697e-41eb-871e-faba5743f499"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""LeftNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b40fc76e-5eba-485e-9ae9-0d6c7db33dd1"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""LeftNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0e4a3c2c-3d82-4df9-8477-526287fa69dd"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Gamepad;Switch"",
                    ""action"": ""LeftNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cced21f8-50a5-407d-9444-82818ddb622c"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""RightNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""785a7be8-b9b6-4105-8eb1-e40a99289f20"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""RightNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f524e256-74b8-4357-976f-e49debdb6d05"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""RightNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea0dc155-dccd-4d6c-8626-645c9cd225fa"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""DownNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""837e1f29-f104-4137-ae2f-4be2c6680e20"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""DownNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""799717f7-7f4a-4589-b10b-70a8c8643a8e"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""DownNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""25be4c1d-ec5d-4a04-b3ea-b70d67d58262"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5952475e-3d96-4047-a8ae-66685322cf3f"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox;Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce914b3a-93ae-4c42-93b2-731df484cd61"",
                    ""path"": ""<DualShockGamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c7fe3906-e675-4666-8cff-65f8cd05a37f"",
                    ""path"": ""<SwitchProControllerHID>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e1a0af0-6cc6-4f08-9e50-17412e0385c9"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""UpJump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f1209355-4a38-41ca-b725-8483774c402e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""UpJump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ba76a0f4-1f86-4bf5-86e5-961817715a67"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""UpJump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""150f0df0-a815-4ea3-8217-77d125193a8b"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""UpJump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4eaceb38-71c8-4f4e-8c04-fc992bd99227"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""dUpNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f20c79df-cb08-42c7-aa92-a22f324673b3"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""dUpNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""829650a8-1660-41d9-a32d-6e45a4f2f620"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Switch;Xbox;DualShock"",
                    ""action"": ""dLeftNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c4efc92-c9c2-4cd0-a692-02ff10437be9"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""dLeftNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bf4c8428-e11a-4239-a0bd-ac9ded1f22e6"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""dRightNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f49d9708-18ae-41c3-9457-812cff11dc35"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""dRightNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""58950e9e-1e18-4477-b085-65d71949ddc2"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""dDownNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""01d011f8-2c60-4544-8f1b-e6fcbbea2b26"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Switch;Gamepad"",
                    ""action"": ""dDownNote"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f949960e-49ed-4981-ad24-c41bdad65e93"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox;DualShock;Gamepad"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d47ae92e-46e5-4323-82ab-cc7b2fd29124"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""575c6b94-14c4-48f3-b620-e0c9597d7aaf"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2dc916d4-0401-46a2-807a-452387531aeb"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b4798c9-b608-4a0a-8d58-fd402caf9bfa"",
                    ""path"": ""<SwitchProControllerHID>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b041b61d-a0da-478d-b449-e555d9fe0ce6"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DualShock;Xbox;Gamepad"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""18aa1f7a-542e-4aef-9e57-619dd10974e1"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85f6ade4-d3ac-46c0-a207-802a089c7b9e"",
                    ""path"": ""<SwitchProControllerHID>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Switch"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""94516c8a-cbab-426e-ba4c-2c8973790a7c"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""27de26c0-f38d-4240-ab0f-47c5240ec35b"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""DualShock"",
            ""bindingGroup"": ""DualShock"",
            ""devices"": [
                {
                    ""devicePath"": ""<DualShockGamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""KeyboardAndMouse"",
            ""bindingGroup"": ""KeyboardAndMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Xbox"",
            ""bindingGroup"": ""Xbox"",
            ""devices"": [
                {
                    ""devicePath"": ""<XInputController>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Switch"",
            ""bindingGroup"": ""Switch"",
            ""devices"": [
                {
                    ""devicePath"": ""<SwitchProControllerHID>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_MoveH = m_Player.FindAction("MoveH", throwIfNotFound: true);
        m_Player_MoveV = m_Player.FindAction("MoveV", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Menu = m_Player.FindAction("Menu", throwIfNotFound: true);
        m_Player_Fire = m_Player.FindAction("Fire", throwIfNotFound: true);
        m_Player_LookX = m_Player.FindAction("LookX", throwIfNotFound: true);
        m_Player_LookY = m_Player.FindAction("LookY", throwIfNotFound: true);
        m_Player_Zoom = m_Player.FindAction("Zoom", throwIfNotFound: true);
        m_Player_Light = m_Player.FindAction("Light", throwIfNotFound: true);
        m_Player_Medium = m_Player.FindAction("Medium", throwIfNotFound: true);
        m_Player_Heavy = m_Player.FindAction("Heavy", throwIfNotFound: true);
        m_Player_SwitchMode = m_Player.FindAction("SwitchMode", throwIfNotFound: true);
        m_Player_LStick = m_Player.FindAction("LStick", throwIfNotFound: true);
        m_Player_DPad = m_Player.FindAction("DPad", throwIfNotFound: true);
        m_Player_UpNote = m_Player.FindAction("UpNote", throwIfNotFound: true);
        m_Player_dUpNote = m_Player.FindAction("dUpNote", throwIfNotFound: true);
        m_Player_LeftNote = m_Player.FindAction("LeftNote", throwIfNotFound: true);
        m_Player_dLeftNote = m_Player.FindAction("dLeftNote", throwIfNotFound: true);
        m_Player_RightNote = m_Player.FindAction("RightNote", throwIfNotFound: true);
        m_Player_dRightNote = m_Player.FindAction("dRightNote", throwIfNotFound: true);
        m_Player_DownNote = m_Player.FindAction("DownNote", throwIfNotFound: true);
        m_Player_dDownNote = m_Player.FindAction("dDownNote", throwIfNotFound: true);
        m_Player_Pause = m_Player.FindAction("Pause", throwIfNotFound: true);
        m_Player_UpJump = m_Player.FindAction("UpJump", throwIfNotFound: true);
        m_Player_Submit = m_Player.FindAction("Submit", throwIfNotFound: true);
        m_Player_Cancel = m_Player.FindAction("Cancel", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_MoveH;
    private readonly InputAction m_Player_MoveV;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Menu;
    private readonly InputAction m_Player_Fire;
    private readonly InputAction m_Player_LookX;
    private readonly InputAction m_Player_LookY;
    private readonly InputAction m_Player_Zoom;
    private readonly InputAction m_Player_Light;
    private readonly InputAction m_Player_Medium;
    private readonly InputAction m_Player_Heavy;
    private readonly InputAction m_Player_SwitchMode;
    private readonly InputAction m_Player_LStick;
    private readonly InputAction m_Player_DPad;
    private readonly InputAction m_Player_UpNote;
    private readonly InputAction m_Player_dUpNote;
    private readonly InputAction m_Player_LeftNote;
    private readonly InputAction m_Player_dLeftNote;
    private readonly InputAction m_Player_RightNote;
    private readonly InputAction m_Player_dRightNote;
    private readonly InputAction m_Player_DownNote;
    private readonly InputAction m_Player_dDownNote;
    private readonly InputAction m_Player_Pause;
    private readonly InputAction m_Player_UpJump;
    private readonly InputAction m_Player_Submit;
    private readonly InputAction m_Player_Cancel;
    public struct PlayerActions
    {
        private @Controls m_Wrapper;
        public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveH => m_Wrapper.m_Player_MoveH;
        public InputAction @MoveV => m_Wrapper.m_Player_MoveV;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Menu => m_Wrapper.m_Player_Menu;
        public InputAction @Fire => m_Wrapper.m_Player_Fire;
        public InputAction @LookX => m_Wrapper.m_Player_LookX;
        public InputAction @LookY => m_Wrapper.m_Player_LookY;
        public InputAction @Zoom => m_Wrapper.m_Player_Zoom;
        public InputAction @Light => m_Wrapper.m_Player_Light;
        public InputAction @Medium => m_Wrapper.m_Player_Medium;
        public InputAction @Heavy => m_Wrapper.m_Player_Heavy;
        public InputAction @SwitchMode => m_Wrapper.m_Player_SwitchMode;
        public InputAction @LStick => m_Wrapper.m_Player_LStick;
        public InputAction @DPad => m_Wrapper.m_Player_DPad;
        public InputAction @UpNote => m_Wrapper.m_Player_UpNote;
        public InputAction @dUpNote => m_Wrapper.m_Player_dUpNote;
        public InputAction @LeftNote => m_Wrapper.m_Player_LeftNote;
        public InputAction @dLeftNote => m_Wrapper.m_Player_dLeftNote;
        public InputAction @RightNote => m_Wrapper.m_Player_RightNote;
        public InputAction @dRightNote => m_Wrapper.m_Player_dRightNote;
        public InputAction @DownNote => m_Wrapper.m_Player_DownNote;
        public InputAction @dDownNote => m_Wrapper.m_Player_dDownNote;
        public InputAction @Pause => m_Wrapper.m_Player_Pause;
        public InputAction @UpJump => m_Wrapper.m_Player_UpJump;
        public InputAction @Submit => m_Wrapper.m_Player_Submit;
        public InputAction @Cancel => m_Wrapper.m_Player_Cancel;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @MoveH.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveH;
                @MoveH.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveH;
                @MoveH.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveH;
                @MoveV.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveV;
                @MoveV.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveV;
                @MoveV.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveV;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Menu.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
                @Menu.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
                @Menu.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
                @Fire.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                @Fire.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                @Fire.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                @LookX.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookX;
                @LookX.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookX;
                @LookX.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookX;
                @LookY.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookY;
                @LookY.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookY;
                @LookY.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookY;
                @Zoom.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoom;
                @Zoom.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoom;
                @Zoom.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoom;
                @Light.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLight;
                @Light.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLight;
                @Light.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLight;
                @Medium.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMedium;
                @Medium.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMedium;
                @Medium.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMedium;
                @Heavy.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHeavy;
                @Heavy.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHeavy;
                @Heavy.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHeavy;
                @SwitchMode.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchMode;
                @SwitchMode.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchMode;
                @SwitchMode.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchMode;
                @LStick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLStick;
                @LStick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLStick;
                @LStick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLStick;
                @DPad.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDPad;
                @DPad.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDPad;
                @DPad.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDPad;
                @UpNote.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUpNote;
                @UpNote.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUpNote;
                @UpNote.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUpNote;
                @dUpNote.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDUpNote;
                @dUpNote.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDUpNote;
                @dUpNote.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDUpNote;
                @LeftNote.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftNote;
                @LeftNote.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftNote;
                @LeftNote.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftNote;
                @dLeftNote.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDLeftNote;
                @dLeftNote.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDLeftNote;
                @dLeftNote.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDLeftNote;
                @RightNote.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightNote;
                @RightNote.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightNote;
                @RightNote.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightNote;
                @dRightNote.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDRightNote;
                @dRightNote.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDRightNote;
                @dRightNote.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDRightNote;
                @DownNote.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDownNote;
                @DownNote.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDownNote;
                @DownNote.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDownNote;
                @dDownNote.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDDownNote;
                @dDownNote.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDDownNote;
                @dDownNote.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDDownNote;
                @Pause.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @UpJump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUpJump;
                @UpJump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUpJump;
                @UpJump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUpJump;
                @Submit.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubmit;
                @Submit.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubmit;
                @Submit.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubmit;
                @Cancel.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveH.started += instance.OnMoveH;
                @MoveH.performed += instance.OnMoveH;
                @MoveH.canceled += instance.OnMoveH;
                @MoveV.started += instance.OnMoveV;
                @MoveV.performed += instance.OnMoveV;
                @MoveV.canceled += instance.OnMoveV;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Menu.started += instance.OnMenu;
                @Menu.performed += instance.OnMenu;
                @Menu.canceled += instance.OnMenu;
                @Fire.started += instance.OnFire;
                @Fire.performed += instance.OnFire;
                @Fire.canceled += instance.OnFire;
                @LookX.started += instance.OnLookX;
                @LookX.performed += instance.OnLookX;
                @LookX.canceled += instance.OnLookX;
                @LookY.started += instance.OnLookY;
                @LookY.performed += instance.OnLookY;
                @LookY.canceled += instance.OnLookY;
                @Zoom.started += instance.OnZoom;
                @Zoom.performed += instance.OnZoom;
                @Zoom.canceled += instance.OnZoom;
                @Light.started += instance.OnLight;
                @Light.performed += instance.OnLight;
                @Light.canceled += instance.OnLight;
                @Medium.started += instance.OnMedium;
                @Medium.performed += instance.OnMedium;
                @Medium.canceled += instance.OnMedium;
                @Heavy.started += instance.OnHeavy;
                @Heavy.performed += instance.OnHeavy;
                @Heavy.canceled += instance.OnHeavy;
                @SwitchMode.started += instance.OnSwitchMode;
                @SwitchMode.performed += instance.OnSwitchMode;
                @SwitchMode.canceled += instance.OnSwitchMode;
                @LStick.started += instance.OnLStick;
                @LStick.performed += instance.OnLStick;
                @LStick.canceled += instance.OnLStick;
                @DPad.started += instance.OnDPad;
                @DPad.performed += instance.OnDPad;
                @DPad.canceled += instance.OnDPad;
                @UpNote.started += instance.OnUpNote;
                @UpNote.performed += instance.OnUpNote;
                @UpNote.canceled += instance.OnUpNote;
                @dUpNote.started += instance.OnDUpNote;
                @dUpNote.performed += instance.OnDUpNote;
                @dUpNote.canceled += instance.OnDUpNote;
                @LeftNote.started += instance.OnLeftNote;
                @LeftNote.performed += instance.OnLeftNote;
                @LeftNote.canceled += instance.OnLeftNote;
                @dLeftNote.started += instance.OnDLeftNote;
                @dLeftNote.performed += instance.OnDLeftNote;
                @dLeftNote.canceled += instance.OnDLeftNote;
                @RightNote.started += instance.OnRightNote;
                @RightNote.performed += instance.OnRightNote;
                @RightNote.canceled += instance.OnRightNote;
                @dRightNote.started += instance.OnDRightNote;
                @dRightNote.performed += instance.OnDRightNote;
                @dRightNote.canceled += instance.OnDRightNote;
                @DownNote.started += instance.OnDownNote;
                @DownNote.performed += instance.OnDownNote;
                @DownNote.canceled += instance.OnDownNote;
                @dDownNote.started += instance.OnDDownNote;
                @dDownNote.performed += instance.OnDDownNote;
                @dDownNote.canceled += instance.OnDDownNote;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @UpJump.started += instance.OnUpJump;
                @UpJump.performed += instance.OnUpJump;
                @UpJump.canceled += instance.OnUpJump;
                @Submit.started += instance.OnSubmit;
                @Submit.performed += instance.OnSubmit;
                @Submit.canceled += instance.OnSubmit;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_DualShockSchemeIndex = -1;
    public InputControlScheme DualShockScheme
    {
        get
        {
            if (m_DualShockSchemeIndex == -1) m_DualShockSchemeIndex = asset.FindControlSchemeIndex("DualShock");
            return asset.controlSchemes[m_DualShockSchemeIndex];
        }
    }
    private int m_KeyboardAndMouseSchemeIndex = -1;
    public InputControlScheme KeyboardAndMouseScheme
    {
        get
        {
            if (m_KeyboardAndMouseSchemeIndex == -1) m_KeyboardAndMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardAndMouse");
            return asset.controlSchemes[m_KeyboardAndMouseSchemeIndex];
        }
    }
    private int m_XboxSchemeIndex = -1;
    public InputControlScheme XboxScheme
    {
        get
        {
            if (m_XboxSchemeIndex == -1) m_XboxSchemeIndex = asset.FindControlSchemeIndex("Xbox");
            return asset.controlSchemes[m_XboxSchemeIndex];
        }
    }
    private int m_SwitchSchemeIndex = -1;
    public InputControlScheme SwitchScheme
    {
        get
        {
            if (m_SwitchSchemeIndex == -1) m_SwitchSchemeIndex = asset.FindControlSchemeIndex("Switch");
            return asset.controlSchemes[m_SwitchSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMoveH(InputAction.CallbackContext context);
        void OnMoveV(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnMenu(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnLookX(InputAction.CallbackContext context);
        void OnLookY(InputAction.CallbackContext context);
        void OnZoom(InputAction.CallbackContext context);
        void OnLight(InputAction.CallbackContext context);
        void OnMedium(InputAction.CallbackContext context);
        void OnHeavy(InputAction.CallbackContext context);
        void OnSwitchMode(InputAction.CallbackContext context);
        void OnLStick(InputAction.CallbackContext context);
        void OnDPad(InputAction.CallbackContext context);
        void OnUpNote(InputAction.CallbackContext context);
        void OnDUpNote(InputAction.CallbackContext context);
        void OnLeftNote(InputAction.CallbackContext context);
        void OnDLeftNote(InputAction.CallbackContext context);
        void OnRightNote(InputAction.CallbackContext context);
        void OnDRightNote(InputAction.CallbackContext context);
        void OnDownNote(InputAction.CallbackContext context);
        void OnDDownNote(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnUpJump(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
    }
}
