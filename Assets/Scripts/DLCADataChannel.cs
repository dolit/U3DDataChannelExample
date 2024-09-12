using System;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.UI;

public class DLCADataChannel : MonoBehaviour
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
    async void Start()
    {
        int port = GetPort();
        if (port > 0)
        {
            // 创建实例
            string address = "ws://127.0.0.1:" + port + "/inner/data_channel";
            websocket = new WebSocket(address);

            // 注册回调
            websocket.OnOpen += () =>
            {
                onConenctedFlag = true;
                Debug.Log("DolitDataChannel Connection open!");
            };

            websocket.OnError += (e) =>
            {
                Debug.Log("DolitDataChannel Error! " + e);
                onConenctedFlag = false;
            };

            websocket.OnClose += (e) =>
            {
                Debug.Log("DolitDataChannel Connection closed!");
                onConenctedFlag = false;
            };

            websocket.OnMessage += (bytes) =>
            {
                // bytes.
                string message = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.Log(string.Format("DLCADataChannel Receive Text: {0}", message));
                GameObject go = GameObject.Find("Canvas/Text");
                if (go != null)
                {
                    Text ta = go.GetComponent<Text>();
                    if (ta != null)
                        ta.text = "接收到来自客户端文本:" + message;
                }
            };

            Debug.Log("DLCADataChannel beging connecting...");
            // 连接
            await websocket.Connect();
            
        }
        else
        {
            Debug.Log("DLCADataChannel parse port failed,port <= 0. please check enable data channel features from cms app settings.");
        }
    }

    /**
     * 发送文本数据给客户端。
     */
    public void AsyncSendText(string text)
    {
        websocket.SendText(text);
        Debug.Log("DLCADataChannel send text: " + text);
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

#if !UNITY_WEBGL || UNITY_EDITOR
        if(websocket != null)
            websocket.DispatchMessageQueue();
#endif
    }

}
