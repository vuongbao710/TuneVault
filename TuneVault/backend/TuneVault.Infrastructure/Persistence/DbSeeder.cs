using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuneVault.Domain.Entities;
using TuneVault.Domain.Enums;
using TuneVault.Infrastructure.Identity;
using TuneVault.Infrastructure.Persistence;

namespace TuneVault.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TuneVaultDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TuneVaultDbContext>>();

        await context.Database.EnsureCreatedAsync();

        if (await context.MediaItems.AnyAsync())
        {
            return;
        }

        logger.LogInformation("Seeding database...");

        var alice = await CreateUserAsync(userManager, "alice@demo.com", "Demo@123", "Alice Nguyen");
        var bob = await CreateUserAsync(userManager, "bob@demo.com", "Demo@123", "Bob Tran");

        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var mediaRoot = Path.Combine(env.ContentRootPath, "wwwroot", "media");
        Directory.CreateDirectory(mediaRoot);

        var mediaItems = new List<MediaItem>
        {
            CreateMedia("Neon Dreams", "Luna Wave", "Pop", MediaType.Audio, alice.Id, "seed-01.wav", 210),
            CreateMedia("Midnight Drive", "City Lights", "Synthwave", MediaType.Audio, alice.Id, "seed-02.wav", 185),
            CreateMedia("Ocean Breeze", "Calm Collective", "Lo-fi", MediaType.Audio, bob.Id, "seed-03.wav", 240),
            CreateMedia("Starfall", "Nova", "Electronic", MediaType.Audio, bob.Id, "seed-04.wav", 198),
            CreateMedia("Golden Hour", "Sunset Club", "Indie", MediaType.Audio, alice.Id, "seed-05.wav", 225),
            CreateMedia("Rainy Cafe", "Soft Keys", "Jazz", MediaType.Audio, bob.Id, "seed-06.wav", 260),
            CreateMedia("Mountain Echo", "Trail Sound", "Ambient", MediaType.Audio, alice.Id, "seed-07.wav", 300),
            CreateMedia("Pulse Runner", "Beat Forge", "EDM", MediaType.Audio, bob.Id, "seed-08.wav", 190),
            CreateMedia("City Timelapse", "Visual Lab", "Documentary", MediaType.Video, alice.Id, "seed-09.mp4", 120),
            CreateMedia("Studio Session", "Live Room", "Performance", MediaType.Video, bob.Id, "seed-10.webm", 150)
        };

        foreach (var item in mediaItems)
        {
            var fullPath = Path.Combine(mediaRoot, Path.GetFileName(item.FilePath));
            await CreatePlaceholderFileAsync(fullPath, item.Type);
            context.MediaItems.Add(item);
        }

        var playlist1 = new Playlist
        {
            Id = Guid.NewGuid(),
            Name = "Alice Chill Mix",
            Description = "Relaxing tracks curated by Alice",
            IsPublic = true,
            OwnerId = alice.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };

        var playlist2 = new Playlist
        {
            Id = Guid.NewGuid(),
            Name = "Bob Video Picks",
            Description = "Favorite videos from Bob",
            IsPublic = true,
            OwnerId = bob.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        context.Playlists.AddRange(playlist1, playlist2);
        await context.SaveChangesAsync();

        context.PlaylistTracks.AddRange(
            new PlaylistTrack { PlaylistId = playlist1.Id, MediaItemId = mediaItems[0].Id, Order = 0 },
            new PlaylistTrack { PlaylistId = playlist1.Id, MediaItemId = mediaItems[1].Id, Order = 1 },
            new PlaylistTrack { PlaylistId = playlist1.Id, MediaItemId = mediaItems[4].Id, Order = 2 },
            new PlaylistTrack { PlaylistId = playlist2.Id, MediaItemId = mediaItems[8].Id, Order = 0 },
            new PlaylistTrack { PlaylistId = playlist2.Id, MediaItemId = mediaItems[9].Id, Order = 1 }
        );

        await context.SaveChangesAsync();
        logger.LogInformation("Database seeded successfully.");
    }

    private static async Task<ApplicationUser> CreateUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string displayName)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName,
            Bio = $"Demo account for {displayName}"
        };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        return user;
    }

    private static MediaItem CreateMedia(string title, string artist, string genre, MediaType type, string ownerId, string fileName, int duration)
    {
        var ext = Path.GetExtension(fileName);
        return new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Artist = artist,
            Genre = genre,
            Type = type,
            OwnerId = ownerId,
            FilePath = Path.Combine("media", fileName).Replace('\\', '/'),
            ContentType = ext switch
            {
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".mp4" => "video/mp4",
                ".webm" => "video/webm",
                _ => "application/octet-stream"
            },
            DurationSeconds = duration,
            Description = $"{title} by {artist}",
            CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 10))
        };
    }

    private static async Task CreatePlaceholderFileAsync(string fullPath, MediaType type)
    {
        if (File.Exists(fullPath)) return;

        if (type == MediaType.Audio)
        {
            await File.WriteAllBytesAsync(fullPath, CreateMinimalWavBytes(1));
        }
        else
        {
            await File.WriteAllTextAsync(fullPath, "TuneVault placeholder video file for demo streaming.");
        }
    }

    private static byte[] CreateMinimalWavBytes(int seconds)
    {
        const int sampleRate = 8000;
        const short channels = 1;
        const short bits = 8;
        var dataSize = sampleRate * seconds * channels * bits / 8;
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(36 + dataSize);
        writer.Write(Encoding.ASCII.GetBytes("WAVE"));
        writer.Write(Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);
        writer.Write(channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * bits / 8);
        writer.Write((short)(channels * bits / 8));
        writer.Write(bits);
        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(dataSize);
        writer.Write(new byte[dataSize]);
        return ms.ToArray();
    }
}
