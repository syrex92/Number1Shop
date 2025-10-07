using Shop.Core.Messages;
using Shop.FavoriteService.Abstractions;
using Shop.FavoriteService.Model;

namespace Shop.FavoriteService.Services;

internal class RabbitBackgroundService(
    ILogger<RabbitBackgroundService> logger, 
    IServiceProvider services,
    IMessageListener<ProductMessage> messageListener
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        using var scope = services.CreateScope();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = await messageListener.GetMessages(stoppingToken);

            if (messages.Length == 0)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            IRepository<FavoriteItem>? favoriteRepository = scope.ServiceProvider.GetService<IRepository<FavoriteItem>>();
            var searchRepository = scope.ServiceProvider.GetService<IProductSearchRepository>();

            if (favoriteRepository == null)
                throw new InvalidOperationException("No IRepository<FavoriteItem> is registered.");

            if (searchRepository == null)
                throw new InvalidOperationException("No IProductSearchRepository is registered.");
                
            foreach (var message in messages)
            {
                logger.LogError(message.ToString());
                
                var items = await searchRepository.GetByProductId(message.ProductId);
                foreach (var favoriteItem in items)
                {
                    //cartItem.Quantity = message.QuantityInStock;
                    await favoriteRepository.Update(favoriteItem);
                }
            }
        }
    }
}