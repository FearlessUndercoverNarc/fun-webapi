namespace Services.SharedServices.Abstractions
{
    public interface IRequestAccountIdService
    {
        public long Id { get; }

        public bool HasSubscription { get; set; }

        public bool IsSet { get; }
    }

    public interface IRequestAccountIdSetterService
    {
        void Set(long id, bool hasSubscription);
    }
}