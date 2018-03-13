namespace tstseri
{
    using System;
    using Serilog;
    using Microsoft.Extensions.DependencyInjection;  
    using Microsoft.Extensions.Logging;

    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("tstseri.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(logbuilder => logbuilder.AddSerilog(dispose: true))
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IBarService, BarService>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Starting application");

            //do the actual work here
            var bar = serviceProvider.GetService<IBarService>();
            bar.DoSomeRealWork();

            logger.LogDebug("All done!");
        }
    }

    public interface IFooService  
    {
        void DoThing(int number);
    }

    public interface IBarService  
    {
        void DoSomeRealWork();
    }

    public class BarService : IBarService  
    {
        private readonly IFooService _fooService;
        public BarService(IFooService fooService)
        {
            _fooService = fooService;
        }

        public void DoSomeRealWork()
        {
            for (int i = 0; i < 10; i++)
            {
                _fooService.DoThing(i);
            }
        }
    }

    public class FooService : IFooService  
    {
        private readonly ILogger<FooService> _logger;
        public FooService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FooService>();
        }

        public void DoThing(int number)
        {
            _logger.LogInformation($"Doing the thing {number}");
        }
    }
}
