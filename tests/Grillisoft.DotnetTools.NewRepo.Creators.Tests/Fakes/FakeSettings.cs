using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Tests.Fakes;

public class FakeSettings : INewRepoSettings
{
    private static string DefaultRootPath => OperatingSystem.IsWindows() ? @"C:\repo" : "/repo";

    private readonly Dictionary<string, object> _values = new(StringComparer.OrdinalIgnoreCase);

    public IDirectoryInfo Root { get; }
    public IFileInfo InitFile { get; }

    public FakeSettings(IFileSystem fileSystem, string rootPath = null)
    {
        rootPath ??= DefaultRootPath;
        fileSystem.Directory.CreateDirectory(rootPath);
        Root = fileSystem.DirectoryInfo.New(rootPath);
        InitFile = fileSystem.FileInfo.New(fileSystem.Path.Combine(rootPath, "init.yml"));
    }

    public FakeSettings With(string key, object value)
    {
        _values[key] = value;
        return this;
    }

    public string GetString(ConfigurationKey key)
    {
        if (_values.TryGetValue(key.Key, out var v))
            return v?.ToString() ?? string.Empty;
        return key.DefaultValue?.ToString() ?? string.Empty;
    }

    public bool GetBool(ConfigurationKey key)
    {
        if (_values.TryGetValue(key.Key, out var v))
            return v is bool b ? b : bool.Parse(v?.ToString() ?? "false");
        return key.DefaultValue is bool def && def;
    }

    public int GetInt32(ConfigurationKey key)
    {
        if (_values.TryGetValue(key.Key, out var v))
            return v is int i ? i : int.Parse(v?.ToString() ?? "0");
        return key.DefaultValue is int def ? def : 0;
    }

    public T Get<T>(ConfigurationKey key)
    {
        if (_values.TryGetValue(key.Key, out var v))
            return (T)v;
        return key.DefaultValue is T def ? def : default;
    }

    public bool TryGet<T>(ConfigurationKey key, out T value)
    {
        if (_values.TryGetValue(key.Key, out var v))
        {
            value = (T)v;
            return true;
        }
        value = default;
        return false;
    }

    public Task Load(ILogger logger, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task Init(ILogger logger, CancellationToken cancellationToken) => Task.CompletedTask;
}
