namespace LeaveFlowHR.Api.Configurations;

public class AppConfiguration
{
    public ConnectionStringConfiguration ConnectionStrings { get; set; } = new();
    public JwtConfiguration Jwt { get; set; } = new();
}

public class ConnectionStringConfiguration
{
    public string DefaultConnection { get; set; } = string.Empty;
}

public class JwtConfiguration
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiryInMinutes { get; set; } = 60;
}
