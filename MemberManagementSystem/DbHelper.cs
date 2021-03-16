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
                // Log Exception
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
                    var sqlCommand = "INSERT INTO Account (Name, Balance, Status, MemberName) VALUES (@value1,@value2,@value3,@value4)";

                    //Create mysql command and pass sql query
                    using (var command = new SqlCommand(sqlCommand, connection))
                    {
                        command.Parameters.AddWithValue("@value1", account.Name);
                        command.Parameters.AddWithValue("@value2", account.Balance);
                        command.Parameters.AddWithValue("@value3", account.Status);
                        command.Parameters.AddWithValue("@value4", account.MemberName);
                        command.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log Exception
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
                    currentBalance = GetBalance(account.Name).Result;
                }
                catch (Exception ex)
                {

                }

                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    //Open connection
                    connection.Open();

                    int accountExist = 0;
                    var sqlCommand = "SELECT COUNT(*) FROM Account WHERE Name =@value1";
                    using (var command = new SqlCommand(sqlCommand, connection))
                    {
                        command.Parameters.AddWithValue("@value1", account.Name);
                        accountExist = (int)command.ExecuteScalar();
                    }

                    if (accountExist == 0)
                    {
                        // Account does not exist
                        return false;
                    }

                   
                    var sqlCommand1 = "Update Account SET Balance=@value1 WHERE Name =@value2";
                    
                    using (var command = new SqlCommand(sqlCommand1, connection))
                    {
                        command.Parameters.AddWithValue("@value1", account.Balance + currentBalance);
                        command.Parameters.AddWithValue("@value2", account.Name);
                        command.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log Exception
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
                    currentBalance = GetBalance(account.Name).Result;
                }
                catch(Exception ex)
                {
                }

                if(IsBalancePositiv(currentBalance, account.Balance) == false)
                {
                    throw new Exception("Not enouth money on account");
                }

                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    connection.Open();

                    var sqlCommand = "Update Account SET Balance=@value1 WHERE (Name=@value2 AND Status='active')";

                    using (var command = new SqlCommand(sqlCommand, connection))
                    {
                        command.Parameters.AddWithValue("@value1", currentBalance - account.Balance);
                        command.Parameters.AddWithValue("@value2", account.Name);
                        command.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log Exception
                return false;
            }
        }

        public async Task<int> GetBalance(string membername)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    connection.Open();

                    using var command = new SqlCommand("SELECT Balance FROM Account WHERE Name=@value1;", connection);
                    command.Parameters.AddWithValue("@value1", membername);

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
                throw ex;
            }
        }

        public async Task<List<Member>> GetMember()
        {
            try
            {
                var memberList = new List<Member>();

                using (var connection = new SqlConnection(CONNECTIONSTRING))
                {
                    connection.Open();

                    using var command = new SqlCommand("SELECT * FROM Member;", connection);

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
                                tempAccountList.Add(new Account()
                                {
                                    Name = item.Name,
                                    Balance = item.Balance,
                                    Status = item.Status
                                });
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
                // Log Exception
                return false;
            }
        }

        public async Task<bool> CheckIfAccountNameExist(string membername)
        {
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
                throw ex;
            }
        }
    }
}
