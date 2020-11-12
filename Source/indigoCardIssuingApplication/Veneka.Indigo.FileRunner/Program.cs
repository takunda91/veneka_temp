using System;
using Common.Logging;

namespace Veneka.Indigo.FileRunner
{
    internal class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private static void Main(string[] args)
        {
            try
            {
                log.Info("Indigo File Loader Started.");
                int selectedOption = 0;

                //Display user menu if args is null
                if (args == null || args.Length <= 0)
                {
                    log.Info("DO NOT CLOSE THIS WINDOW");
                    while (true)
                    {                        
                        selectedOption = MenuAction();
                        ProcessOption(selectedOption);
                    }
                }
                else //Process the args
                {
                    foreach(var arg in args)
                    {
                        selectedOption = 0;

                        if (int.TryParse(arg, out selectedOption))
                            ProcessOption(selectedOption);
                        else
                            log.WarnFormat("Argument {0} is invalid.", arg);
                    }
                }

                log.Info("Indigo File Loader terminating.");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                if (args == null || args.Length <= 0)
                    Console.ReadKey();
            }
        }

        private static void ProcessOption(int selectedOption)
        {
            log.InfoFormat("Processing option {0}", selectedOption);
            Processor processor = new Processor();

            try
            {
                switch (selectedOption)
                {
                    case 1:
                        processor.RunFileProcessor();
                        break;
                    case 2:
                        processor.RunFileGenerator();
                        break;
                    case 3:
                        processor.RunBulkFileProcesser();
                        break;
                    case 9999:
                        Environment.Exit(0);
                        break;
                    default: break;
                }
            }
            catch(Exception ex)
            {
                log.Warn(ex.Message);
            }

            log.InfoFormat("Processing option {0} done.", selectedOption);
        }

        private static int MenuAction()
        {
            int selectedOption;
            do
            {
                Console.WriteLine("Please select option to run:");
                Console.WriteLine("1 - Load File/s");
                Console.WriteLine("2 - Create File/s for CMS");
                Console.WriteLine("3 - Bulk Requests");
                Console.WriteLine("x - Exit");

                var userOption = Console.ReadLine();

                if (int.TryParse(userOption, out selectedOption))
                {
                    if (selectedOption < 1 || selectedOption > 3)
                        selectedOption = 0;
                }
                else if (userOption.ToUpper().Trim().Equals("X"))
                    selectedOption = 9999;

                if (selectedOption == 0)
                {
                    Console.WriteLine("Invalid Option Selected, press any key to continue.");
                    Console.ReadKey();
                }

            }
            while (selectedOption == 0);

            return selectedOption;
        }
    }
}