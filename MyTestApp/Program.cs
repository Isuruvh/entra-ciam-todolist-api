using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

using var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};
using HttpClient client = new HttpClient(handler);

var clientId = "70a5c0ab-b454-4ee4-ac4a-7570ff6d3fc3";
var clientSecret = Environment.GetEnvironmentVariable("TODO_CLIENT_SECRET")
    ?? throw new InvalidOperationException("Set the TODO_CLIENT_SECRET environment variable before running MyTestApp.");
var scopes = new[] {"api://70a5c0ab-b454-4ee4-ac4a-7570ff6d3fc3/.default"};
var tenantId = "0388de7b-f0a0-4e12-90f1-47ece33a581e";
var authority = $"https://isurutraderexternal.ciamlogin.com/{tenantId}";

var app = ConfidentialClientApplicationBuilder
    .Create(clientId)
    .WithAuthority(authority)
    .WithClientSecret(clientSecret)
    .Build();

var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
Console.WriteLine("Access token acquired.");

client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
var response = await client.GetAsync("https://localhost:44351/api/todolist");
var content = await response.Content.ReadAsStringAsync();

Console.WriteLine("Your response is: " + response.StatusCode);
Console.WriteLine(content);