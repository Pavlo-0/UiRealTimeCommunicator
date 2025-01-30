namespace UiRtc.Typing.PublicInterface.Attributes
{
    public class UiRtcHubAttribute(string hubName) : Attribute
    {
        public string HubName { get; private set; } = hubName;
    }
}
