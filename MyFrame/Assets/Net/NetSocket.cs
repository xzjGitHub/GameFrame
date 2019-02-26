using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;

public class NetSocket
{

    public delegate void CallBackNormal(bool sucess,SoketError soketError,string exception);
    public delegate void CallBackRecv(bool sucess,SoketError soketError,string exception,byte[] data,string message);

    public enum SoketError
    {
        Sucess = 0,


        TimeOut,
        SocketNull,
        SocketUnConnect,

        ConnectSucess,
        ConnectError,

        SendUnSucess,
        SendUnError,

        RecivSucess,
        ReciveUnSucess,

        DisConnect,
        DisConnectSucess,
        DisConnectError,

        Unknow,
    }

    private SocketError m_errorSocket;

    private CallBackNormal callBackConnect;
    private CallBackNormal callBackSend;
    private CallBackNormal callBackDisconnect;

    private CallBackRecv callBackRecv;

    private Socket clientSocket;
    private string adressIp;
    private ushort port;
    private SocketBuffer m_socketBuffer;

    private byte[] m_recvBuffer;

    private byte headLengh = 6;

    public NetSocket()
    {
        m_socketBuffer = new SocketBuffer(headLengh,RecvMsgOver);
        m_recvBuffer = new byte[1024];
    }

    #region connect

    public void AsyncConnect(string ip,ushort port,CallBackNormal callBackConnect,CallBackRecv callBackRecv)
    {
        m_errorSocket = SocketError.Success;
        this.callBackConnect = callBackConnect;
        this.callBackRecv = callBackRecv;

        if(clientSocket != null && clientSocket.Connected)
        {
            this.callBackConnect(false,SoketError.ConnectError,"connect repeat");
        }
        else if(clientSocket == null || !clientSocket.Connected)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress,18010);
            IAsyncResult asyncResult = clientSocket.BeginConnect(iPEndPoint,ConnetCallBack,clientSocket);
            if(!TimeOutCheck(asyncResult))
            {
                this.callBackConnect(false,SoketError.TimeOut,"connect timeout");
            }
        }
    }

    private void ConnetCallBack(IAsyncResult asyncResult)
    {
        try
        {
            clientSocket.EndConnect(asyncResult);
            if(clientSocket.Connected == false)
            {
                callBackConnect(false,SoketError.ConnectError,"connect fail");
            }
            else
            {
                callBackConnect(true,SoketError.ConnectSucess,"connect Sucess");
                //接收消息

            }
        }
        catch(Exception ex)
        {
            this.callBackConnect(false,SoketError.Unknow,ex.ToString());
        }
    }

    #endregion

    #region DisConnect

    public void AynsDisConnect(CallBackNormal callBack)
    {
        try
        {
            callBackDisconnect = callBack;
            if(clientSocket == null)
            {
                callBackDisconnect(false,SoketError.DisConnectError,"clientSocket is null");
            }
            else if(!clientSocket.Connected)
            {
                callBackDisconnect(false,SoketError.DisConnectError,"clientSocket is UnConnected");
            }
            else
            {
                IAsyncResult asyncResult = clientSocket.BeginDisconnect(false,AsysDisConnectCallBack,null);

                if(!TimeOutCheck(asyncResult))
                {
                    callBackSend(false,SoketError.DisConnectError,"DisConnect Error");
                }
            }
        }
        catch(Exception ex)
        {
            callBackDisconnect(false,SoketError.Unknow,ex.ToString());
        }
    }

    private void AsysDisConnectCallBack(IAsyncResult aysncResult)
    {
        try
        {
            clientSocket.EndDisconnect(aysncResult);
            clientSocket.Close();
            clientSocket = null;
            callBackDisconnect(true,SoketError.DisConnectSucess,"DisConnectSucess");
        }
        catch(Exception ex)
        {
            callBackDisconnect(false,SoketError.Unknow,ex.ToString());
        }
    }


    #endregion

    #region Recive

    public void Recive()
    {
        if(clientSocket != null && clientSocket.Connected)
        {
            clientSocket.BeginReceive(m_recvBuffer,0,m_recvBuffer.Length,SocketFlags.None,AsysRecvCallBack,clientSocket);
        }
    }

    private void AsysRecvCallBack(IAsyncResult aysncResult)
    {
        try
        {
            if(!clientSocket.Connected)
            {
                this.callBackRecv(false,SoketError.ReciveUnSucess,"已经断开连接",null,"");
            }
            else
            {
                int length = clientSocket.EndReceive(aysncResult);
                if(length != 0)
                {
                    m_socketBuffer.ReciveByte(m_recvBuffer,m_recvBuffer.Length);
                }
            }
        }
        catch(Exception ex)
        {
            this.callBackRecv(false,SoketError.Unknow,ex.ToString(),null,"");
        }
        Recive();
    }

    public void RecvMsgOver(byte[] data)
    {
        callBackRecv(true,SoketError.RecivSucess,"",null,"recv back sucess");
    }

    #endregion


    #region send

    public void AsynSend(byte[] data,CallBackNormal callBackSend)
    {
        this.callBackSend = callBackSend;
        if(clientSocket == null)
        {
            this.callBackSend(false,SoketError.SocketNull,"socket is null");
        }
        else if(!clientSocket.Connected)
        {
            this.callBackSend(false,SoketError.DisConnect,"socket DisConnect");
        }
        else
        {
            IAsyncResult asyncResult = clientSocket.BeginSend(data,0,data.Length,SocketFlags.None,AsysSendCallBack,null);
            if(!TimeOutCheck(asyncResult))
            {
                this.callBackSend(false,SoketError.SendUnSucess,"send faild");
            }
        }
    }

    private void AsysSendCallBack(IAsyncResult aysncResult)
    {
        try
        {
            //  int byteSend = clientSocket.EndSend(aysncResult);
            this.callBackSend(true,SoketError.Sucess,"send sucess");
        }
        catch(Exception ex)
        {
            this.callBackSend(false,SoketError.SendUnSucess,ex.ToString());
        }
    }

    #endregion




    #region TimeOut check

    bool TimeOutCheck(IAsyncResult ar)
    {
        int i = 0;
        while(ar.IsCompleted == false)
        {
            i++;
            if(i > 20)
            {
                m_errorSocket = SocketError.TimedOut;
                return false;
            }
            Thread.Sleep(100);
        }
        return true;
    }

    #endregion

    public bool IsConnect()
    {
        if (clientSocket != null && clientSocket.Connected)
        {
            return true;
        }
        return false;
    }

}

