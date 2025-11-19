using Locadora_de_Automoveis.Core.Dominio.Shared;
using Microsoft.EntityFrameworkCore;

namespace Locadora_de_Automoveis.Infraestrutura.ORM.Shared;

public class BaseRepository<T>(AppDbContext contexto) where T : BaseEntity<T>
{
    protected readonly DbSet<T> records = contexto.Set<T>();

    public async Task AddAsync(T novoRegistro) => await this.records.AddAsync(novoRegistro);

    public async Task AddMultiplyAsync(IList<T> entidades) => await this.records.AddRangeAsync(entidades);

    public async Task<bool> UpdateAsync(Guid idRegistro, T registroEditado)
    {
        T? registroSelecionado = await GetByIdAsync(idRegistro);

        if (registroSelecionado is null)
        {
            return false;
        }

        registroSelecionado.AtualizarRegistro(registroEditado);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid idRegistro)
    {
        T? registroSelecionado = await GetByIdAsync(idRegistro);

        if (registroSelecionado is null)
        {
            return false;
        }

        this.records.Remove(registroSelecionado);

        return true;
    }

    public virtual async Task<List<T>> GetAllAsync() => await this.records.ToListAsync();

    public virtual async Task<List<T>> GetAllAsync(int quantidade) => await this.records.Take(quantidade).ToListAsync();

    public virtual async Task<T?> GetByIdAsync(Guid idRegistro) => await this.records.FirstOrDefaultAsync(x => x.Id.Equals(idRegistro));
}
