#region Packges | Microsoft

global using Mapster;
global using System.Text;
global using FluentValidation;
global using System.Linq.Expressions;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Logging;
global using System.Security.Cryptography;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.UI.Services;

#endregion

//////////////////////////////////////////////////////////////////////////////////

#region MyMethods

global using SmartAttendanceSystem.Application.Contracts.Authentication;
global using SmartAttendanceSystem.Application.Abstraction.DTOsServices;
global using SmartAttendanceSystem.Application.Contracts.Department;
global using SmartAttendanceSystem.Application.Contracts.Student;
global using SmartAttendanceSystem.Application.Contracts.Course;
global using SmartAttendanceSystem.Infrastructure.Repositories;
global using SmartAttendanceSystem.Core.Abstraction.Constants;
global using SmartAttendanceSystem.Infrastructure.Persistence;
global using SmartAttendanceSystem.Core.GenericServices;
global using SmartAttendanceSystem.Application.Helpers;
global using SmartAttendanceSystem.Core.Abstraction;
global using SmartAttendanceSystem.Core.Entities;
global using SmartAttendanceSystem.Core.Settings;
global using SmartAttendanceSystem.Core.Errors;

#endregion