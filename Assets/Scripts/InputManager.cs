using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InputManager : MonoBehaviour
{
    public static Vector2 MousePosition => instance.input.General.MousePos.ReadValue<Vector2>();

    public static event Action PrimaryAction;
    public static event Action SecondaryAction;

    private static InputManager instance;

    private PlayerInput input;

    void Awake()
    {
        Assert.IsNull(instance, $"Duplicate {nameof(InputManager)} on {this}");
        instance = this;
        input = new PlayerInput();

        input.General.Primary.started += (_) => PrimaryAction?.Invoke();
        input.General.Secondary.started += (_) => SecondaryAction?.Invoke();
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

        public enum State
        {
            Started,
            Performed,
            Ended
        }
    }
}
