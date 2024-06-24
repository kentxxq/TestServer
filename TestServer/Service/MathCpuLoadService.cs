namespace TestServer.Service;

public class MathCpuLoadService : ICpuLoadService
{
    public void LightLoad()
    {
        // 轻负载：简单的循环
        for (var i = 0; i < 1000; i++)
        {
            Math.Sqrt(i);
        }
    }

    public void MediumLoad()
    {
        // 中等负载：更复杂的计算
        for (var i = 0; i < 10000; i++)
        {
            Math.Sqrt(i);
        }
    }

    public void HeavyLoad()
    {
        // 重负载：大量计算
        for (var i = 0; i < 100000; i++)
        {
            Math.Sqrt(i);
        }
    }
}
