using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateLogger.Services.StartEvents
{
    public class StartEventsCommand
    {
        public string command { get; set; } = "startEvents";
        //public string command { get; set; } = "getConfig";
        public int? result { get; set; } = 1;
    }
}
