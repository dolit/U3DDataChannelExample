using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Test : MonoBehaviour
{
    static bool onConenctedFlag = false;

    static public void OnConenctedFunc(DolitDataChannel.DLCAConnectRes res)
    {
        // 已连接上云渲染模块的回调，注意此回调不是在unity主线程中，如需操作需post主线程中。
        Debug.Log("on conencted....");
        onConenctedFlag = true;
    }

    static public void OnDataFunc(string text)
    {
        // 从客户端接受数据的回调，注意此回调不是在unity主线程中，如需操作需post主线程中。
        Debug.Log("on recv data from client:" + text);

    }

    // Start is called before the first frame update
    void Start()
    {
        // 注册
        DolitDataChannel.onConnectedCallback = OnConenctedFunc;
        DolitDataChannel.onTextDataCallback = OnDataFunc;
        DolitDataChannel.InitChannel();
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
            if(onConenctedFlag)
            {
                Debug.Log("send data to client:" + str);
                if (!DolitDataChannel.SendText(str))
                {
                    Debug.Log("send data to client failed.");
                }
            }
        }
    }
}
