namespace LocadoraDeAutomoveis.Core.Application.Auth.DTOs;

public class UserAuthenticatedDto
{
    public required Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
}