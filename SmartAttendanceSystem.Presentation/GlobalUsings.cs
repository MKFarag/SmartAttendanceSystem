#region Packges | Microsoft

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.Extensions.Logging;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Hangfire;
global using Mapster;

#endregion

//////////////////////////////////////////////////////////////////////////////////

#region MyMethods

global using SmartAttendanceSystem.Infrastructure.Authentication.Filters;
global using SmartAttendanceSystem.Application.Contracts.Authentication;
global using SmartAttendanceSystem.Application.Contracts.Fingerprint;
global using SmartAttendanceSystem.Application.Contracts.Department;
global using SmartAttendanceSystem.Application.Contracts.Student;
global using SmartAttendanceSystem.Application.Contracts.Course;
global using SmartAttendanceSystem.Application.Contracts.Users;
global using SmartAttendanceSystem.Presentation.Abstraction;
global using SmartAttendanceSystem.Presentation.Extensions;
global using SmartAttendanceSystem.Application.Interfaces;
global using SmartAttendanceSystem.Application.Helpers;
global using SmartAttendanceSystem.Core.Abstraction;
global using SmartAttendanceSystem.Core.Settings;
global using SmartAttendanceSystem.Core.Entities;
global using SmartAttendanceSystem.Core.Errors;

#endregion