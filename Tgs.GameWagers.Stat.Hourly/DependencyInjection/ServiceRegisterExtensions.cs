using Tgs.GameWagers.Stat.Hourly.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
    public static async Task<WebApplication> UseHangfireRegisterAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var hangfireSchedule = scope.ServiceProvider.GetRequiredService<IHangfireUtensil>();
            await hangfireSchedule.JobInitAsync().ConfigureAwait(false);
        }

        return app;
    }
}
