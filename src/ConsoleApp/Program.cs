using System;
using System.Threading.Tasks;
using Core;

namespace ConsoleApp
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            await InversionOfControl.Container
                .GetInstance<Bot>()
                .RunAsync();
        }
    }
}
