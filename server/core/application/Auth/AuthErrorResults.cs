using FluentResults;

namespace LocadoraDeAutomoveis.Application.Auth;

public abstract class AuthErrorResults
{
    public static Error PendingEmailConfirmationError(string email)
    {
        return new Error("Pending email confirmation")
            .CausedBy($"Email confirmation for \"{email}\" is pending")
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error PhoneNumberNotConfirmedError(string phoneNumber)
    {
        return new Error("Phone number not confirmed")
            .CausedBy($"The phone number \"{phoneNumber}\" must be confirmed to proceed.")
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error LoginNotAllowedError()
    {
        return new Error("Login not allowed")
            .CausedBy("Login is blocked due to security restrictions (e.g., unconfirmed email or phone number).")
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error IncorrectCredentialsError()
    {
        return new Error("Incorrect credentials")
            .CausedBy("The login or password are incorrect")
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error UserLockedOutError()
    {
        return new Error("User locked out")
            .CausedBy("Access for this user has been locked out, try again later")
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error UserNotFoundError(string name)
    {
        return new Error("User not found")
            .CausedBy($"The user with login '{name}' was not found")
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error TwoFactorRequiredError()
    {
        return new Error("Two-factor authentication required")
            .CausedBy("Login confirmation with two-factor authentication is required.")
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error PasswordConfirmationError()
    {
        return new Error("Passwords do not match")
            .CausedBy("The confirmation password does not match the password.")
            .WithMetadata("ErrorType", "BadRequest");
    }
}