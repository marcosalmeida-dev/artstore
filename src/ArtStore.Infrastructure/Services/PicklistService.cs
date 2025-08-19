using Microsoft.Extensions.Caching.Memory;

namespace ArtStore.Infrastructure.Services;

public class PicklistService : IPicklistService
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _fusionCache;

    public PicklistService(
        IMemoryCache fusionCache,
        IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        _fusionCache = fusionCache;
    }

    public event  Func<Task>? OnChange;
    //public List<PicklistSetDto> DataSource { get; private set; } = new();


    public void Initialize()
    {
        //DataSource = _fusionCache.GetOrCreate(PicklistSetCacheKey.PicklistCacheKey,
        //    _ => _context.PicklistSets.ProjectTo<PicklistSetDto>(_mapper.ConfigurationProvider)
        //        .OrderBy(x => x.Name).ThenBy(x => x.Value)
        //        .ToList()
        //) ?? new List<PicklistSetDto>();
    }

    public void Refresh()
    {
        //_fusionCache.Remove(PicklistSetCacheKey.PicklistCacheKey);
        //DataSource = _fusionCache.GetOrSet(PicklistSetCacheKey.PicklistCacheKey,
        //    _ => _context.PicklistSets.ProjectTo<PicklistSetDto>(_mapper.ConfigurationProvider)
        //        .OrderBy(x => x.Name).ThenBy(x => x.Value)
        //        .ToList()
        //) ?? new List<PicklistSetDto>();
        //OnChange?.Invoke();
    }
}