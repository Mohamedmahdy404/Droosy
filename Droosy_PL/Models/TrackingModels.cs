using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Droosy_DAL.Entities;

namespace Droosy_PL.Models;

public class AttendanceInput
{
    public string StudentId { get; set; } = "";
    public string StudentName { get; set; } = "";
    public AttendanceStatus? Status { get; set; }
    [StringLength(300)] public string? Note { get; set; } = "";
}
public class AttendanceEditorModel
{
    public string SessionId { get; set; } = "";
    [ValidateNever] public ClassSession Session { get; set; } = null!;
    public List<AttendanceInput> Entries { get; set; } = [];
    public List<AuditLog> History { get; set; } = [];
}
public class VideoProgressInput
{
    public double Position { get; set; }
    public List<double[]> Played { get; set; } = [];
}
public static class TrackingDisplay
{
    public static string Status(AttendanceStatus? status) => status switch
    {
        AttendanceStatus.Present => "حاضر", AttendanceStatus.Absent => "غائب",
        AttendanceStatus.Late => "متأخر", AttendanceStatus.Excused => "معتذر", _ => "لم يسجل"
    };
    public static string Tone(AttendanceStatus? status) => status switch
    {
        AttendanceStatus.Absent => "danger", AttendanceStatus.Late => "amber", AttendanceStatus.Excused => "neutral", _ => ""
    };
    public static string Progress(LessonProgress? progress) => progress == null ? "لم يبدأ" : progress.IsCompleted ? "مكتمل" : "بدأ التعلم";
}

public class AttendanceSnapshot
{
    public string StudentId { get; set; } = "";
    public AttendanceStatus? Status { get; set; }
    public string Note { get; set; } = "";
}
