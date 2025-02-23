using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Scripts
{
    public class LobbyCreateUI : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button createPublicButton;
        [SerializeField] private Button createPrtivateButton;
        [SerializeField] private TMP_InputField lobbyNameInputField;

        [SerializeField] private CharacterSelectionUI characterSelectUI;

        private void Awake()
        {
            closeButton.onClick.AddListener(Hide);
            createPublicButton.onClick.AddListener(() =>
                LobbyServices.Instance.CreateLobby(lobbyNameInputField.text, false));
            createPrtivateButton.onClick.AddListener(() => StartLobbyCreateUI());
        }


        private void StartLobbyCreateUI()
        {
            LobbyServices.Instance.CreateLobby(lobbyNameInputField.text, true);
        }

        void Start()
        {
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
