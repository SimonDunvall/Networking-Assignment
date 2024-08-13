using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input/Input Reader")]
    public class InputReader : ScriptableObject, GameInput.IGameplayActions
    {
        private GameInput GameInput;

        internal bool IsShooting { get; private set; }
        internal Vector2 MousePosition { get; private set; }

        private void OnEnable()
        {
            if (GameInput != null) return;
            GameInput = new GameInput();
            GameInput.Gameplay.SetCallbacks(this);
            GameInput.Gameplay.Enable();
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MoveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnShot(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                IsShooting = true;
                ShotEvent.Invoke(MousePosition);
            }
            else if (context.canceled)
            {
                IsShooting = false;
            }
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            var mousePosition = GameInput.Gameplay.MousePosition.ReadValue<Vector2>();
            var worldPosition = (Vector2)Camera.main!.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y,
                Camera.main.transform.position.z));
            MousePosition = worldPosition;
        }

        public void OnSendChat(InputAction.CallbackContext context)
        {
            if (context.performed) SendEvent.Invoke();
        }

        public void OnInputFieldSelected()
        {
            GameInput.Gameplay.MousePosition.Disable();
            GameInput.Gameplay.Shot.Disable();
            GameInput.Gameplay.Movement.Disable();
        }

        public void OnInputFieldDeselected()
        {
            GameInput.Gameplay.MousePosition.Enable();
            GameInput.Gameplay.Shot.Enable();
            GameInput.Gameplay.Movement.Enable();
        }


        public event UnityAction<Vector2> MoveEvent = delegate { };
        public event UnityAction<Vector2> ShotEvent = delegate { };
        public event UnityAction SendEvent = delegate { };
    }
}