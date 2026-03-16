namespace BusinessLayer.BusinessException
{
    public class Conflict : Exception
    {
        public Conflict(string message) : base(message) { }
    }
}
