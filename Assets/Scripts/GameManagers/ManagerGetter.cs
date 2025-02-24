using UnityEngine;

public class ManagerGetter : MonoBehaviour
{
    public static ManagerGetter Instance { get; private set; }

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
    
    public void GetManagerInChildren<T>(out T manager)
    {
        manager = GetComponentInChildren<T>();
    }

    public void GetNetworkManager<T>(out T manager)
    {
        manager = GetComponent<T>();
    }


}
