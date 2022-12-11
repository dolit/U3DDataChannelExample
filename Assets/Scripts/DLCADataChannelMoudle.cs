using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DolitDataChannel : MonoBehaviour
{
    public enum DLCACode
    {
        DLCACodeSuccessfully,
        DLCACodeFailed
    }

    public enum DLCADisconnectedCode
    {
        DLCA_DISCONNECTED_CODE_SERVER_SHUTDOWN,
        DLCA_DISCONNECTED_CODE_NET_ERROR,
    };

    public enum DLCADataType
    {
        DLCA_DATA_TYPE_TEXT,
        DLCA_DATA_TYPE_BINRAY,
    };

    public enum DLCAConnectRes
    {
        DLCA_CONNECT_RES_SUCCESSFULLY,
        DLCA_CONNECT_RES_TIMEOUT,
        DLCA_CONNECT_RES_FAILED,
    };

    public delegate void OnOnConenctedDelegate(DLCAConnectRes res);
    public delegate void OnTextDataDelegate(string text);

    public static DolitDataChannel.OnOnConenctedDelegate onConnectedCallback;
    public static DolitDataChannel.OnTextDataDelegate onTextDataCallback;

    private delegate void OnConenctedCFunc(DLCAConnectRes res, IntPtr userData);
    private delegate void OnDataCFunc(DLCADataType dataType, IntPtr data, int size, IntPtr userData);
    private delegate void OnDisConnectedCFunc(DLCADisconnectedCode disConnectedCode, IntPtr userData);

    private static OnConenctedCFunc onConenctedCFunc;
    private static OnDataCFunc onDataCFunc;
    private static OnDisConnectedCFunc onDisConnectedCFunc;

    [DllImport("dlca-data-channelmt")]
    private static extern DLCACode dlca_data_channel_connect(int port,int ssl_enabled, OnConenctedCFunc onConenctedFunc, OnDataCFunc onDataFunc, OnDisConnectedCFunc onDisConenctedFunc,IntPtr userData);

    [DllImport("dlca-data-channelmt")]
    private static extern DLCACode dlca_data_channel_send(DLCADataType dataType,IntPtr data,int size);

    /**
     * 初始化数据通信通道。
     */
    public static bool InitChannel()
    {
        string[] commandLines = System.Environment.GetCommandLineArgs();
        foreach (string cmdLine in commandLines)
        {
            string[] splits = null;
            if (cmdLine.StartsWith("-DLCAPort="))
            {
                splits = cmdLine.Split("=");
                if (splits.Length != 2)
                    return false;
                if (Int32.TryParse(splits[1], out int port))
                {
                    if (port > 0)
                    {
                        onConenctedCFunc = (DLCAConnectRes res, IntPtr userData) => {
                            if(onConnectedCallback != null)
                                onConnectedCallback.Invoke(res);
                        };

                        onDataCFunc = (DLCADataType dataType, IntPtr data, int size, IntPtr userData) => {
                            byte[] b = new byte[size];
                            Marshal.Copy(data, b, 0, size);
                            if (dataType == DLCADataType.DLCA_DATA_TYPE_TEXT)
                            {
                                string text = System.Text.Encoding.UTF8.GetString(b);
                                if(onTextDataCallback != null)
                                    onTextDataCallback.Invoke(text);
                            }
                        };

                        onDisConnectedCFunc = (DLCADisconnectedCode disConnectedCode, IntPtr userData) => {

                        };

                        if (dlca_data_channel_connect(port, 0, onConenctedCFunc, onDataCFunc, onDisConnectedCFunc, IntPtr.Zero) == DLCACode.DLCACodeSuccessfully)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        Debug.Log("parse dlca port failed,port < 0.");
                    }
                }
                else
                {
                    Debug.Log("parse dlca port failed");
                    return false;
                }
            }
        }
        return false;
    }

    /**
     * 发送文本数据给客户端。
     */
    public static bool SendText(string text)
    {
        byte[] u8Bytes = System.Text.Encoding.UTF8.GetBytes(text);
        IntPtr unmanagedPointer = Marshal.AllocHGlobal(u8Bytes.Length);
        Marshal.Copy(u8Bytes, 0, unmanagedPointer, u8Bytes.Length);
        DLCACode code = dlca_data_channel_send(DLCADataType.DLCA_DATA_TYPE_TEXT,unmanagedPointer,u8Bytes.Length);
        // Call unmanaged code
        Marshal.FreeHGlobal(unmanagedPointer);
        return code == DLCACode.DLCACodeSuccessfully;
    }
}
