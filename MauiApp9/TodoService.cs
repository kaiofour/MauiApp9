using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MauiApp9;

using System.Net.Http.Json;
using System.Text.Json;


public class TodoService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://todo-list.dcism.org"; // Replace with actual API URL

    public static ObservableCollection<TodoItem> PendingTasks { get; } = new();
    public static ObservableCollection<TodoItem> CompletedTasks { get; } = new();
    public static TodoItem SelectedItem { get; set; }

    public TodoService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    // API Methods
    public async Task<(bool success, string message)> GetUserInfo(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"getUser_action.php?email={Uri.EscapeDataString(email)}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<UserInfo>>(content);

            if (result.status == 200)
            {
                Preferences.Set("user_id", result.data.user_id);
                return (true, result.message);
            }
            return (false, result.message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool success, string message, List<TodoItem> items)> GetTodoItems(string status, int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"getItems_action.php?status={status}&user_id={userId}");
            var content = await response.Content.ReadAsStringAsync();

            // Debug output
            Console.WriteLine($"API Response: {content}");

            var result = JsonSerializer.Deserialize<ApiResponse<Dictionary<string, TodoItem>>>(content);

            if (result.status == 200)
            {
                // Handle empty data case
                if (result.data == null || result.data.Count == 0)
                {
                    return (true, "No tasks found", new List<TodoItem>());
                }
                return (true, result.message, result.data.Values.ToList());
            }
            return (false, result.message, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }


    public async Task<(bool success, string message, TodoItem item)> AddTodoItem(string name, string description, int userId)
    {
        try
        {
            var data = new { item_name = name, item_description = description, user_id = userId };
            var response = await _httpClient.PostAsJsonAsync("addItem_action.php", data);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<TodoItem>>(content);

            if (result.status == 200)
            {
                return (true, result.message, result.data);
            }
            return (false, result.message, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<(bool success, string message)> UpdateTodoItem(int itemId, string name, string description)
    {
        try
        {
            var data = new { item_id = itemId, item_name = name, item_description = description };
            var response = await _httpClient.PutAsJsonAsync("editItem_action.php", data);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<object>>(content);

            return (result.status == 200)
                ? (true, result.message)
                : (false, result.message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool success, string message)> ChangeTodoStatus(int itemId, string status)
    {
        try
        {
            var data = new { item_id = itemId, status = status };
            var response = await _httpClient.PutAsJsonAsync("statusItem_action.php", data);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<object>>(content);

            return (result.status == 200)
                ? (true, result.message)
                : (false, result.message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool success, string message)> DeleteTodoItem(int itemId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"deleteItem_action.php?item_id={itemId}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<object>>(content);

            return (result.status == 200)
                ? (true, result.message)
                : (false, result.message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    // Helper classes
    private class ApiResponse<T>
    {
        public int status { get; set; }
        public string message { get; set; }
        public T data { get; set; } = default; // Initialize with default value
        public int count { get; set; }
    }

    private class UserInfo
    {
        public int user_id { get; set; }
        public string email { get; set; }
        public string timemodified { get; set; }
    }
}