using System;
using System.Collections.Generic;

namespace Services.SharedServices.Abstractions
{
    public interface ISSEService
    {
        public event Action<long, long> DeskActionOccured;
        
        public Dictionary<long, long> LastDeskActionIdMap { get; set; }
        
        public void EmitDeskActionOccured(long deskId, long eventId);
    }
}