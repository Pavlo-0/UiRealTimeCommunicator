using UiRtc.Domain.HubMap.Interface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace UiRtc.Domain.HubMap
{
    internal class NameGenerator : INameGenerator
    {
        public string GetHubName(Type type)
        {
            var attribute = Attribute.GetCustomAttribute(type, typeof(UiRtcHubAttribute)) as UiRtcHubAttribute;

            if (attribute == null || string.IsNullOrEmpty(attribute.HubName))
            {
                return type.Name;
            }

            return attribute.HubName;
        }

        public string GetHubNameByContract(Type type)
        {
            var hubType = type.GetInterfaces().First().GenericTypeArguments[0];
            return GetHubName(hubType);
        }

        public string GetMethodName(Type type)
        {
            var attribute = Attribute.GetCustomAttribute(type, typeof(UiRtcMethodAttribute)) as UiRtcMethodAttribute;

            if (attribute == null || string.IsNullOrEmpty(attribute.MethodName))
            {
                return type.Name;
            }

            return attribute.MethodName;
        }
    }
}
