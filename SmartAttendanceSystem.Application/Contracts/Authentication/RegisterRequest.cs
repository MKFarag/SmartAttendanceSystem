namespace SmartAttendanceSystem.Application.Contracts.Authentication;

public record RegisterRequest(
    string Email,    
    string Password,    
    string Name 
);
