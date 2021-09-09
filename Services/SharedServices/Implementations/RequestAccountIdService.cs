using Services.SharedServices.Abstractions;

namespace Services.SharedServices.Implementations
{
    public class RequestAccountIdService : IRequestAccountIdService, IRequestAccountIdSetterService
    {
        public long Id { get; private set; }
        public bool IsSet { get; private set; }
        public bool HasSubscription { get; set; }

        public void Set(long id, bool hasSubscription)
        {
            Id = id;
            IsSet = true;
            HasSubscription = hasSubscription;
        }
    }
}