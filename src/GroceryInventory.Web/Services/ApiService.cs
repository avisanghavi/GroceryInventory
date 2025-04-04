using System.Net.Http.Json;
using GroceryInventory.Core.Models;

namespace GroceryInventory.Web.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5003/api/");
    }

    // Grocery Items
    public async Task<IEnumerable<GroceryItem>> GetGroceryItemsAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<GroceryItem>>("groceryitems") ?? Array.Empty<GroceryItem>();
    }

    public async Task<GroceryItem?> GetGroceryItemAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<GroceryItem>($"groceryitems/{id}");
    }

    public async Task<GroceryItem> CreateGroceryItemAsync(GroceryItem item)
    {
        var response = await _httpClient.PostAsJsonAsync("groceryitems", item);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GroceryItem>() ?? throw new Exception("Failed to create grocery item");
    }

    public async Task UpdateGroceryItemAsync(int id, GroceryItem item)
    {
        var response = await _httpClient.PutAsJsonAsync($"groceryitems/{id}", item);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteGroceryItemAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"groceryitems/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Suppliers
    public async Task<IEnumerable<Supplier>> GetSuppliersAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Supplier>>("suppliers") ?? Array.Empty<Supplier>();
    }

    public async Task<Supplier?> GetSupplierAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Supplier>($"suppliers/{id}");
    }

    public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
    {
        var response = await _httpClient.PostAsJsonAsync("suppliers", supplier);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Supplier>() ?? throw new Exception("Failed to create supplier");
    }

    public async Task UpdateSupplierAsync(int id, Supplier supplier)
    {
        var response = await _httpClient.PutAsJsonAsync($"suppliers/{id}", supplier);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteSupplierAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"suppliers/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Orders
    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Order>>("orders") ?? Array.Empty<Order>();
    }

    public async Task<Order?> GetOrderAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Order>($"orders/{id}");
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        var response = await _httpClient.PostAsJsonAsync("orders", order);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Order>() ?? throw new Exception("Failed to create order");
    }

    public async Task UpdateOrderAsync(int id, Order order)
    {
        var response = await _httpClient.PutAsJsonAsync($"orders/{id}", order);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteOrderAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"orders/{id}");
        response.EnsureSuccessStatusCode();
    }
}