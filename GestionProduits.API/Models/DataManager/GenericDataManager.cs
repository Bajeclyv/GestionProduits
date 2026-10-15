using AutoMapper;
using AutoMapper.QueryableExtensions;
using GestionProduits.API.Models.DTO;
using GestionProduits.API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace GestionProduits.API.Models.DataManager;

public class GenericDataManager<TEntity, TDto> : IDataRepository<TDto>
    where TEntity : class
    where TDto : class, IEntityDto
{
    protected readonly DbContext Context;
    protected readonly IMapper Mapper;
    private readonly List<(TDto Dto, TEntity Entity)> _pendingAdds = [];

    public GenericDataManager(DbContext context, IMapper mapper)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<TDto>> GetAllAsync()
    {
        return await Context.Set<TEntity>()
            .ProjectTo<TDto>(Mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<TDto?> GetByIdAsync(int id)
    {
        var keyName = GetKeyName();
        return await Context.Set<TEntity>()
            .Where(entity => EF.Property<int>(entity, keyName) == id)
            .ProjectTo<TDto>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(TDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        await Context.Set<TEntity>().AddAsync(entity);
        _pendingAdds.Add((dto, entity));
    }

    public async Task UpdateAsync(TDto entityToUpdate, TDto entity)
    {
        var trackedEntity = await Context.Set<TEntity>().FindAsync(entityToUpdate.Id);
        if (trackedEntity != null)
        {
            Mapper.Map(entity, trackedEntity);
        }
    }

    public async Task DeleteAsync(TDto entity)
    {
        var trackedEntity = await Context.Set<TEntity>().FindAsync(entity.Id);
        if (trackedEntity != null)
        {
            Context.Set<TEntity>().Remove(trackedEntity);
        }
    }

    public async Task SaveAsync()
    {
        await Context.SaveChangesAsync();

        var keyName = GetKeyName();
        foreach (var (dto, entity) in _pendingAdds)
        {
            dto.Id = Convert.ToInt32(Context.Entry(entity).Property(keyName).CurrentValue);
        }

        _pendingAdds.Clear();
    }

    private string GetKeyName()
    {
        return Context.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties.Single().Name
            ?? throw new InvalidOperationException($"No primary key is configured for {typeof(TEntity).Name}.");
    }
}
