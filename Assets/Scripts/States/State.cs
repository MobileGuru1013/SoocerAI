using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public abstract class State// : MonoBehaviour
{

    //static CHANGENAME instance;

    //public static CHANGENAME Instance()
    //{
    //    if (instance == null)
    //    {
    //        instance = new CHANGENAME();
    //    }
    //    return instance;
    //}

    /**
    *   this will execute when the state is entered
    */
    public abstract void Enter(GameObject CallingObject);

    /**
    *   this is the updated fucntion for the state
    */
    public abstract void Excute(GameObject CallingObject);

    /**
    *   this will execute when the state is exited
    */
    public abstract void Exit(GameObject CallingObject);

    public virtual bool OnMessage(GameObject CallingObject, Message Msg)
    {

        return false;
    }

}
