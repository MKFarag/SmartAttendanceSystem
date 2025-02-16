#region Packges | Microsoft

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Asp.Versioning;
global using Hangfire;
global using Mapster;

#endregion

//////////////////////////////////////////////////////////////////////////////////

#region MyMethods

global using SmartAttendanceSystem.Infrastructure.Authentication.Filters;
global using SmartAttendanceSystem.Core.Abstraction.Constants;
global using SmartAttendanceSystem.Presentation.Abstraction;
global using SmartAttendanceSystem.Presentation.Extensions;
global using SmartAttendanceSystem.Fingerprint.Interfaces;
global using SmartAttendanceSystem.Application.Interfaces;
global using SmartAttendanceSystem.Application.Helpers;
global using SmartAttendanceSystem.Core.Abstraction;
global using SmartAttendanceSystem.Core.Settings;
global using SmartAttendanceSystem.Core.Entities;
global using SmartAttendanceSystem.Core.Errors;

#endregion

//////////////////////////////////////////////////////////////////////////////////

#region DTOs

global using SmartAttendanceSystem.Application.Contracts.Authentication.Requests;
global using SmartAttendanceSystem.Application.Contracts.Department.Requests;
global using SmartAttendanceSystem.Application.Contracts.Student.Requests;
global using SmartAttendanceSystem.Application.Contracts.Course.Requests;
global using SmartAttendanceSystem.Application.Contracts.Users.Requests;
global using SmartAttendanceSystem.Application.Contracts.Roles.Requests;
global using SmartAttendanceSystem.Application.Contracts.Common;

#endregion