using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityWebSocket;
using UnityEngine.UI;

// ���ļ�Ϊ��ȡ���ʶ���ҳ����˷����ݵ�Demoʾ����
// ������һ��ȫ��Ψһ�Ķ���ʾ�������ϴ˽ű����ɡ�

public class DLCAMicorphoneChannel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int port = DolitDataChannel.GetPort();
        if(port <= 0)
        {
            Debug.Log("dlca port is zero,please cheack enabed data chennel from cms app settings.");
            return;
        }
        // ����ʵ��
        string address = "ws://127.0.0.1:" + port  + "/pull_mic";
        websocket = new WebSocket(address);

        // ע��ص�
        websocket.OnOpen += OnOpen;
        websocket.OnClose += OnClose;
        websocket.OnMessage += OnMessage;
        websocket.OnError += OnError;

        // ����
        websocket.ConnectAsync();
    }

    void OnOpen(object sender, OpenEventArgs openArgs)
    {

    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        if (e.IsBinary)
        {
            Debug.Log(string.Format("Receive Pcm Bytes Len: {0}", e.RawData.Length));
        }
        else if (e.IsText)
        {
            Debug.Log(string.Format("Receive Pcm Format: {0}", e.Data));

            GameObject go = GameObject.Find("Canvas/MicText");
            if (go != null)
            {
                Text ta = go.GetComponent<Text>();
                if (ta != null)
                    ta.text = "���յ����Կͻ����������� ��ʽ:" + e.Data;
            }
        }
    }

    private void OnClose(object sender, CloseEventArgs e)
    {
       // AddLog(string.Format("Closed: StatusCode: {0}, Reason: {1}", e.StatusCode, e.Reason));
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
       // AddLog(string.Format("Error: {0}", e.Message));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private WebSocket websocket;
}
