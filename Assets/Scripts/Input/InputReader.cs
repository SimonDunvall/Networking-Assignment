using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input/Input Reader")]
    public class InputReader : ScriptableObject, GameInput.IGameplayActions
    {
        private GameInput GameInput;

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
            if (!context.performed) return;
            var mousePosition = GameInput.Gameplay.MousePosition.ReadValue<Vector2>();

            var worldPosition = (Vector2)Camera.main!.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y,
                Camera.main.transform.position.z));
            ShotEvent.Invoke(worldPosition);
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
        }

        public event UnityAction<Vector2> MoveEvent = delegate { };
        public event UnityAction<Vector2> ShotEvent = delegate { };
    }
}