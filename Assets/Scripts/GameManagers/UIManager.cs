using System;
using UI_Scripts;
using UnityEngine;

namespace GameManagers
{
    public class UIManager : MonoBehaviour
    {
        private PlayerControls playerControls;
        private LobbyUI lobbyUI;

        private void Awake()
        {
            playerControls = new PlayerControls();
        }

        private void Start()
        {
            playerControls.UI.Esc.performed += _ => ToggleVisibility();
            lobbyUI = FindFirstObjectByType<LobbyUI>(findObjectsInactive: FindObjectsInactive.Include);
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        private void ToggleVisibility()
        {
            if (lobbyUI.gameObject.activeSelf == true)
            {
                lobbyUI.Hide();
            }
            else
            {
                lobbyUI.Show();
            }
        }
    }
}