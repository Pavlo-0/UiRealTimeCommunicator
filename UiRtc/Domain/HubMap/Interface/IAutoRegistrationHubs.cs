using Microsoft.AspNetCore.Builder;
using System.Reflection;

namespace UiRtc.Domain.HubMap.Interface
{
    internal interface IAutoRegistrationHubs
    {
        void RegisterHub(Assembly assembly, WebApplication app);
    }
}