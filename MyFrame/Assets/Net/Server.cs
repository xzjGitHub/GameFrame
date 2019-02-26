using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

using UnityEngine;


public class SocketState
{
    public Socket socket;

    public byte[] buffer;

    public SocketState(Socket sc)
    {
        socket = sc;
        buffer = new byte[1024];
    }

    public void BeginRecv()
    {
        socket.BeginReceive(buffer,0,1024,SocketFlags.None,AsysRecvCallBack,this);
    }

    private void AsysRecvCallBack(IAsyncResult aysncResult)
    {
        int length = socket.EndReceive(aysncResult);
        string tempstr = Encoding.UTF8.GetString(buffer,0,length);
        Debug.Log("server recive : " + tempstr);

        BeginSend(tempstr);
    }

    public void BeginSend(string data)
    {
        byte[] buf = Encoding.Default.GetBytes(data);
        socket.BeginSend(buf,0,buf.Length,SocketFlags.None,AsysSendCallBack,this);
    }

    private void AsysSendCallBack(IAsyncResult aysncResult)
    {
        int byteSend = socket.EndSend(aysncResult);
        Debug.Log("发送的字节数: " + byteSend);
    }
}

public class Server: MonoBehaviour
{
    private Socket m_server;
    private bool m_isRuning = true;

    private List<SocketState> m_socketArr = new List<SocketState>();


    private void Awake()
    {
        InitSocket();
    }

    public void InitSocket()
    {
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any,18010);
        m_server = new Socket(iPEndPoint.AddressFamily,SocketType.Stream,ProtocolType.Tcp);
        m_server.Bind(iPEndPoint);
        m_server.Listen(0);

        Thread thread = new Thread(ListenRecv);
        thread.Start();
    }

    private void ListenRecv()
    {
        while(m_isRuning)
        {
            try
            {
                m_server.BeginAccept(new AsyncCallback(AsysAcceptCallBack),m_server);
            }
            catch(Exception ex)
            {
                Debug.Log(ex);
            }
            Thread.Sleep(1000);
        }
    }

    //连接回调
    private void AsysAcceptCallBack(IAsyncResult aysncResult)
    {
        Socket server = (Socket)aysncResult.AsyncState;

        Socket clientSocket = server.EndAccept(aysncResult);

        SocketState socketState = new SocketState(clientSocket);
        m_socketArr.Add(socketState);
        socketState.BeginRecv();
    }


    private void Update()
    {
        if(m_socketArr.Count > 0)
        {
            for(int i = 0; i < m_socketArr.Count; i++)
            {
                m_socketArr[i].BeginRecv();
            }
        }
    }

    private void OnApplicationQuit()
    {
        m_server.Shutdown(SocketShutdown.Both);
        m_server.Close(0);
    }
}

