using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utils
{
    public class RisingEdge
    {
        private bool last_state;
        private bool this_state;

        public RisingEdge(bool init_state)
        {
            last_state = init_state;
            this_state = init_state;
        }

        public void update(bool current_state)
        {
            // Must always be called in an Update or FixedUpdate function
            last_state = this_state;
            this_state = current_state;
        }

        public bool q()
        {
            return this_state && !last_state;
        }
    }

    public class TON
    {
        public float ttime;
        float when_triggered;
        public bool state;
        RisingEdge redge;
        bool to_return;

        // Very similar to twincat TON
        public TON(bool init_state, float trigger_time)
        {
            state = init_state;
            ttime = trigger_time;
            when_triggered = Mathf.Infinity;
            redge = new RisingEdge(init_state);
            to_return = false;
        }

        public void update(bool signal)
        {
            redge.update(signal);
            if (redge.q())
            {
                when_triggered = Time.time;
            }
            to_return = ((Time.time - when_triggered) >= ttime) && signal;
        }

        public bool q()
        {
            return to_return;
        }
    }

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool m_shuttingDown = false;
        private static object m_lock;
        private static T m_instance;

        public static T Instance
        {
            get
            {
                if (m_shuttingDown)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) + 
                                     "'already destroyed. Returning null");
                    return null;
                }

                lock (m_lock)
                {
                    if (m_instance == null)
                    {
                        m_instance = (T)FindObjectOfType(typeof(T));
                    }
                    return m_instance;
                }
            }
        }

        private void OnApplicationQuit()
        {
            m_shuttingDown = true;
        }

        private void OnDestroy()
        {
            m_shuttingDown = true;
        }
    }
}
