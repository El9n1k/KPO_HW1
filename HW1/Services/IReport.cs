namespace HW1.Services;

public enum ReportType { Energy, Entity, Newbie }

public interface IReport
{
    ReportType Type { get; }
    string CreateReport();
}