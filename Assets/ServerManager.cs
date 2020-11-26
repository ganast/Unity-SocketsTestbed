using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

/**
 * 
 */
public class ServerManager : MonoBehaviour {

    /**
     * 
     */
    private ServerThreadData serverData;

    /**
     * 
     */
    public void Start() {

        Thread serverThread = new Thread(ServerThreadProc);
        serverData = new ServerThreadData();

        Debug.Log("[ServerManager] Starting server thread...");
        serverThread.Start(serverData);
    }

    /**
     * 
     */
    public void Update() {
        lock (serverData.clients) {
            foreach (ClientData clientData in serverData.clients) {
                if (clientData.message != null) {
                    Debug.Log(clientData.message);
                    clientData.message = null;
                }
            }
        }
    }

    /**
     * 
     */
    public static void ClientThreadProc(object o) {

        ClientData clientData = (ClientData) o;

        StreamReader reader = new StreamReader(clientData.client.GetStream());

        while (!clientData.exitRequested) {
            if (clientData.client.Available != 0) {
                string message = reader.ReadLine();
                clientData.message = message;
            }
            else {
                Thread.Yield();
            }
        }

        clientData.client.Close();
    }

    /**
     * 
     */
    public static void ServerThreadProc(object o) {

        Debug.Log("[ServerThread] Server thread started...");

        ServerThreadData serverData = (ServerThreadData) o;

        TcpListener listener = new TcpListener(IPAddress.Loopback, 55000);
        listener.Start();

        while (!serverData.exitRequested) {

            if (listener.Pending()) {

                TcpClient client = listener.AcceptTcpClient();
 
                ClientData clientData = new ClientData();
                clientData.client = client;

                lock(serverData.clients) {
                    serverData.clients.Add(clientData);
                }

                Thread clientThread = new Thread(ClientThreadProc);
                clientThread.Start(clientData);
            }
            else {
                Thread.Yield();
            }
        }

        listener.Stop();

        lock (serverData.clients) {
            foreach (ClientData clientData in serverData.clients) {
                clientData.exitRequested = true;
            }
        }

        Debug.Log("[ServerThread] Server thread ended...");
    }

    /**
     * 
     */
    public class ClientData {

        public bool exitRequested = false;

        public string message = null;

        public TcpClient client;
    }

    /**
     * 
     */
    public class ServerThreadData {
        
        public bool exitRequested = false;

        public List<ClientData> clients = new List<ClientData>();
    }

    /**
     * 
     */
    public void OnDestroy() {
        Debug.Log("[ServerManager] Ending server thread...");
        serverData.exitRequested = true;
    }
}
