using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

public class SocketBuffer
{
    private byte[] m_headByte;

    private byte[] m_allRecvData;

    private byte m_headLength = 6;

    //当前接收到的数据长度
    private int m_curRecvLength;

    //总共接收的数据长度
    private int m_allDataLength;

    public delegate void CallBackRecvOver(byte[] allData);

    CallBackRecvOver m_callBackRecvOver;

    public SocketBuffer(byte headLength,CallBackRecvOver callBackRecvOver)
    {
        this.m_headLength = headLength;
        m_headByte = new byte[headLength];

        m_callBackRecvOver = callBackRecvOver;
    }

    public void ReciveByte(byte[] recvByte,int realLength)
    {
        if(realLength == 0) return;

        //当前接收的数据 小于头的长度
        if(m_curRecvLength < m_headByte.Length)
        {
            RecvHead(recvByte,realLength);
        }
        else
        {
            //接收的总长度
            int tempLeghth = m_curRecvLength + realLength;
            if(tempLeghth == m_allDataLength)
            {
                //处理刚好相等的情况
                RecvOneAll(recvByte,realLength);
            }
            else if(tempLeghth > m_allDataLength)
            {
                //接收到的有多的
                RecvLarger(recvByte,realLength);
            }
            else
            {
                RecvSmall(recvByte,realLength);
            }
        }
    }

    private void RecvOneAll(byte[] recvByte,int realLength)
    {
        Buffer.BlockCopy(recvByte,0,m_allRecvData,m_curRecvLength,realLength);

        m_curRecvLength += realLength;

        ReciveOneMsgOver();
    }

    private void RecvLarger(byte[] recvByte,int realLength)
    {

        int tempLength = m_allDataLength - m_curRecvLength;

        Buffer.BlockCopy(recvByte,0,m_allRecvData,m_curRecvLength,tempLength);

        m_curRecvLength += tempLength;

        ReciveOneMsgOver();

        int remainLength = realLength - tempLength;

        byte[] remainByte = new byte[remainLength];

        Buffer.BlockCopy(recvByte,tempLength,remainByte,0,remainLength);

        ReciveByte(remainByte,remainLength);
    }

    private void RecvSmall(byte[] recvByte,int realLength)
    {
        Buffer.BlockCopy(recvByte,0,m_allRecvData,m_curRecvLength,realLength);
        m_curRecvLength += realLength;


    }


    private void RecvHead(byte[] revByte,int realLength)
    {
        //差多少个字节 才能拼成头部
        int tempReal = m_headByte.Length - m_curRecvLength;

        //现在接收的和已经接收的总长度
        int tempLength = m_curRecvLength + realLength;
        //总长度小于头
        if(tempLength < m_headByte.Length)
        {
            Buffer.BlockCopy(revByte,0,m_headByte,m_curRecvLength,realLength);
            m_curRecvLength += realLength;
        }
        //大于等于头
        else
        {
            //拷贝差的数据到headbuff  此时 头部已经齐了 只相差 tempReal个数据
            Buffer.BlockCopy(revByte,0,m_headByte,m_curRecvLength,tempReal);
            m_curRecvLength += tempReal;

            //取出4个字节 转换int
            m_allDataLength = BitConverter.ToInt32(m_headByte,0) + m_headLength;
            m_allRecvData = new byte[m_allDataLength];

            //m_allRecvData已经包涵了头部了
            Buffer.BlockCopy(m_headByte,0,m_allRecvData,0,m_headLength);

            int tempRemain = realLength - tempReal;

            //是否还有数据
            if(tempRemain > 0)
            {
                byte[] tempByte = new byte[tempRemain];

                //表示将剩下的字节送入tempByte
                Buffer.BlockCopy(revByte,tempReal,tempByte,0,tempRemain);

                //继续处理剩下的字节
                ReciveByte(tempByte,tempRemain);
            }
            else
            {
                //一个消息已经接收完了
                ReciveOneMsgOver();
            }
        }
    }

    private void ReciveOneMsgOver()
    {

        if(m_callBackRecvOver != null)
        {
            m_callBackRecvOver(m_allRecvData);
        }

        m_curRecvLength = 0;
        m_allDataLength = 0;
        m_allRecvData = null;
    }
}

