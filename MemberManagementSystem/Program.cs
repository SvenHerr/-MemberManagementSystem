using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MemberManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the \"Member Management System\" this is the solution of Sven Herrman");

            while (true)
            {
                Console.WriteLine("Please choose one of the following option:");
                Console.WriteLine("Enter anytime exit to close application");
                Console.WriteLine("1: Create Member");
                Console.WriteLine("2: Create Account");
                Console.WriteLine("3: Collect Coints");
                Console.WriteLine("4: Redeem Coints");
                //Console.WriteLine("5: Import Json");
                Console.WriteLine("5: Export Json");
                Console.WriteLine("6: Dump Logfile");

                Console.WriteLine("Please select an option by type the number and hit enter" + Environment.NewLine);

                var programmNr = 0;
                var terminar = false;

                while (terminar == false)
                {
                    try
                    {
                        programmNr = Int32.Parse(CheckIfExit(Console.ReadLine()));

                        if (programmNr >= 1 && programmNr <= 6)
                        {
                            terminar = true;
                            break;
                        }
                    }
                    catch{}

                    Console.WriteLine("Not a number. Please enter a number between 1 and 6" + Environment.NewLine);
                    terminar = false;
                }

                SelectProgramm(programmNr);
            }
        }

        public static void SelectProgramm(int programmNr)
        {
            switch (programmNr)
            {
                case 1:
                    CreateMember();
                    break;
                case 2:
                    CreateAccount();
                    break;
                case 3:
                    CollectCoints();
                    break;
                case 4:
                    RedeemCoints();
                    break;
                //case 5:
                //    ImportJson();
                //    break;
                case 5:
                    Export();
                    break;
                case 6:
                    DumpLogFile();
                    break;
            }
        }

        public static void DumpLogFile()
        {
            try
            {
                Console.WriteLine("Please enter path:(leave empty for default path)");
                var path = CheckIfExit(Console.ReadLine());

                Console.WriteLine("Please enter logname:(leave empty for default name)");
                var name = CheckIfExit(Console.ReadLine());

                LogHelper.DumpLog(path, name);
            }
            catch(Exception ex)
            {
                LogHelper.WriteToLogWithInfos(ex.ToString(), "Program", "DumpLogFile");
            }
            
        }
        public static string CheckIfExit(string value)
        {
            if (value.Contains("exit") || value.Contains("Exit"))
            {
                Console.WriteLine("Programm wird beendet");

                Environment.Exit(0);

                return null;
            }
            else
            {
                return value;
            }
        }

        public static void CreateMember()
        {
            var user1 = new Member();

            Console.WriteLine("Please enter Name:");
            Console.WriteLine("Example: Max Mustermann");
            user1.Name = CheckIfExit(Console.ReadLine());


            DbHelper dbHelper = new DbHelper();

            var nameExist = dbHelper.CheckIfMembernameExist(user1.Name).Result;

            while (nameExist)
            {
                Console.WriteLine("Membername already exist!:");
                Console.WriteLine("Please enter Name:");
                Console.WriteLine("Example: Max Mustermann");
                user1.Name = CheckIfExit(Console.ReadLine());

                nameExist = dbHelper.CheckIfMembernameExist(user1.Name).Result;
            }


            Console.WriteLine(System.Environment.NewLine + "Bitte Adresse eingeben:");
            Console.WriteLine("Syntax-Beispiel: Musterstraße 17. 97072 Würzburg");
            user1.Address = CheckIfExit(Console.ReadLine());


            if (dbHelper.StoreMemberToDb(user1))
            {
                Console.WriteLine("Member " + user1.Name + " successfully stored" + Environment.NewLine );
            }
            else
            {
                Console.WriteLine("Error on save member " + user1.Name + " please try again" + Environment.NewLine);
            }
        }

        public static void CreateAccount()
        {
            var account = new Account();

            Console.WriteLine(Environment.NewLine + "Please enter an Membername:");
            Console.WriteLine("Example: Sven Herrmann");
            account.MemberName = CheckIfExit(Console.ReadLine());
            //Usually do some checks example:  if member Exist before user enters account name etc.
            

            Console.WriteLine("Please enter an accountname:");
            Console.WriteLine("Example: Burger King");
            account.Name = CheckIfExit(Console.ReadLine());

            DbHelper dbHelper = new DbHelper();
            var accountExistes = dbHelper.CheckIfAccountNameExist(account.Name).Result;


            while (accountExistes)
            {
                Console.WriteLine("Accountname already exist!:");
                Console.WriteLine("Please enter an accountname:");
                Console.WriteLine("Example: Burger King");
                account.Name = CheckIfExit(Console.ReadLine());

                accountExistes = dbHelper.CheckIfAccountNameExist(account.Name).Result;
            }

            Console.WriteLine("Please enter start balance:");
            Console.WriteLine("Example: 0 (without comma)");

            try
            {
                var test = Int32.Parse(CheckIfExit(Console.ReadLine()));
                account.Balance = test;
            }
            catch
            {
                account.Balance = 0;
            }

            Console.WriteLine("Please enter Status: a  for activ or i for inactive");
            Console.WriteLine("Example: a oder i");


            if (CheckIfExit(Console.ReadLine()) == "a")
            {
                account.Status = "active";
            }
            else if (Console.ReadLine() == "i")
            {
                account.Status = "inactive";
            }
            else
            {
                Console.WriteLine("Eingabe nicht erkannt. Der Account wird auf inaktiv gesetzt.");
                Console.WriteLine("Couldn't read status. Status is now incactive.");
                account.Status = "inactive";
            }

            if (dbHelper.StoreAccountToDb(account))
            {
                Console.WriteLine("Account " + account.Name + " successfully stored" + Environment.NewLine);
            }
            else
            {
                Console.WriteLine("Error on save account " + account.Name + " please try again" + Environment.NewLine);
            }
        }

        public static void CollectCoints()
        {
            Console.WriteLine(Environment.NewLine + "Please enter Accountname:");
            Console.WriteLine("Example: Burger King");

            var accountName = CheckIfExit(Console.ReadLine());

            Console.WriteLine("Please enter number of coints:");
            Console.WriteLine("Example: 1 or 2 or 3 etc.");

            var valueCoints = 0;
            try
            {
                valueCoints = Int32.Parse(CheckIfExit(Console.ReadLine()));

                while (valueCoints <= 0)
                {
                    Console.WriteLine("Value must be positive and greater 0. Please try again");
                    valueCoints = Int32.Parse(CheckIfExit(Console.ReadLine()));
                }
            }
            catch
            {
            }

            var temp = new Account()
            {
                Name = accountName,
                Balance = valueCoints
            };

            if (accountName != null)
            {
                DbHelper dbHelper = new DbHelper();

                if (dbHelper.CollectCoinsDb(temp))
                {
                    Console.WriteLine("Balance of " + valueCoints + " added to account " + accountName + Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Error: " + valueCoints + " cant be added to account " + accountName + Environment.NewLine);
                }
            }
        }

        public static void RedeemCoints()
        {
            Console.WriteLine(Environment.NewLine + "Please enter Accountname:");
            Console.WriteLine("Example: Burger King");

            var accountName = CheckIfExit(Console.ReadLine());

            Console.WriteLine("Please enter value of coints:");
            Console.WriteLine("Example: 1 oder 2 oder 3 usw.");

            var valueCoints = 0;
            try
            {
                valueCoints = Int32.Parse(CheckIfExit(Console.ReadLine()));

                while (valueCoints <= 0)
                {
                    Console.WriteLine("Value must be positive and greater 0. Please try again");
                    valueCoints = Int32.Parse(CheckIfExit(Console.ReadLine()));
                }
            }
            catch
            {
            }

            var temp = new Account()
            {
                Name = accountName,
                Balance = valueCoints
            };

            if (accountName != null)
            {
                DbHelper dbHelper = new DbHelper();

                if (dbHelper.RedeemCoinsDb(temp))
                {
                    Console.WriteLine("Balance of " + valueCoints + " redeem to account " + accountName + Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Not enough balance on the account " + accountName + Environment.NewLine);
                }
            }
        }

        //public static void ImportJson()
        //{
        //    Console.WriteLine(Environment.NewLine + "Bitte Pfad angeben:");

        //}

        public static void Export()
        {
            Console.WriteLine(Environment.NewLine + "Please enter path with filename:");
            Console.WriteLine("Example: C:\\Users\\Sven\\Desktop\\myJson.json");
            var path = CheckIfExit(Console.ReadLine());

            DbHelper dbHelper = new DbHelper();
            var memberList =  dbHelper.GetMember()?.Result ?? new List<Member>();

            //using (StreamWriter file = File.CreateText(@"C:\Users\Sven\Desktop\myJson.json"))
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, memberList);
            }

            Console.WriteLine("export created in " + path + Environment.NewLine);

        }
    }
}
