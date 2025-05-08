using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using ConfigRig.Lib.Models;

namespace ConfigRig.Lib.Service;

public class ConfigRig
{
    private readonly IConfigurationRoot _configuration;

    public ConfigRig(string configDirectory)
    {
        var builder = new ConfigurationBuilder();

        if (Directory.Exists(configDirectory))
        {
            foreach (var file in Directory.GetFiles(configDirectory, "appsettings.*.json"))
            {
                builder.AddJsonFile(file, optional: true, reloadOnChange: true);
            }
        }

        _configuration = builder.Build();
    }

    // ---------------------------
    // Strongly Typed Config Access
    // ---------------------------
    public T GetStronglyTypedConfig<T>(string sectionName) where T : new()
    {
        var section = _configuration.GetSection(sectionName);
        return section.Exists() ? section.Get<T>() ?? new T() : new T();
    }

    // ---------------------------
    // Dynamic JSON Config Access
    // ---------------------------
    public ConfigSection GetDynamicConfig(string sectionName)
    {
        var section = _configuration.GetSection(sectionName);
        return section.Exists() ? ParseSection(section) : new ConfigSection();
    }

    private ConfigSection ParseSection(IConfigurationSection section)
    {
        var configSection = new ConfigSection();

        foreach (var child in section.GetChildren())
        {
            if (child.GetChildren().Any()) // Nested values
            {
                configSection.Subsections[child.Key] = ParseSection(child);
            }
            else
            {
                configSection.Values[child.Key] = ConvertValue(child.Value);
            }
        }

        return configSection;
    }

    private object ConvertValue(string value)
    {
        if (int.TryParse(value, out var intResult))
            return intResult;
        if (decimal.TryParse(value, out var decimalResult))
            return decimalResult;
        if (DateTime.TryParse(value, out var dateResult))
            return dateResult;
        if (value.StartsWith("[") && value.EndsWith("]"))
            return value.Trim('[', ']').Split(',').Select(v => ConvertValue(v.Trim())).ToArray();
        return value; // Default to string
    }
}
