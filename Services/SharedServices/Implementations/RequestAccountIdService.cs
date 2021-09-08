using Services.SharedServices.Abstractions;

namespace Services.SharedServices.Implementations
{
    public class RequestAccountIdService : IRequestAccountIdService, IRequestAccountIdSetterService
    {
        public long Id { get; private set; }
        public bool IsSet { get; private set; }

        public void Set(long id)
        {
            Id = id;
            IsSet = true;
        }
    }
}