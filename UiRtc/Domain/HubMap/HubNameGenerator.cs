using UiRtc.Domain.HubMap.Interface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace UiRtc.Domain.HubMap
{
    internal class HubNameGenerator : IHubNameGenerator
    {
        public string GetHubName(Type type)
        {
            // Retrieve the UiRtcHubAttribute from the type
            var attribute = Attribute.GetCustomAttribute(type, typeof(UiRtcHubAttribute)) as UiRtcHubAttribute;

            if (attribute == null || string.IsNullOrEmpty(attribute.HubName))
            {
                return type.Name;
            }

            return attribute.HubName;
        }
    }
}
