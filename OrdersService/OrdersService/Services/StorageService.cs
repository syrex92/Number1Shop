using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OrdersService.Interfaces;
using OrdersService.Models;

namespace OrdersService.Services
{
    public class StorageService: IStorageService
    {
        private readonly HttpClient _httpClient;
        public StorageService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("StorageService");
        }
        public async Task<bool> CheckAvailability(Guid productId, int quantity)
        {
            var response = await _httpClient.GetAsync($"/availability/{productId}?quantity={quantity}");
            response.EnsureSuccessStatusCode();
            var availabilityAnswer = await response.Content.ReadFromJsonAsync<AvailabilityResponse>();
            if (availabilityAnswer == null)
            {
                throw new Exception("Invalid response from storage service");
            }
            return availabilityAnswer.IsAvailable;
        }
        public async Task<Guid> Reserve(Order order)
        {
            var response = await _httpClient.PostAsync($"/reserve", JsonContent.Create(new ReserveRequest
            {
                OrderId = order.Id,
                Items = order.Items.Select(item => new ReserveRequestItem
                {
                    ProductId = item.Product,
                    Quantity = item.Quantity
                }).ToList()
            }));
            response.EnsureSuccessStatusCode();
            var reserveResponse = await response.Content.ReadFromJsonAsync<ReserveResponse>();
            if (reserveResponse == null || !reserveResponse.Success)
            {
                throw new Exception("Failed to reserve items: " + reserveResponse?.ErrorMessage);
            }
            return reserveResponse.ReservationId;
        }
        public async Task ConfirmReservation(Guid orderId, Guid reservationId)
        {
            var response = await _httpClient.PostAsync($"/confirm", JsonContent.Create(new ConfirmRequest
            {
                OrderId = orderId,
                ReservationIds = new List<Guid> { reservationId }
            }));
            response.EnsureSuccessStatusCode();
            var confirmResponse = await response.Content.ReadFromJsonAsync<ConfirmResponse>();
            if (confirmResponse == null || !confirmResponse.Success)
            {
                throw new Exception("Failed to confirm reservation: " + confirmResponse?.ErrorMessage);
            }            
        }
        public async Task CancelReservation(Guid reservationId)
        {
            var response = await _httpClient.DeleteAsync($"/reserve/{reservationId}");
            response.EnsureSuccessStatusCode();
        }
        public async Task<IStockInfo> GetStockInfo(Guid productId)
        {
            var response = await _httpClient.GetAsync($"/stock/{productId}");
            response.EnsureSuccessStatusCode();
            var text = await response.Content.ReadAsStringAsync();
            var stockResponse = await response.Content.ReadFromJsonAsync<StockInfo>();
            if (stockResponse == null || !string.IsNullOrEmpty(stockResponse.Error))
            {
                throw new Exception("Invalid response from storage service: " + stockResponse?.Error);
            }
            return stockResponse;
        }
    }

    internal class ConfirmResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("orderId")]
        public Guid OrderId { get; set; }
        [JsonPropertyName("confirmedItems")]
        public List<ConfirmResponseItem> ConfirmedItems { get; set; } = new();
        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;        
    }

    public class ConfirmResponseItem
    {
        [JsonPropertyName("productId")]
        public Guid ProductId { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

    internal class ConfirmRequest
    {
        [JsonPropertyName("orderId")]
        public Guid OrderId { get; set; }
        [JsonPropertyName("reservationIds")]
        public List<Guid> ReservationIds { get; set; } = new();
    }

    internal class ReserveResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("reservationId")]
        public Guid ReservationId { get; set; }
        [JsonPropertyName("expiresAt")]
        public string ExpiresAt { get; set; } = string.Empty;
        [JsonPropertyName("reservedItems")]
        public List<ReserveResponseReservedItem> ReservedItems { get; set; } = new();
        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;
    }

    internal class ReserveResponseReservedItem
    {
        [JsonPropertyName("productId")]
        public Guid ProductId { get; set; }
        [JsonPropertyName("reservedQuantity")]
        public int ReservedQuantity { get; set; }
        [JsonPropertyName("reservationDetailId")]
        public Guid ReservationDetailId { get; set; }
    }

    internal class ReserveRequestItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    internal class ReserveRequest
    {
        public Guid OrderId { get; set; }
        public List<ReserveRequestItem> Items { get; set; } = new();
        public string ReservationExpiry { get; set; } = DateTimeOffset.UtcNow.AddMinutes(15).ToString("o");

    }

    internal class AvailabilityResponse
    {
        [JsonPropertyName("isAvailable")]
        public bool IsAvailable { get; set; }
        [JsonPropertyName("availableQuantity")]
        public int AvailableQuantity { get; set; }
        [JsonPropertyName("checkedAt")]
        public string CheckedAt { get; set; } = string.Empty;
        [JsonPropertyName("productId")]
        public Guid ProductId { get; set; }
        [JsonPropertyName("requestedQuantity")]
        public int RequestedQuantity { get; set; }
    }
}