namespace TestServer.Service;

public interface ICpuLoadService
{
    void LightLoad();
    void MediumLoad();
    void HeavyLoad();
    /// <summary>
    /// 这里使用int,限制一下最大数量
    /// </summary>
    /// <param name="count"></param>
    void CustomLoad(int count);
}
