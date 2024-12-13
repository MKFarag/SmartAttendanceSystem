#region Packges | Microsoft

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Hangfire;

#endregion

//////////////////////////////////////////////////////////////////////////////////

#region MyMethods

global using SmartAttendanceSystem.Application.Abstraction.DTOsServices;
global using SmartAttendanceSystem.Application.Contracts.Authentication;
global using SmartAttendanceSystem.Application.Contracts.Department;
global using SmartAttendanceSystem.Application.Contracts.Student;
global using SmartAttendanceSystem.Application.Contracts.Course;
global using SmartAttendanceSystem.Application.Contracts.Users;
global using SmartAttendanceSystem.Presentation.Abstraction;
global using SmartAttendanceSystem.Presentation.Extensions;
global using SmartAttendanceSystem.Core.Abstraction;
global using SmartAttendanceSystem.Core.Services;
global using SmartAttendanceSystem.Core.Entities;
global using SmartAttendanceSystem.Core.Errors;

#endregion