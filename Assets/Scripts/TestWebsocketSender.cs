using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using HoloToolkit.Unity.InputModule;
using System;

public class TestWebsocketSender : MonoBehaviour, IInputClickHandler
{
    WebSocket websocket;
    bool isClicked;
    bool updateTextInfo;
    String newTextInfo;
    TextMesh textinfo;


    public void Connect()
    {
        websocket = new WebSocket("ws://138.67.205.185:8000/");
        try
        {
            websocket.ConnectAsync();
            Debug.Log("STARTING WEBSOCKET");
        } catch (AggregateException e)
        {
            Debug.Log("EXCEPTION: " + e.Message);

        }
    }

    public void Disconnect()
    {
        websocket.Close();
    }

    public void SendCommand(string msg)
    {
        if (websocket != null && websocket.IsAlive)
        {
            websocket.Send(msg);
        }
    }
    // Start is called before the first frame update
   
    void Start()
    {
        textinfo = GameObject.Find("infotext").GetComponentInChildren<TextMesh>();
        Connect();
        websocket.OnOpen += (sender, e) =>
        {
            Debug.Log("Websocket Open");
            try
            {
                SendCommand("CONNECTED");
            }
            catch (AggregateException ex)
            {
                Debug.Log("EXCEPTION: " + ex.Message);
            }
        };

        websocket.OnMessage += (sender, e) =>
        {
            Debug.Log("WebSocket Message Type: " + e.GetType() + ", Data: " + e.Data);
            updateTextInfo = true;
            newTextInfo = e.Data;


        };

        websocket.OnError += (sender, e) =>
        {
            Debug.Log("WebSocket Error Message: " + e.Message);
        };

        websocket.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket Close");
            SendCommand("HOLOLENS DISCONNECTED");

        };
    }
    
   // Update is called once per frame
   void Update()
   {
        if (Input.GetKeyUp("s"))
        {
            try
            {
                SendCommand("test message");
            }
            catch (AggregateException e)
            {
                Debug.Log("EXCEPTION: " + e.Message);
            }
        }

        if (updateTextInfo)
        {
            
            try
            {
                if (textinfo != null)
                {
                    textinfo.text = newTextInfo;
                    /*
                    if (textinfo.text.Length > 300)
                    {
                        textinfo.text = newTextInfo + "\n";
                    }
                    else
                    {
                        textinfo.text += newTextInfo + "\n";
                    }
                    */
                }

            } catch (Exception exception)
            {
                Debug.Log("EXCEPTION: " + exception.Message);
            }

            updateTextInfo = false;

        }
    }

    void OnDestroy()
    {
        websocket.Close();
        websocket = null;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (!isClicked)
        {
            this.GetComponent<MeshRenderer>().material.color = Color.green;
            isClicked = true;
            Debug.Log("Clicking at Box - green");
            try
            {
                SendCommand("GREEN - clicked");
            }
            catch (AggregateException e)
            {
                Debug.Log("EXCEPTION: " + e.Message);
            }
            
        } else
        {
            this.GetComponent<MeshRenderer>().material.color = Color.red;
            Debug.Log("Clicking at Box - red");
            try
            {
                SendCommand("RED - clicked");
            }
            catch (AggregateException e)
            {
                Debug.Log("EXCEPTION: " + e.Message);
            }
            isClicked = false;
        }

    }
}
