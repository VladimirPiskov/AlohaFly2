using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlohaFly.Utils
{
    public  class TaskWithEvent
    {
        public TaskWithEvent(Action action, Action<string> _event)
        {
            Action = action;
            Event = _event;
        }
        public Action Action;
        public Action<string> Event;


    }
}
