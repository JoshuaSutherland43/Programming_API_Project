using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.SqlClient;
using System.Data;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Models
{
    public class Data
    {
        // Creating a connection to the database
        private static string connectionString = "Server = labVMH8OX\\SQLEXPRESS;initial catalog = PROG; trusted_connection=true;Integrated Security = True; Encrypt=False;";
        static SqlConnection con = new SqlConnection(connectionString);
        static SqlCommand command;



        //Obtains and automatically calculates rate at which lecturer is payed.
        public double CalculateRate(int hours)
        {
            if (hours > 0 && hours <= 10)
                return 200;
            else if (hours > 10 && hours <= 20)
                return 300;
            else if (hours > 20 && hours <= 30)
                return 400;
            else if (hours > 30 && hours <= 40)
                return 500;
            else if (hours > 40 && hours <= 50)
                return 600;
            else
                return 0;
        }

        public void SignUp(string email, string password, string fName, string surname)
        {
            con.Open();
            string sql = $"INSERT INTO USERS" +
                $"(FIRST_NAME,LAST_NAME, EMAIL, PASSWORD, ROLE_ID)" +
                $" VALUES ( @fName, @surname,@email, @password, @staff)";
            command = new SqlCommand(sql, con);
            command.Parameters.AddWithValue("@fName", fName);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@staff", "1");
            command.ExecuteNonQuery();
            con.Close();
        }


        // Login method that will call and check if the login info matches the user information entered in login
        //(Rudman, G. 2024)
        public User LoginUser(string email, string password)
        {
            try
            {
                con.Open();
                // Add User_ID to the SELECT statement
                string sql = "SELECT User_ID, FIRST_NAME, LAST_NAME, EMAIL, PASSWORD, ROLE_ID FROM USERS WHERE EMAIL = @Email AND PASSWORD = @Password";
                command = new SqlCommand(sql, con);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                if (dataTable.Rows.Count == 0)
                {
                    return null; // User not found or invalid credentials
                }

                // Map the user data from the query result
                User user = new User
                {
                    UserID = Convert.ToInt32(dataTable.Rows[0]["User_ID"]),  // New line to map User_ID
                    FirstName = dataTable.Rows[0]["FIRST_NAME"].ToString(),
                    LastName = dataTable.Rows[0]["LAST_NAME"].ToString(),
                    Email = dataTable.Rows[0]["EMAIL"].ToString(),
                    Password = dataTable.Rows[0]["PASSWORD"].ToString(),
                    RoleId = dataTable.Rows[0]["ROLE_ID"].ToString()
                };

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        public string GetUserRole(int userId)
        {
            string role = "";

            con.Open();
            string sql = @" SELECT R.Name AS RoleName FROM USERS U JOIN ROLES R ON U.ROLE_ID = R.Id WHERE U.User_ID = @UserID;"; ;
            command = new SqlCommand(sql, con);
            command.Parameters.AddWithValue("@UserID", userId);
            command.ExecuteNonQuery();
            con.Close();
            return role;
        }


        // Method to upload a claim to the database
        //(Rudman, G. 2024)
        public void UploadClaim(DateTime uploadDate, int hours, double rate, string? notes, int userId, string filePath, string status)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString)) 
                {
                    con.Open();
                    string sql = @"INSERT INTO CLAIM (UPLOAD_DATE, HOURS, RATE, NOTES, User_ID, FILE_PATH, STATUS)
                           VALUES (@UploadDate, @Hours, @Rate, @Notes, @UserID, @FilePath, @Status)";

                    using (SqlCommand command = new SqlCommand(sql, con))
                    {
                        command.Parameters.AddWithValue("@UploadDate", uploadDate);
                        command.Parameters.AddWithValue("@Hours", hours);
                        command.Parameters.AddWithValue("@Rate", rate);
                        command.Parameters.AddWithValue("@Notes", notes ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@FilePath", filePath);
                        command.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception("Error inserting claim: " + ex.Message);
            }
        }


        // Method that will get Claims specifically to that users ID
        //(Rudman, G. 2024)
        public List<Claim> GetUserClaims(int userId)
        {
            List<Claim> claims = new List<Claim>();
            con.Open();
            string sql = "SELECT CLAIM_ID, UPLOAD_DATE, APPROVED_DATE, STATUS, HOURS, RATE, NOTES, User_ID, FILE_PATH FROM CLAIM WHERE User_ID = @userId";
            command = new SqlCommand(sql, con);
            command.Parameters.AddWithValue("@userId", userId);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            con.Close();

            foreach (DataRow row in dataTable.Rows)
            {
                claims.Add(new Claim
                {
                    ClaimID = Convert.ToInt32(row["CLAIM_ID"]),
                    UploadDate = Convert.ToDateTime(row["UPLOAD_DATE"]),
                    Hours = Convert.ToInt32(row["HOURS"]),
                    Rate = Convert.ToDouble(row["RATE"]),
                    Notes = row["NOTES"].ToString(),
                    UserID = Convert.ToInt32(row["USER_ID"]),
                    FilePath = row["FILE_PATH"].ToString(),
                    Status = row["STATUS"].ToString(),
                    ApprovedDate = row["APPROVED_DATE"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["APPROVED_DATE"]) : null,
                });
            }
            return claims;
        }

        // Method that will allow the admin to update the order of a user
        //(Rudman, G. 2024)
        public void UpdateClaim(int claim_id, string status)
        {
            con.Open();

            string sql = "UPDATE CLAIM SET STATUS = @updatedStatus, APPROVED_DATE = @approvedDate WHERE CLAIM_ID = @claim_id";
            command = new SqlCommand(sql, con);

            // Set parameters for the SQL command
            command.Parameters.AddWithValue("@updatedStatus", status);
            command.Parameters.AddWithValue("@approvedDate", status == "Approved" || status == "Rejected" ? (object)DateTime.Now : DBNull.Value);
            command.Parameters.AddWithValue("@claim_id", claim_id);

            command.ExecuteNonQuery();

            con.Close();
        }


        // Method that will get all Claims entered by a user through the DB
        //(Rudman, G. 2024)
        public List<Claim> GetAllClaims()
        {
            List<Claim> orders = new List<Claim>();
            con.Open();
            string sql = "SELECT * FROM CLAIM where STATUS = 'Pending';";
            command = new SqlCommand(sql, con);
            command.ExecuteNonQuery();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            con.Close();
            foreach (DataRow row in dataTable.Rows)
            {
                orders.Add(new Claim
                {
                    ClaimID = Convert.ToInt32(row["CLAIM_ID"].ToString()),
                    UploadDate = Convert.ToDateTime(row["UPLOAD_DATE"]),
                    Hours = Convert.ToInt32(row["HOURS"].ToString()),
                    Rate = Convert.ToDouble(row["RATE"].ToString()),
                    Notes = row["NOTES"].ToString(),
                    UserID = Convert.ToInt32(row["User_ID"].ToString()),
                    FilePath = row["FILE_PATH"].ToString(),
                    Status = row["STATUS"].ToString(),
                    ApprovedDate = row["APPROVED_DATE"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["APPROVED_DATE"]) : null,
                });
            }
            return orders;
        }


        // Method that gets a specifc lecturer from a certain claim Id
        //(Rudman, G. 2024)
        public string GetUserFullNameByClaim(int claimId)
        {
            con.Open();
            string sql1 = "SELECT User_ID FROM CLAIM WHERE CLAIM_ID = @claimId";
            command = new SqlCommand(sql1, con);
            command.Parameters.AddWithValue("@claimId", claimId);
            int userId = Convert.ToInt32(command.ExecuteScalar());

            string sql2 = "SELECT FIRST_NAME, LAST_NAME FROM USERS WHERE User_ID = @userId";
            command = new SqlCommand(sql2, con);
            command.Parameters.AddWithValue("@userId", userId);
            SqlDataReader reader = command.ExecuteReader();
            string fullName = string.Empty;

            if (reader.Read())
            {
                string firstName = reader["FIRST_NAME"].ToString();
                string lastName = reader["LAST_NAME"].ToString();
                fullName = $"{firstName} {lastName}";
            }
            reader.Close();
            con.Close();
            return fullName;
        }

        // Method that gets a specifc lecturer from a certain lecturer ID
        //(Rudman, G. 2024)
        public string GetLecturerFullNameFromLecturer(int lecturerId)
        {
            con.Open();
            string sql1 = "SELECT User_ID FROM HR WHERE User_ID = @lecturerId";
            command = new SqlCommand(sql1, con);
            command.Parameters.AddWithValue("@lecturerId", lecturerId);

            // Another query that gets the first name and lastname of lecturer 
            string sql2 = "SELECT FIRST_NAME, LAST_NAME FROM USERS WHERE User_ID = @lecturerId";
            command = new SqlCommand(sql2, con);
            command.Parameters.AddWithValue("@lecturerId", lecturerId);

            SqlDataReader reader = command.ExecuteReader();
            string fullName = string.Empty;

            if (reader.Read())
            {
                string firstName = reader["FIRST_NAME"].ToString();
                string lastName = reader["LAST_NAME"].ToString();
                // Saves the first name and lastname obtained into one name that will be shown in admin table
                fullName = $"{firstName} {lastName}";
            }
            reader.Close();
            con.Close();
            return fullName;

        }

        //(Rudman, G. 2024)
        // Method that calculates the certain rate depending on how many hours the lecturer has submitted.
        public double GetAmountEarned(int claimId)
        {
            double amount = 0.0;

            con.Open();

            string sql = "SELECT RATE, HOURS FROM CLAIM WHERE CLAIM_ID = @claimId";
            command = new SqlCommand(sql, con);
            command.Parameters.AddWithValue("@claimId", claimId);

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                double rate = Convert.ToDouble(reader["RATE"]);
                int hours = Convert.ToInt32(reader["HOURS"]);

                amount = rate * hours;
            }

            reader.Close();
            con.Close();

            return amount;
        }

        // Obtaining Invoice amount for HR
        public double GetTotalAmountEarnedByLecturer(int lecturerId)
        {
            double totalAmount = 0.0;

            try
            {
                con.Open();

                string sql = "SELECT RATE, HOURS FROM CLAIM WHERE User_ID = @lecturerId AND STATUS = 'Approved'";
                command = new SqlCommand(sql, con);
                command.Parameters.AddWithValue("@lecturerId", lecturerId);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    double rate = Convert.ToDouble(reader["RATE"]);
                    int hours = Convert.ToInt32(reader["HOURS"]);

                    totalAmount += rate * hours;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                con.Close();
            }

            return totalAmount;
        }


        //Inserting total amount eaned by each lecturer into HR table
        public void UploadInvoice(int lecturerId, int claimId)
        {
            try
            {
                // Calculate the total amount earned by the lecturer
                double totalAmount = GetTotalAmountEarnedByLecturer(lecturerId);

                // Open the database connection
                con.Open();

                // Insert the total amount into the HR table, including the INVOICE_DATE
                string checkSql = "SELECT COUNT(*) FROM HR WHERE User_ID = @LecturerID";
                using (SqlCommand checkCommand = new SqlCommand(checkSql, con))
                {
                    checkCommand.Parameters.AddWithValue("@LecturerID", lecturerId);
                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        // Update existing invoice
                        string updateSql = @"UPDATE HR SET CLAIM_ID = @ClaimID, TOTAL_AMOUNT = @TotalAmount, INVOICE_DATE = @InvoiceDate WHERE User_ID = @LecturerID";

                        using (SqlCommand updateCommand = new SqlCommand(updateSql, con))
                        {
                            updateCommand.Parameters.AddWithValue("@ClaimID", claimId);
                            updateCommand.Parameters.AddWithValue("@TotalAmount", totalAmount);
                            updateCommand.Parameters.AddWithValue("@InvoiceDate", DateTime.Now);
                            updateCommand.Parameters.AddWithValue("@LecturerID", lecturerId);

                            updateCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Insert new invoice
                        string insertSql = @"INSERT INTO HR (CLAIM_ID, User_ID, TOTAL_AMOUNT, INVOICE_DATE) 
                                      VALUES (@ClaimID, @LecturerID, @TotalAmount, @InvoiceDate)";

                        using (SqlCommand insertCommand = new SqlCommand(insertSql, con))
                        {
                            insertCommand.Parameters.AddWithValue("@ClaimID", claimId);
                            insertCommand.Parameters.AddWithValue("@LecturerID", lecturerId);
                            insertCommand.Parameters.AddWithValue("@TotalAmount", totalAmount);
                            insertCommand.Parameters.AddWithValue("@InvoiceDate", DateTime.Now);

                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                con.Close();
            }
        }

        //Method to get invices
        public List<Invoice> GetAllInvoices()
        {
            List<Invoice> invoices = new List<Invoice>();
            try
            {
                con.Open();
                string sql = @"
            SELECT 
                I.INVOICE_ID, I.CLAIM_ID, I.User_ID, I.TOTAL_AMOUNT, I.INVOICE_DATE,
                L.FIRST_NAME, L.LAST_NAME,
                C.HOURS, C.RATE
            FROM HR AS I
            INNER JOIN USERS AS L ON I.User_ID = L.User_ID
            INNER JOIN CLAIM AS C ON I.CLAIM_ID = C.CLAIM_ID";
                command = new SqlCommand(sql, con);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    invoices.Add(new Invoice
                    {
                        InvoiceID = Convert.ToInt32(reader["INVOICE_ID"]),
                        ClaimID = Convert.ToInt32(reader["CLAIM_ID"]),
                        UserID = Convert.ToInt32(reader["User_ID"]),

                        TotalAmount = Convert.ToDouble(reader["TOTAL_AMOUNT"]),
                        InvoiceDate = Convert.ToDateTime(reader["INVOICE_DATE"])
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                con.Close();
            }

            return invoices;
        }

        public List<Claim> GetAllClaimsFromInvoice()
        {
            List<Claim> claims = new List<Claim>();
            con.Open();
            string sql = "SELECT * FROM CLAIM WHERE STATUS IN ('Approved', 'Rejected');";
            command = new SqlCommand(sql, con);
            command.ExecuteNonQuery();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            con.Close();
            foreach (DataRow row in dataTable.Rows)
            {
                claims.Add(new Claim
                {
                    ClaimID = Convert.ToInt32(row["CLAIM_ID"].ToString()),
                    UploadDate = Convert.ToDateTime(row["UPLOAD_DATE"]),
                    Hours = Convert.ToInt32(row["HOURS"].ToString()),
                    Rate = Convert.ToDouble(row["RATE"].ToString()),
                    Notes = row["NOTES"].ToString(),
                    UserID = Convert.ToInt32(row["User_ID"].ToString()),
                    FilePath = row["FILE_PATH"].ToString(),
                    Status = row["STATUS"].ToString(),
                    ApprovedDate = row["APPROVED_DATE"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["APPROVED_DATE"]) : null,
                });
            }
            return claims;
        }

        // Method that saves all claims that have been approved or rejected into a list
        //(Rudman, G. 2024)
        public List<Claim> GetAllProcessedClaims()
        {
            List<Claim> claims = new List<Claim>();
            con.Open();
            string sql = "SELECT * FROM CLAIM WHERE STATUS IN ('Approved', 'Rejected');";
            command = new SqlCommand(sql, con);
            command.ExecuteNonQuery();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            con.Close();
            foreach (DataRow row in dataTable.Rows)
            {
                claims.Add(new Claim
                {
                    ClaimID = Convert.ToInt32(row["CLAIM_ID"].ToString()),
                    UploadDate = Convert.ToDateTime(row["UPLOAD_DATE"]),
                    Hours = Convert.ToInt32(row["HOURS"].ToString()),
                    Rate = Convert.ToDouble(row["RATE"].ToString()),
                    Notes = row["NOTES"].ToString(),
                    UserID = Convert.ToInt32(row["User_ID"].ToString()),
                    FilePath = row["FILE_PATH"].ToString(),
                    Status = row["STATUS"].ToString(),
                    ApprovedDate = row["APPROVED_DATE"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["APPROVED_DATE"]) : null,
                });
            }
            return claims;
        }


        //Method that saves and obtains all processed claims via a certain lecturer ID
        //(Rudman, G. 2024)
        public List<Claim> GetProcessedClaimsByLecturer(int lecturerId)
        {
            List<Claim> claims = new List<Claim>();
            con.Open();
            string sql = "SELECT * FROM CLAIM WHERE User_ID = @lecturerId AND STATUS IN ('Approved', 'Rejected');";
            command = new SqlCommand(sql, con);
            command.Parameters.AddWithValue("@lecturerId", lecturerId);
            command.ExecuteNonQuery();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            con.Close();
            foreach (DataRow row in dataTable.Rows)
            {
                claims.Add(new Claim
                {
                    ClaimID = Convert.ToInt32(row["CLAIM_ID"].ToString()),
                    UploadDate = Convert.ToDateTime(row["UPLOAD_DATE"]),
                    Hours = Convert.ToInt32(row["HOURS"].ToString()),
                    Rate = Convert.ToDouble(row["RATE"].ToString()),
                    Notes = row["NOTES"].ToString(),
                    UserID = Convert.ToInt32(row["User_ID"].ToString()),
                    FilePath = row["FILE_PATH"].ToString(),
                    Status = row["STATUS"].ToString(),
                    ApprovedDate = row["APPROVED_DATE"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["APPROVED_DATE"]) : null,
                });
            }
            return claims;
        }



        //Get approved claims for lecturer:
        public List<Claim> GetApprovedClaimsByLecturer(int lecturerId)
        {
            List<Claim> claims = new List<Claim>();
            con.Open();
            string sql = "SELECT * FROM CLAIM WHERE User_ID = @lecturerId AND STATUS IN ('Approved');";
            command = new SqlCommand(sql, con);
            command.Parameters.AddWithValue("@lecturerId", lecturerId);
            command.ExecuteNonQuery();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            con.Close();
            foreach (DataRow row in dataTable.Rows)
            {
                claims.Add(new Claim
                {
                    ClaimID = Convert.ToInt32(row["CLAIM_ID"].ToString()),
                    UploadDate = Convert.ToDateTime(row["UPLOAD_DATE"]),
                    Hours = Convert.ToInt32(row["HOURS"].ToString()),
                    Rate = Convert.ToDouble(row["RATE"].ToString()),
                    Notes = row["NOTES"].ToString(),
                    UserID = Convert.ToInt32(row["User_ID"].ToString()),
                    FilePath = row["FILE_PATH"].ToString(),
                    Status = row["STATUS"].ToString(),
                    ApprovedDate = row["APPROVED_DATE"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["APPROVED_DATE"]) : null,
                });
            }
            return claims;
        }

    }
}
//=====================================================
// Referencing
//=====================================================

//Rudman, G. (2024)
//BCA2 CLDV Part 2 Workshop, YouTube.
//Available at: https://www.youtube.com/watch?v=I_tiFJ-nlfE&list=LL&index=1&t=13s
//(Accessed: 18 October 2024). 
