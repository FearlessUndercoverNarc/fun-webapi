using System;
using System.Collections.Generic;
using Services.SharedServices.Abstractions;

namespace Services.SharedServices.Implementations
{
    public class SSEService : ISSEService
    {
        public event Action<long, long> DeskActionOccured;
        
        public Dictionary<long, long> LastDeskActionIdMap { get; set; }

        public void EmitDeskActionOccured(long deskId, long eventId)
        {
            DeskActionOccured?.Invoke(deskId, eventId);
        }
    }
}