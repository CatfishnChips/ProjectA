using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public static class NetworkUtils
{
    public static string GetLocalIPAddress()
    {
        string localIP = "";
        try
        {
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error getting local IP address: {ex.Message}");
        }

        if (string.IsNullOrEmpty(localIP))
        {
            Debug.LogError("Local IP Address Not Found!");
        }

        return localIP;
    }

    public static int GetPort()
    {
        int port = -1;
        try
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport != null)
            {
                port = transport.ConnectionData.Port;
            }
            else
            {
                Debug.LogError("UnityTransport component not found on NetworkManager.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error getting port number: {ex.Message}");
        }

        if (port == -1)
        {
            Debug.LogError("Port number not set or found!");
        }

        return port;
    }

}