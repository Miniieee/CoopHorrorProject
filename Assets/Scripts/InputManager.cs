using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    private static InputManager instance;

    public static InputManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        playerControls = new PlayerControls();
    }


    public Vector2 GetPlayerMovement()
    {
        return playerControls.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool GetSprint()
    {
        return playerControls.Player.Sprint.IsPressed();
    }


    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
