using GameSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace InputControll
{
    public class InputManager : GameSystem
    {
        public Vector2 MousePosition => _input.General.MousePos.ReadValue<Vector2>();

        public ButtonAction PrimaryAction { get; private set; }
        public ButtonAction SecondaryAction { get; private set; }

        public ButtonAction GameResetAction { get; private set; }


        private PlayerInput _input;

        protected override void InitSystem()
        {
            _input = new PlayerInput();

            PrimaryAction = new ButtonAction(_input.General.Primary);
            SecondaryAction = new ButtonAction(_input.General.Secondary);
            GameResetAction = new ButtonAction(_input.General.GameReset);

            _input.Enable();
        }
        protected override void DeinitSystem()
        {
            _input.Disable();
        }


        public class ButtonAction
        {
            public event Action Started;
            public event Action Performed;
            public event Action Ended;
            public event Action<State> Combo;

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
}