using UnityEngine;

namespace GameManagers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}