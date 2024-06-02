using UnityEngine;

public class MultiplayerConnectionData : MonoBehaviour
{
    public enum NetworkRole{
        None,
        Server,
        Host,
        Client
    }

    public static string ServerIP;
    public static NetworkRole networkRole; 
}
