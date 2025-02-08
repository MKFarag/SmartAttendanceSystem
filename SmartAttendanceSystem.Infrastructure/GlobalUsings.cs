#region Packges | Microsoft

global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using System.ComponentModel.DataAnnotations;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using System.Linq.Expressions;
global using Mapster;

#endregion

//////////////////////////////////////////////////////////////////////////////////

#region MyMethods

global using SmartAttendanceSystem.Infrastructure.Persistence;
global using SmartAttendanceSystem.Core.Abstraction.Constants;
global using SmartAttendanceSystem.Application.Interfaces;
global using SmartAttendanceSystem.Application.Helpers;
global using SmartAttendanceSystem.Core.Abstraction;
global using SmartAttendanceSystem.Core.Entities;
global using SmartAttendanceSystem.Core.Errors;

#endregion

//////////////////////////////////////////////////////////////////////////////////

#region DTOs

global using SmartAttendanceSystem.Application.Contracts.Department.Responses;
global using SmartAttendanceSystem.Application.Contracts.Department.Requests;
global using SmartAttendanceSystem.Application.Contracts.Student.Responses;
global using SmartAttendanceSystem.Application.Contracts.Course.Responses;
global using SmartAttendanceSystem.Application.Contracts.Course.Requests;
global using SmartAttendanceSystem.Application.Contracts.Common;

#endregion