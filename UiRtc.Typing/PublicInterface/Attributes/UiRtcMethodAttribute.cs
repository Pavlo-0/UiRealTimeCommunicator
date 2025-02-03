namespace UiRtc.Typing.PublicInterface.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class UiRtcMethodAttribute(string methodName) : Attribute
    {
        public string MethodName { get; private set; } = methodName;
    }
}
