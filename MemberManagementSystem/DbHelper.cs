using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace MemberManagementSystem
{
    class DbHelper
    {
        public const string CONNECTIONSTRING = "server=localhost;Initial Catalog=DatabaseMemberManagement;User ID=sa;Password=demol23;Trusted_Connection=true";
        public bool StoreMemberToDb(Member member)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    //Open connection
                    connection.Open();

                    //Compose query using sql parameters
                    var sqlCommand = "INSERT INTO Member (Name, Address) VALUES (@value1,@value2)";

                    //Create mysql command and pass sql query
                    using (var command = new SqlCommand(sqlCommand, connection))
                    {
                        command.Parameters.AddWithValue("@value1", member.Name);
                        command.Parameters.AddWithValue("@value2", member.Address);
                        command.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogWithInfos(ex.ToString(), "DbHelper", "StoreMemberToDb");
                return false;
            }
        }

        public bool StoreAccountToDb(Account account)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    //Open connection
                    connection.Open();

                    //Compose query using sql parameters
                    var sqlCommand = "INSERT INTO Account (AccountId, Name, Balance, Status, MemberName) VALUES (@value1,@value2,@value3,@value4,@value5)";

                    account.AccountId = account.Name + account.MemberName;
                    //Create mysql command and pass sql query
                    using (var command = new SqlCommand(sqlCommand, connection))
                    {
                        command.Parameters.AddWithValue("@value1", account.AccountId);
                        command.Parameters.AddWithValue("@value2", account.Name);
                        command.Parameters.AddWithValue("@value3", account.Balance);
                        command.Parameters.AddWithValue("@value4", account.Status);
                        command.Parameters.AddWithValue("@value5", account.MemberName);
                        command.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogWithInfos(ex.ToString(), "DbHelper", "StoreAccountToDb");
                return false;
            }
        }

        public bool CollectCoinsDb(Account account)
        {
            try
            {
                var currentBalance = 0;

                try
                {
                    currentBalance = GetBalance(account.AccountId).Result;
                }
                catch { }

                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    //Open connection
                    connection.Open();

                    int accountExist = 0;
                    var sqlCommand = "SELECT COUNT(*) FROM Account WHERE AccountId =@value1";
                    using (var command = new SqlCommand(sqlCommand, connection))
                    {
                        command.Parameters.AddWithValue("@value1", account.AccountId);
                        accountExist = (int)command.ExecuteScalar();
                    }

                    if (accountExist == 0)
                    {
                        // Account does not exist
                        return false;
                    }

                   
                    var sqlCommand1 = "Update Account SET Balance=@value1 WHERE AccountId =@value2";
                    
                    using (var command = new SqlCommand(sqlCommand1, connection))
                    {
                        command.Parameters.AddWithValue("@value1", account.Balance + currentBalance);
                        command.Parameters.AddWithValue("@value2", account.AccountId);
                        command.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogWithInfos(ex.ToString(), "DbHelper", "CollectCoinsDb");
                return false;
            }
        }

        private bool IsBalancePositiv(int currentBalance, int redeemValue)
        {
            if(currentBalance - redeemValue >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RedeemCoinsDb(Account account)
        {
            try
            {
                var currentBalance = 0;

                try
                {
                    currentBalance = GetBalance(account.AccountId).Result;
                }
                catch
                {}

                if(IsBalancePositiv(currentBalance, account.Balance) == false)
                {
                    throw new Exception("Not enouth money on account");
                }

                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    connection.Open();

                    var sqlCommand = "Update Account SET Balance=@value1 WHERE (AccountId=@value2 AND Status='active')";

                    using (var command = new SqlCommand(sqlCommand, connection))
                    {
                        command.Parameters.AddWithValue("@value1", currentBalance - account.Balance);
                        command.Parameters.AddWithValue("@value2", account.AccountId);
                        command.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogWithInfos(ex.ToString(), "DbHelper", "RedeemCoinsDb");
                return false;
            }
        }

        public async Task<int> GetBalance(string accountId)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    connection.Open();

                    using var command = new SqlCommand("SELECT Balance FROM Account WHERE AccountId=@value1;", connection);
                    command.Parameters.AddWithValue("@value1", accountId);

                    int result = 0;
                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        try
                        {
                            result = reader.GetInt32(0);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogWithInfos(ex.ToString(), "DbHelper", "GetBalance");
                throw ex;
            }
        }

        public async Task<List<Member>> GetMember(string option, string option1)
        {
            try
            {
                int value = 0;
                if (String.IsNullOrEmpty(option1) == false)
                {
                    value = Int32.Parse(option1);
                }

                string sql = "SELECT * FROM Member;";
                string part1 = "SELECT * FROM Member m JOIN[DatabaseMemberManagement].[dbo].[Account] a ON a.MemberName = m.Name";
                
                switch (option)
                {
                    case "2":
                        sql = part1 + " WHERE Status='active';";
                        break;
                    case "3":
                        sql = part1 + " WHERE Status='inactive';";
                        break;
                    case "4":
                        sql = part1 + " WHERE Status='active' AND Balance >'" +option1+"';";
                        break;
                    case "5":
                        sql = part1 + " WHERE Status='active' AND Balance <'" + option1 + "';";
                        break;
                    case "6":
                        sql = part1 + " WHERE Status='inactive' AND Balance >'" + option1 + "';";
                        break;
                    case "7":
                        sql = part1 + " WHERE Status='inactive' AND Balance <'" + option1 + "';";
                        break;
                    default:
                        sql = "SELECT * FROM Member;";
                        break;
                }

                var memberList = new List<Member>();

                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    connection.Open();

                    using var command = new SqlCommand(sql, connection);

                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        try
                        {
                            var tempMember = new Member();
                            tempMember.Name = reader.GetValue(0).ToString();
                            tempMember.Address = reader.GetValue(1).ToString();

                            var tempAccountList = new List<Account>();
                            

                            foreach(var item in await GetAccounts(tempMember.Name))
                            {
                                var tempAccount = new Account()
                                {
                                    Name = item.Name,
                                    Balance = item.Balance,
                                    Status = item.Status
                                };

                                if (option == "2" && item.Status == "active")
                                {
                                    tempAccountList.Add(tempAccount);
                                }
                                else if (option == "3" && item.Status == "inactive")
                                {
                                    tempAccountList.Add(tempAccount);
                                }
                                else if (option == "4" && item.Status == "active" && item.Balance > value )
                                {
                                    tempAccountList.Add(tempAccount);
                                }
                                else if (option == "5" && item.Status == "active" && item.Balance < value )
                                {
                                    tempAccountList.Add(tempAccount);
                                }
                                else if (option == "6" && item.Status == "inactive" && item.Balance > value)
                                {
                                    tempAccountList.Add(tempAccount);
                                }
                                else if (option == "7" && item.Status == "inactive" && item.Balance < value)
                                {
                                    tempAccountList.Add(tempAccount);
                                }
                                else if(String.IsNullOrEmpty(option) && String.IsNullOrEmpty(option1) || option == "1" && String.IsNullOrEmpty(option1))
                                {
                                    tempAccountList.Add(tempAccount);
                                }
                            }

                            tempMember.Accounts = tempAccountList;
                            memberList.Add(tempMember);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    return memberList;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogWithInfos(ex.ToString(), "DbHelper", "GetMember");
                throw ex;
            }
        }

        public async Task<List<Account>> GetAccounts(string accountName)
        {
            try
            {
                var accountList = new List<Account>();

                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    connection.Open();

                    using var command = new SqlCommand("SELECT * FROM Account WHERE MemberName=@value1;", connection);
                    command.Parameters.AddWithValue("@value1", accountName);

                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        try
                        {
                            accountList.Add(new Account()
                            {
                                Name = reader.GetValue(0).ToString(),
                                Balance = reader.GetInt32(1),
                                Status = reader.GetValue(2).ToString()
                            });
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    return accountList;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogWithInfos(ex.ToString(), "DbHelper", "GetAccounts");
                throw ex;
            }
        }

        public async Task<bool> CheckIfMembernameExist(string membername)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    connection.Open();

                    using var command = new SqlCommand("SELECT Name FROM Member WHERE Name=@value1;", connection);
                    command.Parameters.AddWithValue("@value1", membername);

                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        try
                        {
                            string temp = reader.GetValue(0).ToString();

                            if(temp == null)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogWithInfos(ex.ToString(), "DbHelper", "CheckIfMembernameExist");
                return false;
            }
        }

        public async Task<bool> CheckIfAccountNameExist(string membername)
        {
            // TODO: Muss geändert werden
            try
            {
                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    connection.Open();

                    using var command = new SqlCommand("SELECT Name FROM Account WHERE Name=@value1;", connection);
                    command.Parameters.AddWithValue("@value1", membername);

                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        try
                        {
                            string temp = reader.GetValue(0).ToString();

                            if (temp == null)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogWithInfos(ex.ToString(), "DbHelper", "CheckIfMembernameExist");
                throw ex;
            }
        }
    }
}
