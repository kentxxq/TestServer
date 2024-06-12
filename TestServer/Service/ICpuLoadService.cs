namespace TestServer.Service;

public interface ICpuLoadService
{
    void LightLoad();
    void MediumLoad();
    void HeavyLoad();
}
