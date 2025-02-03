namespace UiRtc.Typing.PublicInterface.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class UiRtcHubAttribute(string hubName) : Attribute
    {
        public string HubName { get; private set; } = hubName;
    }
}
