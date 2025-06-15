using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PhaserIDE.Services;

public class NpmService
{
    private static readonly HttpClient _httpClient = new();

    /// <summary>
    /// Asynchronously retrieves all available versions of a given npm package
    /// from the official npm registry (https://registry.npmjs.org).
    /// </summary>
    /// <param name="packageName">The name of the npm package (e.g., "react").</param>
    /// <returns>
    /// A list of version strings (e.g., "18.2.0", "17.0.2"), corresponding to
    /// the keys of the "versions" object in the registry response.
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Thrown when the HTTP request fails (e.g., network issues, package not found).
    /// </exception>
    /// <exception cref="JsonException">
    /// Thrown when the JSON response cannot be parsed or lacks a "versions" field.
    /// </exception>

    public async Task<List<string>> GetPackageVersionsAsync(string packageName)
    {
        var url = $"https://registry.npmjs.org/{packageName}";
        var versionList = new List<string>();

        var response = await _httpClient.GetAsync(url);
        try
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(content);

            // 1. Try to get the "latest" version from dist-tags
            string? latestVersion = null;
            if (json.RootElement.TryGetProperty("dist-tags", out var distTags))
            {
                if (distTags.TryGetProperty("latest", out var latestElement))
                {
                    latestVersion = latestElement.GetString();
                }
            }

            // 2. Get all versions from the "versions" object
            var versionsProp = json.RootElement.GetProperty("versions");
            foreach (var version in versionsProp.EnumerateObject())
            {
                versionList.Add(version.Name);
            }

            // 3. Ensure "latest" version is at index 0
            if (latestVersion != null)
            {
                versionList.Remove(latestVersion); // prevent duplicate
                versionList.Insert(0, latestVersion);
            }
        }
        catch
        {
            Debug.WriteLine("Could not read Versions from NPM Registry.");
        }

        return versionList;
    }
}
