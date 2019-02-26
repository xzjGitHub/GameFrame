using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class NetMessageBase
{

    private byte[] buffer;
    public byte[] BufferData
    {
        get { return buffer; }
    }

    private ushort m_msgId;

    public NetMessageBase(byte[] arr)
    {
        buffer = arr;
        m_msgId = BitConverter.ToUInt16(arr, 4);
    }
}

