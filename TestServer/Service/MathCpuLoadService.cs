namespace TestServer.Service;

public class MathCpuLoadService : ICpuLoadService
{
    public void LightLoad()
    {
        // 轻负载：简单的循环
        CustomLoad(Convert.ToInt32(Math.Pow(10,6)));
    }

    public void MediumLoad()
    {
        // 中等负载：更复杂的计算
        CustomLoad(Convert.ToInt32(Math.Pow(10,7)));
    }

    public void HeavyLoad()
    {
        // 重负载：大量计算
        CustomLoad(Convert.ToInt32(Math.Pow(10,8)));
    }

    public void CustomLoad(int count)
    {
        for (var i = 0; i < count; i++)
        {
            Math.Sqrt(i);
        }
    }
}
