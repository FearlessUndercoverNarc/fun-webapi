namespace Services.SharedServices.Abstractions
{
    public interface IRequestAccountIdService
    {
        public long Id { get; }

        public bool IsSet { get; }
    }

    public interface IRequestAccountIdSetterService
    {
        void Set(long id);
    }
}