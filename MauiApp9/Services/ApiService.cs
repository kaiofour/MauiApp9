using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp9.Models;
using System.Net.Http;
using System.Text.Json;


namespace MauiApp9.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;

        public ApiService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://todo-list.dcism.org")
            };
        }

        // Add this method to ApiService.cs
        public async Task<string> SignUpAsync(SignUpModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/signup_action.php", content);
            return await response.Content.ReadAsStringAsync();
        }


        public async Task<string> SignInAsync(string email, string password)
        {
            var url = $"/signin_action.php?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";
            var response = await _client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetTasksAsync(string status, int userId)
        {
            var url = $"/getItems_action.php?status={Uri.EscapeDataString(status)}&user_id={userId}";
            var response = await _client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AddTaskAsync(string item_name, string item_description, int user_id, string status)
        {
            var taskData = new
            {
                item_name,
                item_description,
                user_id,
                status
            };

            var json = JsonSerializer.Serialize(taskData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/addItem_action.php", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> EditTaskAsync(int item_id, string item_name, string item_description)
        {
            var updatedTask = new
            {
                item_id,
                item_name,
                item_description
            };

            var json = JsonSerializer.Serialize(updatedTask);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, "/editItem_action.php")
            {
                Content = content
            };

            var response = await _client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> DeleteTaskAsync(int item_id)
        {
            var response = await _client.DeleteAsync($"/deleteItem_action.php?item_id={item_id}");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ToggleTaskStatusAsync(int item_id, string newStatus)
        {
            var statusData = new
            {
                item_id,
                status = newStatus
            };

            var json = JsonSerializer.Serialize(statusData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, "/statusItem_action.php")
            {
                Content = content
            };

            var response = await _client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
