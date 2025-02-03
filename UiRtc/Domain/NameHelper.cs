using UiRtc.Typing.PublicInterface.Attributes;

namespace UiRtc.Domain
{
    internal static class NameHelper
    {
        public static string GetHubName(Type type)
        {
            var attribute = Attribute.GetCustomAttribute(type, typeof(UiRtcHubAttribute)) as UiRtcHubAttribute;

            if (attribute == null || string.IsNullOrEmpty(attribute.HubName))
            {
                return type.Name;
            }

            return attribute.HubName;
        }

        public static string GetHubNameByContract(Type type)
        {
            var hubType = type.GetInterfaces().First().GenericTypeArguments[0];
            return GetHubName(hubType);
        }

        public static string GetMethodName(Type type)
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
