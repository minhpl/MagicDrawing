using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[Serializable]
public class Test
{
    public LinkedList<string> linkedlist;
    public List<string> list;
    public int number = 10;
    //public Queue<string> queue;
    public Test()
    {
        //AotHelper.EnsureList<string>();
        //AotHelper.EnsureList<string>();
        AotHelper.EnsureList<string>();
    }
}

public class MyClass
{
    public int level;
    public float timeElapsed;
    public string playerName;
}

public class TestSaladSeriazable : MonoBehaviour {


    void Start()
    {

        MyClass myObject = new MyClass();
        myObject.level = 1;
        myObject.timeElapsed = 47.5f;
        myObject.playerName = "Dr Charles Francis";
        try
        {
            Test test = new Test();
            test.linkedlist = new LinkedList<string>();
            test.linkedlist.AddLast("llist a");
            test.linkedlist.AddLast("llist b");
            test.linkedlist.AddLast("llist c");
            test.list = new List<string>();
            test.list.Add("list a");
            test.list.Add("list b");
            test.list.Add("list c");
            //test.queue = new Queue<string>();
            //test.queue.Enqueue("queue a");
            //test.queue.Enqueue("queue b");
            //test.queue.Enqueue("queue c");
            var json = JsonConvert.SerializeObject(test);
            var jsonUnity = JsonUtility.ToJson(test);
            Utilities.Log("json is {0}", json);
            Utilities.Log("jsonunity is {0}", json);
            Test deserializeObj = new Test();
            //deserializeObj = JsonConvert.DeserializeObject<Test>(json);
            JsonConvert.PopulateObject(json, deserializeObj);
            Test deserializeObjUnity = JsonUtility.FromJson<Test>(jsonUnity);
            Utilities.Log("{0}", deserializeObj.number);
            Utilities.Log("{0}", deserializeObjUnity.number);
            if (deserializeObj.linkedlist != null)
            {
                var first = deserializeObj.linkedlist.First;
                Utilities.Log("deserializeObj linked list:{0} {1} {2} {3}", deserializeObj.number, first.Value, first.Next.Value, first.Next.Next.Value);
            }
            else
            {
                Utilities.Log("deserializeObj linked list is null");
            }
            if (deserializeObjUnity.linkedlist != null)
            {
                var firstUnity = deserializeObjUnity.linkedlist.First;

                Utilities.Log("deserializeObjUnity linked list: {0} {1} {2} {3}", deserializeObjUnity.number, firstUnity.Value, firstUnity.Next.Value, firstUnity.Next.Next.Value);
            }
            else
            {
                Utilities.Log("deserializeObjUnity linked list is null");
            }

            if (deserializeObj.list != null)
            {
                var list = deserializeObj.list;
                Utilities.Log("deserializeObj list:{0} {1} {2} ", list[0], list[1], list[2]);
            }
            else
            {
                Debug.Log("deserializeObj list is null");
            }

            if (deserializeObjUnity.list != null)
            {
                var list = deserializeObjUnity.list;
                Utilities.Log("deserializeObjUnity list:{0} {1} {2} ", list[0], list[1], list[2]);
            }
            else
            {
                Utilities.Log("deserializeObjUnity list is null");
            }

            //if (deserializeObj.queue != null)
            //{
            //    var list = deserializeObj.queue;
            //    Utilities.Log("deserializeObj queue:{0} {1} {2} ", list.Dequeue(), list.Dequeue(), list.Dequeue());
            //}
            //else
            //{
            //    Debug.Log("deserializeObj queue is null");
            //}

            //if (deserializeObjUnity.queue != null)
            //{
            //    var list = deserializeObjUnity.queue;
            //    Utilities.Log("deserializeObjUnity queue:{0} {1} {2} ", list.Dequeue(), list.Dequeue(), list.Dequeue());
            //}
            //else
            //{
            //    Utilities.Log("deserializeObjUnity queue is null");
            //}
        }
        catch (Exception e)
        {
            Utilities.Log("Error :{0}", e.ToString());
            Utilities.Log("Stack trace :{0}", e.StackTrace.ToString());
        }
    }
}
