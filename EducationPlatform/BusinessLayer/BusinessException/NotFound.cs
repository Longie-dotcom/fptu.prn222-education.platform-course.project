namespace BusinessLayer.BusinessException
{
    public class NotFound : Exception
    {
        public NotFound(string message) : base(message) { }
    }
}
