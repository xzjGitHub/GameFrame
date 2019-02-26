using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


public class NetManager
{

    private Queue<NetMessageBase> m_recvMsgPool;
    private Queue<NetMessageBase> m_sendMsgPool;

    private NetSocket m_clientSocket;
    private Thread m_sendThead;

    public NetManager(string ip,ushort port)
    {
        m_recvMsgPool = new Queue<NetMessageBase>();
        m_sendMsgPool = new Queue<NetMessageBase>();

        m_clientSocket = new NetSocket();

        m_clientSocket.AsyncConnect(ip,port,ConnectCallBack,RecvCallBack);
    }

    public void SendMessage(NetMessageBase msg)
    {
        lock(m_sendMsgPool)
        {
            m_sendMsgPool.Enqueue(msg);
        }
    }

    public void ConnectCallBack(bool sucess,NetSocket.SoketError soketError,string exception)
    {
        if(sucess)
        {
            m_sendThead = new Thread(LoopSendMsg);
            m_sendThead.Start();
        }
    }

    public void SendCallBack(bool sucess,NetSocket.SoketError soketError,string exception)
    {
        if(sucess)
        {

        }
    }

    public void RecvCallBack(bool sucess,NetSocket.SoketError soketError,string exception,
        byte[] data,string message)
    {
        if(sucess)
        {
            NetMessageBase msg = new NetMessageBase(data);
            m_recvMsgPool.Enqueue(msg);
        }
    }

    public void Tick()
    {
        if(m_recvMsgPool != null)
        {
            while(m_recvMsgPool.Count > 0)
            {
                HandleMessage(m_recvMsgPool.Dequeue());
            }
        }
    }

    private void HandleMessage(NetMessageBase msg)
    {
        //处理消息
        //MsgCenter.Instance.SendToMsg(msg);
    }


    #region send


    private void LoopSendMsg()
    {
        while(m_clientSocket != null && m_clientSocket.IsConnect())
        {
            lock(m_sendMsgPool)
            {
                while(m_sendMsgPool.Count > 0)
                {
                    NetMessageBase msg = m_sendMsgPool.Dequeue();

                    m_clientSocket.AsynSend(msg.BufferData,SendCallBack);
                }
            }
            Thread.Sleep(100);
        }
    }

    #endregion

    #region disconnet
    public void Disconnet()
    {
        if (m_clientSocket != null)
        {
            m_clientSocket.AynsDisConnect(DisconnectCallBack);
        }
    }

    public void DisconnectCallBack(bool sucess,NetSocket.SoketError soketError,string exception)
    {
        if (sucess)
        {
            m_sendThead.Abort();
        }
    }

    #endregion

}

