namespace Tmp.Model.DatabaseEntity;

public sealed class User : EntityBase
{
    public string? EntraId { get; set; }

    public string? NickName { get; set; }

    public string? Role { get; set; }

    public string? Description { get; set; }
}
