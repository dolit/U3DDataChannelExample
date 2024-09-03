using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;

// ���ļ�Ϊ��ȡ���ʶ���ҳ����˷����ݵ�Demoʾ����
// ������һ��ȫ��Ψһ�Ķ���ʾ�������ϴ˽ű����ɡ�

public class DLCAMicorphoneChannel : MonoBehaviour
{
    private WebSocket websocket;

    // Start is called before the first frame update
    async void Start()
    {
        Application.targetFrameRate = 60;

        int port = DolitDataChannel.GetPort();
        port = 3003;
        if (port <= 0)
        {
            Debug.Log("dlca port is zero,please cheack enabed data chennel from cms app settings.");
            return;
        }
        // ����ʵ��
        string address = "ws://127.0.0.1:" + port  + "/pull_mic";
        websocket = new WebSocket(address);
        websocket.OnOpen += () =>
        {
            Debug.Log("DLCAMicorphoneChannel Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("DLCAMicorphoneChannel Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("DLCAMicorphoneChannel Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            // bytes.
            if (bytes.Length > 200)
            {
                // �˴�Ϊ���յ���pcm���ݡ�
                Debug.Log(string.Format("DLCAMicorphoneChannel Receive Pcm Bytes Len: {0}", bytes.Length));

            } else {
                string message = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.Log(string.Format("DLCAMicorphoneChannel Receive Pcm Format: {0}", message));
                GameObject go = GameObject.Find("Canvas/MicText");
                if (go != null)
                {
                    Text ta = go.GetComponent<Text>();
                    if (ta != null)
                        ta.text = "���յ����Կͻ����������� ��ʽ:" + message;
                }
            }

        };
        await websocket.Connect();
        Debug.Log("DLCAMicorphoneChannel connnecting.");
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }
}
