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
}
