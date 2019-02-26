using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

using UnityEngine;

public class Client: MonoBehaviour
{
    private Socket m_client;
    private SocketBuffer m_socketBuffer;

    public byte[] byteData = new byte[1024];

    private void Awake()
    {
        m_socketBuffer = new SocketBuffer(6,RecvMsgOver);
        Init();
    }

    public void RecvMsgOver(byte[] data)
    {

    }

    private void Init()
    {
        IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint iPEndPoint = new IPEndPoint(iPAddress,18010);

        m_client = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

        m_client.BeginConnect(iPEndPoint,new AsyncCallback(ConnetCallBack),m_client);
    }


    private void ConnetCallBack(IAsyncResult asyncResult)
    {
        m_client.EndConnect(asyncResult);

        Debug.Log("connect fnish");
    }


    public void BeginRecv()
    {
        m_client.BeginReceive(byteData,0,1024,SocketFlags.None,AsysRecvCallBack,this);
    }

    private void AsysRecvCallBack(IAsyncResult aysncResult)
    {
        int length = m_client.EndReceive(aysncResult);
        //  string tempstr = Encoding.UTF8.GetString(byteData,0,length);

        m_socketBuffer.ReciveByte(byteData,length);
        
        //  Debug.Log("client recive : " + tempstr);
    }


    public void BeginSend(string data)
    {
        byte[] buf = Encoding.Default.GetBytes(data);
        m_client.BeginSend(buf,0,buf.Length,SocketFlags.None,AsysSendCallBack,this);
    }

    private void AsysSendCallBack(IAsyncResult aysncResult)
    {
        int byteSend = m_client.EndSend(aysncResult);
        Debug.Log("发送的字节数: " + byteSend);
    }


    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            BeginSend("12345678");
        }

        BeginRecv();
    }

}

