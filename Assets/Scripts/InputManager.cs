using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 MousePosition => instance.input.General.MousePos.ReadValue<Vector2>();

    public static ButtonAction PrimaryAction { get; private set; }
    public static ButtonAction SecondaryAction { get; private set; }

    private static InputManager instance;

    private PlayerInput input;

    void Awake()
    {
        Assert.IsNull(instance, $"Duplicate {nameof(InputManager)} on {this}");
        instance = this;
        input = new PlayerInput();

        PrimaryAction = new ButtonAction(input.General.Primary);
        SecondaryAction = new ButtonAction(input.General.Secondary);
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable(); 
    }

    public class ButtonAction
    {
        public static event Action Started;
        public static event Action Performed;
        public static event Action Ended;
        public static event Action<State> Combo;

        public ButtonAction(InputAction action)
        {
            action.started += (_) => { Started?.Invoke(); Combo?.Invoke(State.Started); };
            action.performed += (_) => { Performed?.Invoke(); Combo?.Invoke(State.Performed); };
            action.canceled += (_) => { Ended?.Invoke(); Combo?.Invoke(State.Ended); };
        }

        public enum State
        {
            Started,
            Performed,
            Ended
        }
    }
}
