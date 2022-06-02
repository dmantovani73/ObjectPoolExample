using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

var app = builder.Build();

app.MapGet("/", Hello);

app.Run();

async Task<string> Hello(ObjectPoolProvider objectPoolProvider)
{
    var objectPool = objectPoolProvider.Create<Stopwatch>();

    Stopwatch timer = default!;    
    try
    {
        timer = objectPool.Get();
        
        timer.Start();
        await Task.Delay(Random.Shared.Next(50, 100));
        timer.Stop();

        return $"Elapsed: {timer.ElapsedMilliseconds}ms";
    }
    finally
    {
        if (timer != null) objectPool.Return(timer);
    }
}