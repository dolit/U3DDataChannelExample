using System;
using UnityEngine;
using UnityWebSocket;
using UnityEngine.UI;

public class DolitDataChannel : MonoBehaviour
{
    bool onConenctedFlag;
    private WebSocket websocket;


    public static int GetPort()
    {
        string[] commandLines = System.Environment.GetCommandLineArgs();
        foreach (string cmdLine in commandLines)
        {
            string[] splits = null;
            if (cmdLine.StartsWith("-DLCAPort="))
            {
                splits = cmdLine.Split("=");
                if (splits.Length != 2)
                    return 0;
                if (Int32.TryParse(splits[1], out int port)) {
                    return port;
                }
                return 0;
            }
        }
        return 0;
    }

    /**
     * 初始化数据通信通道。
     */
    public bool InitChannel()
    {
        int port = GetPort();
        if (port > 0)
        {
            // 创建实例
            string address = "ws://127.0.0.1:" + port + "/inner/data_channel";
            websocket = new WebSocket(address);

            // 注册回调
            websocket.OnOpen += OnOpen;
            websocket.OnClose += OnClose;
            websocket.OnMessage += OnMessage;
            websocket.OnError += OnError;

            // 连接
            websocket.ConnectAsync();
            Debug.Log("DLCADataChannel beging connecting...");
        }
        else
        {
            Debug.Log("DLCADataChannel parse port failed,port <= 0. please check enable data channel features from cms app settings.");
        }
        return false;
    }

    /**
     * 发送文本数据给客户端。
     */
    public void AsyncSendText(string text)
    {
        websocket.SendAsync(text);
        Debug.Log("DLCADataChannel send text: " + text);
    }


    void OnOpen(object sender, OpenEventArgs openArgs)
    {
        onConenctedFlag = true;
        Debug.Log("DLCADataChannel connected");
    }


    private void OnMessage(object sender, MessageEventArgs e)
    {
        // 麦克风数据先触发一帧Text Json数据来描述音频格式，然后紧接着会触发一帧 Binary为实际语音的pcm
        if (e.IsBinary)
        {
            // 此处为接收到的pcm数据。
            Debug.Log(string.Format("DLCADataChannel Receive Bytes Len: {0}", e.RawData.Length));
        }
        else if (e.IsText)
        {
            //Debug.Log(string.Format("DLCADataChannel Receive Text: {0}", e.Data));
            //
            //GameObject go = GameObject.Find("Canvas/Text");
            //if (go != null)
            //{
            //    Text ta = go.GetComponent<Text>();
            //    if (ta != null)
            //        ta.text = "接收到来自客户端文本:" + e.Data;
            //}
        }
    }

    private void OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log(string.Format("DLCADataChannel Closed: StatusCode: {0}, Reason: {1}", e.StatusCode, e.Reason));
        onConenctedFlag = false;
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        Debug.Log(string.Format("DLCADataChannel Error: {0}", e.Message));
        onConenctedFlag = false;
    }

    private float time = 0;

    int index = 0;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= 1)
        {
            // 发送字符串给客户端
            time = 0;
            string str = "Test Data " + index++;
            if (onConenctedFlag)
            {
                Debug.Log("send data to client:" + str);
                AsyncSendText(str);
            }
        }
    }

}
