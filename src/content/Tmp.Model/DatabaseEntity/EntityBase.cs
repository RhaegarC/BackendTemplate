namespace Tmp.Model.DatabaseEntity;

public abstract class EntityBase
{
    public string? Id { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime LastModifiedOn { get; set; }

    public bool? IsDeleted { get; set; }
}
