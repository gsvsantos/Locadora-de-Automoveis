using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Domain.Groups;

public class Group : BaseEntity<Group>
{
    public string Name { get; set; } = string.Empty;
    public List<Vehicle> Vehicles { get; set; } = [];

    public Group() { }
    public Group(string name) : this() => this.Name = name;

    public override void Update(Group updatedEntity) => this.Name = updatedEntity.Name;
}
