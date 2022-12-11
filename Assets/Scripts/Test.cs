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
        // ������������Ⱦģ��Ļص���ע��˻ص�������unity���߳��У����������post���߳��С�
        Debug.Log("on conencted....");
        onConenctedFlag = true;
    }

    static public void OnDataFunc(string text)
    {
        // �ӿͻ��˽������ݵĻص���ע��˻ص�������unity���߳��У����������post���߳��С�
        Debug.Log("on recv data from client:" + text);

    }

    // Start is called before the first frame update
    void Start()
    {
        // ע��
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
            // �����ַ������ͻ���
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
