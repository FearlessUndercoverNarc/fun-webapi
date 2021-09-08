using Models.Misc;

namespace Services
{
    public static class ObjectExtensions
    {
        public static void EnsureNotNullFatal(this object obj, string message = "")
        {
            if (obj == null)
            {
                throw new(message);
            }
        }

        public static void EnsureNotNullHandled(this object obj, string message = "")
        {
            if (obj == null)
            {
                throw new FunException(message);
            }
        }
    }
}