namespace BetterClip.Service.Explorer;

public interface IExplorerService
{
    IEnumerable<string> GetRecords();
    void MonitorOn();
    void MonitorOff();
}
