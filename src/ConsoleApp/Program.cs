using System.Threading.Tasks;
using Core;

namespace ConsoleApp
{
    class Program
    {
        private static async Task Main()
        {
            await InversionOfControl.Container
                .GetInstance<Bot>()
                .RunAsync();

            await Task.Delay(-1);
        }
    }
}
