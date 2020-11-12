using EProcess.Indigo._2._0._0.Migration.Common.Reports;
using EProcess.Indigo._2._0._0.Migration.Common.Utilities;
using OriginalIndigoDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EProcess.Indigo._2._0._0.Migration
{
    class Program
    {
        #region Porperties

        private static NewIndigoDAL.NewIndigoRepo _NewIndigoRepo;
        private static OriginalIndigoDAL.OriginalIndigoRepo _OriginalIndigoRepo;

        public static NewIndigoDAL.NewIndigoRepo NewIndigoRepo
        {
            get
            {
                if (_NewIndigoRepo == null)
                    _NewIndigoRepo = new NewIndigoDAL.NewIndigoRepo();

                return _NewIndigoRepo;
            }
            set { _NewIndigoRepo = value; }
        }
        public static OriginalIndigoDAL.OriginalIndigoRepo OriginalIndigoRepo
        {
            get
            {
                if (_OriginalIndigoRepo == null)
                    _OriginalIndigoRepo = new OriginalIndigoDAL.OriginalIndigoRepo();

                return _OriginalIndigoRepo;
            }
            set { _OriginalIndigoRepo = value; }
        }

        #endregion

        static void Main(string[] args)
        {
            #region AppDomainInitializer Seed flex_response_values validation
		    /*
            try
            {
                if (NewIndigoRepo.DoesNewDatabaseExists())
                {
                    var errorMsg = OriginalIndigoRepo.CompareSeedValues();
                    if (!string.IsNullOrEmpty(errorMsg))
                        throw new Exception(errorMsg);

                }// end if (NewIndigoRepo.DoesNewDatabaseExists())

            }// end try
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("\nError - {0}", ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("\nInner Error - {0}", ex.InnerException.Message);

                Console.WriteLine("\nStack Trace - {0}\n\n", ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.Gray;
                ErrorLogger.LogError(ex);

                goto shutdown;
            }// end catch (Exception ex) 
             */
	        #endregion

            var mainMenuStr = BuildMainMenu();

            Console.WriteLine(mainMenuStr);
            Console.Write("Please select an action or enter 'x' to quit: ");
            var input = Console.ReadLine();

            while (!input.ToLower().Equals("x"))
            {
                try
                {
                    var isDone = false;
                    var msg = string.Empty;

                    switch (input)
                    {
                        case "0":
                            if (!NewIndigoRepo.DoesNewDatabaseExists())
                            {
                                Console.WriteLine("Please enter Master Key password for DB encryption:");
                                string masterKey = Console.ReadLine();
                                isDone = NewIndigoRepo.SetupDatabase(masterKey);

                                if (isDone)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("\nDatabase generate script has been created.");
                                    Console.ForegroundColor = ConsoleColor.Gray;

                                    //msg = string.Format("Please locate it here: '{0}'and run it before continuing.\n\n",
                                    //    (FilesUtils.dirPath + "Output/Database/"));

                                    //Console.WriteLine(msg);
                                    //ErrorLogger.LogInfo("Database generate script has been created, and can be found here: "
                                    //    + Environment.NewLine + (FilesUtils.dirPath + "Output/Database/"));

                                    Console.WriteLine("Press enter to continue.");
                                    Console.ReadLine();

                                    //if (NewIndigoRepo.DoesNewDatabaseExists())
                                    //{
                                    //    msg = string.Format("Please wait while '{0}' is being prepared...", NewIndigoRepo.DatabaseName);
                                    //    Console.WriteLine(msg);
                                    //    ErrorLogger.LogInfo(msg);


                                    //    isDone = NewIndigoRepo.SetupDatabase(masterKey);

                                    //    if (isDone)
                                    //        isDone = OriginalIndigoRepo.LogCurrentSeeds();
                                    //}// end if (NewIndigoRepo.DoesNewDatabaseExists())
                                    //else
                                    //    throw new Exception(string.Format("Cannot continue with the selected process as '{0}' does not exists!", NewIndigoRepo.DatabaseName));
                                }// end if (isDone)
                            }// end if (!NewIndigoRepo.DoesNewDatabaseExists())
                            else
                                throw new Exception(string.Format("Cannot create '{0}' as it already exists!", NewIndigoRepo.DatabaseName));

                            break;
                        case "1":
                            if (NewIndigoRepo.DoesNewDatabaseExists())
                            {
                                var errorMsg = OriginalIndigoRepo.CompareSeedValues();
                                if (!string.IsNullOrEmpty(errorMsg))
                                    throw new Exception(errorMsg);

                                msg = "Please wait while backing up the databases...";

                                Console.WriteLine("\n{0}", msg);
                                ErrorLogger.LogInfo(msg);

                                if (OriginalIndigoRepo.BackupDatabase())
                                {
                                    if (NewIndigoRepo.BackupDatabase())
                                    {
                                        msg = "Database bakcups complete...";

                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("{0}\n\n", msg);
                                        ErrorLogger.LogInfo(msg);
                                        Console.ForegroundColor = ConsoleColor.Gray;

                                        var selectedIssuer = BuildSelectIssuerMenu();

                                        if (selectedIssuer != null)
                                        {
                                            msg = string.Format("Issuer: {0} ({1}) is being migrated", selectedIssuer.issuer_name, selectedIssuer.issuer_code);

                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.WriteLine(msg);
                                            ErrorLogger.LogInfo(msg);
                                            Console.ForegroundColor = ConsoleColor.Gray;

                                            //isDone = NewIndigoRepo.MigrateIssuer(selectedIssuer.issuer_id);
                                            isDone = ReportsService.GenerateReport(OriginalIndigoRepo.OriginalDataContext, selectedIssuer, false);
                                            
                                            if (isDone)
                                                isDone = OriginalIndigoRepo.MigrateIssuer(selectedIssuer.issuer_id);

                                            if (isDone)
                                                isDone = ReportsService.GenerateReport(NewIndigoRepo.NewIndigoDataModel, selectedIssuer, true);


                                        }// end if (selectedIssuer != null)
                                        else
                                            throw new Exception(string.Format("Issuer not found"));

                                    }// end if (NewIndigoRepo.BackupDatabase())
                                    else
                                        throw new Exception(string.Format("Could not backup '{0}'", NewIndigoRepo.DatabaseName));
                                }// if (OriginalIndigoRepo.BackupDatabase())
                                else
                                    throw new Exception(string.Format("Could not backup '{0}'", OriginalIndigoRepo.DatabaseName));

                            }// end if (NewIndigoRepo.DoesNewDatabaseExists())
                            else
                                throw new Exception(string.Format("Cannot continue with the selected process as '{0}' does not exists!", NewIndigoRepo.DatabaseName));

                            break;
                        case "2":
                            if (NewIndigoRepo.DoesNewDatabaseExists())
                            {
                                Console.WriteLine("\nPlease wait while backing up the databases...");

                                if (OriginalIndigoRepo.BackupDatabase())
                                {
                                    if (NewIndigoRepo.BackupDatabase())
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("Database bakcups complete...\n\n");
                                        Console.ForegroundColor = ConsoleColor.Gray;

                                        msg = "Please wait while updating the seed values...";

                                        Console.WriteLine("\n{0}", msg);
                                        ErrorLogger.LogInfo(msg);

                                        isDone = OriginalIndigoRepo.LogCurrentSeeds();

                                        msg = "Updating seed values complete...";

                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("{0}\n\n", msg);
                                        Console.ForegroundColor = ConsoleColor.Gray;


                                    }// end if (NewIndigoRepo.BackupDatabase())
                                    else
                                        throw new Exception(string.Format("Could not backup '{0}'", NewIndigoRepo.DatabaseName));
                                }// if (OriginalIndigoRepo.BackupDatabase())
                                else
                                    throw new Exception(string.Format("Could not backup '{0}'", OriginalIndigoRepo.DatabaseName));

                            }// end if (NewIndigoRepo.DoesNewDatabaseExists())
                            else
                                throw new Exception(string.Format("Cannot continue with the selected process - Database backup as '{0}' does not exists!", NewIndigoRepo.DatabaseName));

                            break;
                        default:
                            msg = "Error - Invalid selection. Please try again";
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n{0}", msg);
                            ErrorLogger.LogError(msg);
                            Console.ForegroundColor = ConsoleColor.Gray;

                            break;
                    }// end switch (input)

                    if (isDone)
                    {
                        msg = "Operation complete! :)";

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n{0}\n", msg);
                        ErrorLogger.LogInfo(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }// end if (isDone)
                    else
                    {
                        msg = "Operation could not be completed :{";
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n{0}\n", msg);
                        ErrorLogger.LogError(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }


                }// end try
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("\nError - {0}", ex.Message);
                    if (ex.InnerException != null)
                        Console.WriteLine("\nInner Error - {0}", ex.InnerException.Message);

                    Console.WriteLine("\nStack Trace - {0}\n\n", ex.StackTrace);

                    Console.ForegroundColor = ConsoleColor.Gray;

                    ErrorLogger.LogError(ex);
                }// end catch (Exception ex)



                mainMenuStr = BuildMainMenu();
                Console.WriteLine(mainMenuStr);
                Console.Write("Please select an action or enter 'x' to quit: ");

                input = Console.ReadLine();
            }// end while (!input.ToLower().Equals("x"))

            shutdown:
            Console.WriteLine("\n\nApplication is shutting down...\nPress any key to close.");
            ErrorLogger.LogInfo("Application is shutting down");
            Console.ReadLine();
        }

        /// <summary>
        /// Builds the issuers list menu for migrations
        /// </summary>
        /// <returns></returns>
        private static issuer BuildSelectIssuerMenu()
        {
            issuer selectIssuer = null;

            var migratedIssuers = NewIndigoRepo.GetIssuersIDs();
            var issuersForMigration = OriginalIndigoRepo.GetOtherIssuers(migratedIssuers);

            var menu = "\n\n                          Issuers List\n" +
                           "                    ______________________\n\n";

            for (int i = 0; i < issuersForMigration.Count; i++)
            {
                var menuItem = string.Format("{0}   {1:25} ({2})\n",
                    i + 1, issuersForMigration[i].issuer_name, issuersForMigration[i].issuer_code);
                menu += menuItem;
            }// end for (int i = 0; i < issuersForMigration.Count; i++)

            Console.WriteLine(menu);

        prompt:
            Console.Write("\nPlease select an Issuer to migrate: ");
            var input = Console.ReadLine();
            try
            {
                int selectedIndex;
                if (int.TryParse(input, out selectedIndex))
                    selectIssuer = issuersForMigration[(selectedIndex - 1)];
            }
            catch
            {

            }// end catch

            if (selectIssuer == null)
            {
                var msg = "Invalid selection. Please try again. :{";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n{0}\n", msg);
                ErrorLogger.LogError(msg);
                Console.ForegroundColor = ConsoleColor.Gray;
                goto prompt;
            }

            return selectIssuer;
        }// end method static issuer BuildSelectIssuerMenu()

        /// <summary>
        /// Builds the main menu string to be presented to the user
        /// </summary>
        /// <returns></returns>
        public static string BuildMainMenu()
        {
            var mainMenuStr = string.Empty;

            mainMenuStr =
            "****************************************************************************\n" +
            "*                                  MAIN MENU:                              *\n" +
            "*                          =========================                       *\n" +
            "*                                                                          *\n";

            if (!NewIndigoRepo.DoesNewDatabaseExists())
            {
                mainMenuStr +=
            "* 0. Configure new Database and do all required house-keeping tasks        *\n" +
            "*                                                                          *\n" +
            "****************************************************************************\n";

            }// end if (!NewIndigoRepo.DoesNewDatabaseExists())
            else
            {
                mainMenuStr +=
                    //"* 1. Backup Databases                                                      *\n" +
                "* 1. Migrate an Issuer                                                     *\n" +
                "* 2. Update Seed values                                                    *\n" +
                "*                                                                          *\n" +
                "****************************************************************************\n";
            }// end else

            return mainMenuStr;
        }// end method static string BuildMainMenu()

    }
}
