using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace SmartAttendanceSystem.Presentation.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class HideFromScalarAttribute : Attribute, IApiDescriptionVisibilityProvider
{
    public bool IgnoreApi => true;
}