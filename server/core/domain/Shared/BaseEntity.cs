using LocadoraDeAutomoveis.Core.Domain.Auth;

namespace LocadoraDeAutomoveis.Core.Domain.Shared;

public abstract class BaseEntity<Tipo>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }

    protected BaseEntity() => this.Id = Guid.NewGuid();

    public abstract void Update(Tipo registroEditado);
}
