using UnityEditor;
using Unity.Netcode; // Or using Mirror; if you're using Mirror Networking
using Sirenix.OdinInspector.Editor;

namespace Editor
{
    
[CustomEditor(typeof(NetworkBehaviour), true)]
public class OdinNetworkBehaviourEditor : OdinEditor
{
}
}
