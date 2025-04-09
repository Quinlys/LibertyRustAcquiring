namespace LibertyRustAcquiring.Exceptions
{
    public class ObjectIsNullException : Exception
    {
        private readonly string typeName;
        public ObjectIsNullException(string type) : base()
        {
            typeName = type;
        }

        public override string Message => $"Object of {typeName} type is not found.";
    }
}
