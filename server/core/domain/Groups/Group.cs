using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Groups;

public class Group : BaseEntity<Group>
{
    public string Name { get; set; } = string.Empty;

    public override void Update(Group updatedEntity) => this.Name = updatedEntity.Name;
}
