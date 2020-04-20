using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BluSenseWorker.BusinessLogic;
using Microsoft.Extensions.Configuration;

namespace BluSenseWorker
{
    public class Worker
    {
        private readonly IConfiguration _configuration;

        public Worker(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Execute(string name)
        {
            var path = GetFilePath(name);
            try
            {
                Console.WriteLine($"[Starting] Reading {path} at {DateTime.Now}");

                var reader = new RepFileReader(path);
                var parser = new RepFileParser(reader);
                parser.Parsing();

                var repFileLogic = new RepFileBusinessLogic(_configuration);
                repFileLogic.Save(parser.RepFiles);

                var bluboxLogic = new BluBoxBussinessLogic(_configuration);
                bluboxLogic.Save(name, parser.RepFiles.LastOrDefault());

                Console.WriteLine($"[Finished] Saved {name} at {DateTime.Now}.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"[Warning] {path} is not exited.");
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private string GetFilePath(string name)
        {
            var folder = this._configuration.GetValue<string>("FolderPath");
            return $"{folder}/{name}.csv";
        }
    }
}
