# Smart Attendance System

A comprehensive fingerprint-based attendance management solution built with ASP.NET Core 9.0 using clean architecture principles.

<!-- ![Smart Attendance System Logo](logo_url_here) -->

## 📋 Overview

The Smart Attendance System is a graduation project designed to streamline attendance tracking in educational institutions using biometric fingerprint authentication. This system replaces traditional paper-based attendance methods with a secure, efficient, and tamper-proof digital solution.

## ✨ Features

- **Biometric Authentication**: Secure fingerprint-based verification for student attendance
- **User Management**: Comprehensive user roles (administrators, instructors, students)
- **Course Management**: Create, update, and manage courses and their attendance requirements
- **Department Organization**: Organize courses by academic departments
- **Real-time Attendance Tracking**: Mark and monitor student attendance instantly
- **JWT Authentication**: Secure API access with token-based authentication
- **Role-based Access Control**: Different capabilities based on user roles
- **API Versioning**: Support for evolving API endpoints while maintaining backward compatibility
- **Health Monitoring**: Built-in system health checks
<!-- - **Weekly Attendance Reports**: Generate detailed reports on student attendance patterns -->

## 🏗️ Architecture

The project follows Clean Architecture principles with a clear separation of concerns:

![Clean Architecture Diagram](https://miro.medium.com/v2/resize:fit:500/1*sura91gPMoCjPNvZWsAO_g.png)

### Core Layer (SmartAttendanceSystem.Core)
- Domain entities (Student, Course, Attendance, etc.)
- Business rules and domain logic

### Application Layer (SmartAttendanceSystem.Application)
- Implementation of use cases
- Service interfaces and implementations
- DTOs and mappers
- Business logic orchestration

### Infrastructure Layer (SmartAttendanceSystem.Infrastructure)
- Database context and configurations
- Entity Framework Core implementation
- Repository implementations
- External service integrations
- Authentication services

### Presentation Layer (SmartAttendanceSystem.Presentation)
- API controllers and endpoints
- Request/response models
- Middleware configurations
- Dependency injection setup

### Fingerprint Module (SmartAttendanceSystem.Fingerprint)
- Specialized services for fingerprint processing
- Integration with fingerprint hardware
- Biometric data handling

## 🧩 Domain Model

The system is built around the following key entities:

- **ApplicationUser**: Base user entity with authentication properties
- **Student**: Represents students who can have attendance records
- **Department**: Academic departments that organize courses
- **Course**: Subjects offered by departments
- **Attendance**: Records of student presence in courses
- **Weeks**: Represents academic weeks for attendance tracking

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: JWT (JSON Web Tokens)
- **API Documentation**: Scalar
- **Background Processing**: Hangfire
- **Logging**: Serilog
- **Health Monitoring**: AspNetCore.HealthChecks
- **API Versioning**: Asp.Versioning

## 📝 API Documentation

The API includes endpoints for:
- User authentication and management
- Department and course management
- Student registration and management
- Fingerprint registration and verification
- Attendance tracking and reporting

## 🔒 Security

- JWT-based authentication
- Role-based authorization
- Secure fingerprint data handling
- HTTPS communication
