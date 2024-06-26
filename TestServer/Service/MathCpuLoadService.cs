namespace TestServer.Service;

public class MathCpuLoadService : ICpuLoadService
{
    public void LightLoad()
    {
        // 轻负载：简单的循环
        CustomLoad(1000);
    }

    public void MediumLoad()
    {
        // 中等负载：更复杂的计算
        CustomLoad(10000);
    }

    public void HeavyLoad()
    {
        // 重负载：大量计算
        CustomLoad(100000);
    }

    public void CustomLoad(int count)
    {
        for (var i = 0; i < count; i++)
        {
            Math.Sqrt(i);
        }
    }
}
