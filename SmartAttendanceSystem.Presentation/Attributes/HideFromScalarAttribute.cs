using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace SmartAttendanceSystem.Presentation.Attributes;

//This class will be in Presentation Layer

//Importance of this class: Hide the endpoints from the OpenAPI documentation and Scalar documentation.

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class HideFromScalarAttribute : Attribute, IApiDescriptionVisibilityProvider
{
    public bool IgnoreApi => true;
}