using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;

namespace ODBP.Features.Sitemap
{
    /// <summary>
    /// A simple abstraction over <see cref="IMemoryCache"/> with <see href="https://en.wikipedia.org/wiki/Cache_stampede">stampede protection</see>
    /// </summary>
    /// <param name="cache"></param>
    public interface ISimpleCache
    {
        /// <summary>
        /// Asynchronously gets the value associated with this key if it exists, or generates a new entry using the provided key and a value from the given factory if the key is not found. Does not cache null values.
        /// </summary>
        /// <typeparam name="TItem">The type of the object to get.</typeparam>
        /// <param name="key">The key of the entry to look for or create.</param>
        /// <param name="absoluteExpirationRelativeToNow">The duration from now after which the cache entry will expire.</param>
        /// <param name="factory">The factory task that creates the value associated with this key if the key does not exist in the cache.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        ValueTask<TItem> GetOrSetAsync<TItem>(string key, TimeSpan absoluteExpirationRelativeToNow, Func<Task<TItem>> factory);
    }

    public class SimpleCache(IMemoryCache cache) : ISimpleCache
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> s_locks = new();

        public ValueTask<TItem> GetOrSetAsync<TItem>(string key, TimeSpan absoluteExpirationRelativeToNow, Func<Task<TItem>> factory) =>
            TryGetCached<TItem>(key, out var value)
                ? new ValueTask<TItem>(value)
                : SetAsyncWithLock(key, absoluteExpirationRelativeToNow, factory);

        private bool TryGetCached<T>(string key, [NotNullWhen(true)] out T? value) => cache.TryGetValue(key, out value) && value != null;

        private async ValueTask<T> SetAsyncWithLock<T>(string cacheKey, TimeSpan cacheDuration, Func<Task<T>> factory)
        {
            var asyncLock = s_locks.GetOrAdd(cacheKey, _ => new(1, 1));
            await asyncLock.WaitAsync();
            try
            {
                // the value might have been set while we were aquiring the lock, check again to be safe
                if (TryGetCached<T>(cacheKey, out var cached)) return cached;
                var fresh = await factory();
                if (fresh != null)
                {
                    cache.Set(cacheKey, fresh, cacheDuration);
                }
                return fresh;
            }
            finally
            {
                asyncLock.Release();
            }
        }
    }
}
