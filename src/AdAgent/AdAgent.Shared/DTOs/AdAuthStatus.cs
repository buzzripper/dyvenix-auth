
namespace Dyvenix.AdAgent.Shared.DTOs;

public enum AdAuthStatus
{
	Success,
	InvalidCredentials,
	UserNotFound,
	AccountLocked,
	PasswordExpired,
	DomainUnavailable,
	InternalError,
	UnknownError
}
