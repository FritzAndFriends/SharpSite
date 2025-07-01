namespace SharpSite.Abstractions.Security;

/// <summary>
/// Provider-agnostic login information.
/// </summary>
public interface ILoginInfo
{
    string LoginProvider { get; }
    string ProviderKey { get; }
    string ProviderDisplayName { get; }
}

/// <summary>
/// Provider-agnostic sign-in result.
/// </summary>
public class SignInResult
{
	public SignInResult(bool succeeded, bool isLockedOut = false, bool isNotAllowed = false, bool requiresTwoFactor = false)
	{
		Succeeded = succeeded;
		IsLockedOut = isLockedOut;
		IsNotAllowed = isNotAllowed;
		RequiresTwoFactor = requiresTwoFactor;
	}

	public bool Succeeded { get; }
	public bool IsLockedOut { get; }
	public bool IsNotAllowed { get; }
	public bool RequiresTwoFactor { get; }

	public static SignInResult Success => new SignInResult(true);
	public static SignInResult Failed => new SignInResult(false);
	public static SignInResult LockedOut => new SignInResult(false, isLockedOut: true);
	public static SignInResult NotAllowed => new SignInResult(false, isNotAllowed: true);
	public static SignInResult TwoFactorRequired => new SignInResult(false, requiresTwoFactor: true);
		


}

/// <summary>
/// Provider-agnostic operation result.
/// </summary>
public class IdentityResult
{
    private readonly IEnumerable<IdentityError> _errors;
    
    public IdentityResult(bool succeeded, IEnumerable<IdentityError>? errors = null)
    {
        Succeeded = succeeded;
        _errors = errors ?? Array.Empty<IdentityError>();
    }

    public bool Succeeded { get; }
    public IEnumerable<IdentityError> Errors => _errors;

    public static IdentityResult Success => new IdentityResult(true);
    public static IdentityResult Failed(params IdentityError[] errors) => new IdentityResult(false, errors);
}

/// <summary>
/// Provider-agnostic error information.
/// </summary>
public class IdentityError
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Provider-agnostic authentication scheme information.
/// </summary>
public class AuthenticationScheme
{
    public AuthenticationScheme(string name, string displayName, string handlerType)
    {
        Name = name;
        DisplayName = displayName;
        HandlerType = handlerType;
    }

    public string Name { get; }
    public string DisplayName { get; }
    public string HandlerType { get; }
}
