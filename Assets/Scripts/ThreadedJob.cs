using System;
using System.Collections;
using OpenCVForUnity;

public class ThreadedJob
{
    private bool m_IsDone = true;
    private object m_Handle = new object();
    private System.Threading.Thread m_Thread = null;
    public bool IsDone
    {
        get
        {
            bool tmp;
            lock (m_Handle)
            {
                tmp = m_IsDone;
            }
            return tmp;
        }
        set
        {
            lock (m_Handle)
            {
                m_IsDone = value;
            }
        }
    }

    public delegate void Handler(Mat data);
    public virtual void Start(Handler handle)
    {
        this.handle = handle;
        m_Thread = new System.Threading.Thread(Run);
        m_Thread.Start();
    }
    public virtual void Abort()
    {
        m_Thread.Abort();
    }
    Handler handle;
    protected virtual void ThreadFunction(Handler handle)
    {
    }

    protected virtual void OnFinished()
    {
        m_Thread = null;
    }

    public virtual bool Update()
    {
        if (IsDone)
        {
            OnFinished();
            return true;
        }
        return false;
    }
    public IEnumerator WaitFor()
    {
        while (!Update())
        {
            yield return null;
        }
    }
    private void Run()
    {
        ThreadFunction(handle);
        IsDone = true;
    }
}