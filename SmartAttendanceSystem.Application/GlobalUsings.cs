#region Packges | Microsoft

global using Microsoft.AspNetCore.Identity.UI.Services;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Logging;
global using System.Security.Cryptography;
global using Microsoft.AspNetCore.Http;
global using System.Linq.Expressions;
global using FluentValidation;
global using System.Text;
global using Mapster;

#endregion

//////////////////////////////////////////////////////////////////////////////////

#region MyMethods

global using SmartAttendanceSystem.Core.Abstraction.Constants;
global using SmartAttendanceSystem.Application.Interfaces;
global using SmartAttendanceSystem.Application.Helpers;
global using SmartAttendanceSystem.Core.Abstraction;
global using SmartAttendanceSystem.Core.Entities;
global using SmartAttendanceSystem.Core.Settings;
global using SmartAttendanceSystem.Core.Errors;

#endregion

//////////////////////////////////////////////////////////////////////////////////

#region DTOs

global using SmartAttendanceSystem.Application.Contracts.Authentication.Responses;
global using SmartAttendanceSystem.Application.Contracts.Authentication.Requests;
global using SmartAttendanceSystem.Application.Contracts.Department.Responses;
global using SmartAttendanceSystem.Application.Contracts.Department.Requests;
global using SmartAttendanceSystem.Application.Contracts.Student.Responses;
global using SmartAttendanceSystem.Application.Contracts.Student.Requests;
global using SmartAttendanceSystem.Application.Contracts.Course.Responses;
global using SmartAttendanceSystem.Application.Contracts.Course.Requests;
global using SmartAttendanceSystem.Application.Contracts.Users.Responses;
global using SmartAttendanceSystem.Application.Contracts.Users.Requests;
global using SmartAttendanceSystem.Application.Contracts.Roles.Responses;
global using SmartAttendanceSystem.Application.Contracts.Roles.Requests;
global using SmartAttendanceSystem.Application.Contracts.Common;

#endregion