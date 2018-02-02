using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public enum PlayerMessages
{
    ReceiveBall,
    PassToMe,
    SupportAttacker,
    GoHome,
    Wait
};



public class Message 
{
    public void BuildMessage(float TimeDelay, GameObject SenderObj, GameObject RecieverObj, PlayerMessages MessageToSend, Vector3 ExtraVectorInfo )
    {
        Sender = SenderObj;           
        Reciever = RecieverObj;
        Msg = MessageToSend;
        DisptachTime = TimeDelay;
        ExtraInfo = ExtraVectorInfo;
}

    public GameObject Sender;
    public GameObject Reciever;
    public PlayerMessages Msg;
    public float DisptachTime = 0;
    public Vector3 ExtraInfo;

  
}



public class Dispatcher 
{
    private List<Message> DelayedMessages = new List<Message>();

    int SortByDelay(Message p1, Message p2)
    {
        return p1.DisptachTime.CompareTo(p2.DisptachTime);
    }

    static Dispatcher instance;

    public static Dispatcher Instance()
    {
        if (instance == null)
        {
            instance = new Dispatcher();
        }
        return instance;
    }

    void Discharge(GameObject Reciever, Message Telegram)
    {
        Player PlayerScript = Reciever.GetComponent<Player>();


        if (!PlayerScript.HandleMessage(Telegram))
        {
            Debug.Log("Message not Handled!");
        }
    }

    public void DispatchMessage(float Delay, GameObject Sender, GameObject Reciever, PlayerMessages MessageToSend, Vector3 ExtraInfo = new Vector3())
    {
        if (Reciever == null)
        {
            Debug.Log("No Reciever Set For Sending Message");
            return;
        }

        Message Msg = new Message();
        Msg.BuildMessage(Time.time + Delay, Sender, Reciever, MessageToSend, ExtraInfo);

        //If no delay send message straight away
        if (Delay <= 0)
        {
            Discharge(Reciever, Msg);
        }
        else
        {
            DelayedMessages.Add(Msg);
            DelayedMessages.Sort(SortByDelay);
        }
    }

    public void DispatchDelayedMessages()
    {
        while (DelayedMessages.Count > 0 && (DelayedMessages[0].DisptachTime < Time.time))
        {
            Message Msg = DelayedMessages[0];

            Discharge(Msg.Reciever, Msg);

            DelayedMessages.RemoveAt(0);
        }
    }

}
