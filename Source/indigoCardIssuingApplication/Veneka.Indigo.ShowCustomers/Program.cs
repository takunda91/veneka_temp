using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration;

namespace Veneka.Indigo.ShowCustomers
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
                    foreach (var arg in args)
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
            IntegrationController integration = new IntegrationController();

            try
            {
                switch (selectedOption)
                {
                    case 1:
                        integration.RunFileProcessor();
                        break;
                    case 2:
                        integration.RunFileRecreate();
                        break;
                    case 9999:
                        Environment.Exit(0);
                        break;
                    default: break;
                }
            }
            catch (NotImplementedException nie)
            {
                log.Warn("Operation not supported.");
            }

            log.InfoFormat("Processing option {0} done.", selectedOption);
        }

        private static int MenuAction()
        {
            int selectedOption;
            do
            {
                Console.WriteLine("Please select option to run:");
                Console.WriteLine("1 - Get Customer Listing");
              //  Console.WriteLine("2 - Recreate 3D Secure File/s");                
                Console.WriteLine("x - Exit");


                var userOption = Console.ReadLine();

                if (int.TryParse(userOption, out selectedOption))
                {
                    if (selectedOption < 1 || selectedOption > 1)
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





        //private static void Main(string[] args)
        //{
        //    log.Info("3D Secure File Runner - START");

        //    try
        //    {
        //        IntegrationController integration = new IntegrationController();
        //        integration.RunFileProcessor();
        //    }
        //    catch(Exception ex)
        //    {
        //        log.Error(ex);
        //    }

        //    Console.ReadLine();

        //    log.Info("3D Secure File Runner - END");
        //}
    }
}
