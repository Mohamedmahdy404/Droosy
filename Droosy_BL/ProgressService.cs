using System.Text.Json;
using Droosy_DAL.Data;
using Droosy_DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Droosy_BL;

public class ProgressService(ApplicationDbContext db)
{
    public const double CompletionRatio = .9;
    public static List<double[]> MergeRanges(IEnumerable<double[]> ranges, double duration)
    {
        var merged = new List<double[]>();
        foreach (var range in ranges.Where(x => x.Length == 2 && double.IsFinite(x[0]) && double.IsFinite(x[1]) && x[1] > x[0])
                     .Select(x => new[] { Math.Clamp(x[0], 0, duration), Math.Clamp(x[1], 0, duration) }).Where(x => x[1] > x[0]).OrderBy(x => x[0]))
        {
            if (merged.Count == 0 || range[0] > merged[^1][1]) merged.Add(range);
            else merged[^1][1] = Math.Max(merged[^1][1], range[1]);
        }
        return merged;
    }
    public static double WatchedSeconds(LessonProgress? progress) => progress == null ? 0 :
        (JsonSerializer.Deserialize<List<double[]>>(progress.WatchedRangesJson) ?? []).Sum(x => x[1] - x[0]);

    public async Task<LessonProgress> OpenAsync(string studentId, string lessonId, bool complete = false)
    {
        var row = await FindOrCreateAsync(studentId, lessonId);
        row.LastOpenedAtUtc = DateTime.UtcNow;
        if (complete && !row.IsCompleted) { row.IsCompleted = true; row.CompletedAtUtc = DateTime.UtcNow; }
        await db.SaveChangesAsync();
        return row;
    }
    public async Task<LessonProgress> SaveVideoAsync(string studentId, string lessonId, double position, List<double[]> played)
    {
        var row = await FindOrCreateAsync(studentId, lessonId);
        ApplyVideo(row, position, played, DateTime.UtcNow);
        await db.SaveChangesAsync();
        return row;
    }
    public static void ApplyVideo(LessonProgress row, double position, List<double[]> played, DateTime now)
    {
        var duration = Droosy_DAL.Seeding.DemoLessonContent.VideoDurationSeconds;
        var previous = JsonSerializer.Deserialize<List<double[]>>(row.WatchedRangesJson) ?? [];
        var merged = MergeRanges(previous.Concat(played), duration);
        row.WatchedRangesJson = JsonSerializer.Serialize(merged);
        row.LastPositionSeconds = Math.Clamp(position, 0, duration);
        row.LastOpenedAtUtc = now;
        if (!row.IsCompleted && merged.Sum(x => x[1] - x[0]) >= duration * CompletionRatio)
        { row.IsCompleted = true; row.CompletedAtUtc = now; }
    }
    private async Task<LessonProgress> FindOrCreateAsync(string studentId, string lessonId)
    {
        var row = await db.LessonProgresses.SingleOrDefaultAsync(x => x.StudentId == studentId && x.LessonId == lessonId);
        if (row != null) return row;
        row = new LessonProgress { Id = studentId + "-" + lessonId, StudentId = studentId, LessonId = lessonId };
        db.LessonProgresses.Add(row);
        return row;
    }
}
