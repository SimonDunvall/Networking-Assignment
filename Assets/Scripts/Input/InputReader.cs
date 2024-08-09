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

        public event UnityAction<Vector2> MoveEvent = delegate { };
    }
}