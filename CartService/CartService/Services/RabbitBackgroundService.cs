using Shop.CartService.Abstractions;
using Shop.CartService.Model;
using Shop.Core.Messages;

namespace Shop.CartService.Services;

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

            IRepository<CartItem>? cartRepository = scope.ServiceProvider.GetService<IRepository<CartItem>>();
            var searchRepository = scope.ServiceProvider.GetService<IProductSearchRepository>();

            if (cartRepository == null)
                throw new InvalidOperationException("No IRepository<CartItem> is registered.");

            if (searchRepository == null)
                throw new InvalidOperationException("No IProductSearchRepository is registered.");
                
            foreach (var message in messages)
            {
                logger.LogError(message.ToString());
                
                var items = await searchRepository.GetByProductId(message.ProductId);
                foreach (var cartItem in items)
                {
                    cartItem.Quantity = message.QuantityInStock;
                    await cartRepository.Update(cartItem);
                }
            }
        }
    }
}