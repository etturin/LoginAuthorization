using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginAuthorization.Data
{
    public class UserRepository
    {
        private string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                "VALUES (@name, @email, @hash)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@hash", BCrypt.Net.BCrypt.HashPassword(password));

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return isValid ? user : null;

        }

        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Email = (string)reader["Email"],
                Name = (string)reader["Name"],
                Password = (string)reader["PasswordHash"]
            };
        }

        public List<Ad> GetAllAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "select a.id, a.LoginId,a.Description,a.PhoneNumber,l.Name,l.Email,a.Date from Ads a join Users l on a.LoginId = l.Id";
            List<Ad> adds = new List<Ad>();
            connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                adds.Add(new Ad
                {
                    Name = (string)reader["Name"],
                    Description = (string)reader["Description"],
                    Date = (DateTime)reader["Date"],
                    LoginId = (int)reader["LoginId"],
                    Number = (string)reader["PhoneNumber"],
                    Email = (string)reader["Email"],
                    id=(int)reader["id"]
                });
            }
            return adds;

        }
        public void DeleteAdd(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "Delete from adds where id=@id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void NewAdd(Ad ad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Adds VALUES (@PhoneNumber, @Description, @LoginId, GetDate())";
            cmd.Parameters.AddWithValue("@PhoneNumber", ad.Number);
            cmd.Parameters.AddWithValue("@Description", ad.Description);
            cmd.Parameters.AddWithValue("@LoginId", ad.LoginId);
            connection.Open();
            cmd.ExecuteNonQuery();

        }
        public List<Ad> GetAllAds(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "select a.id, a.LoginId,a.Description,a.PhoneNumber,l.Name,l.Email,a.Date from Adds a join Logins l on a.LoginId = l.Id where LoginId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            List<Ad> ads = new List<Ad>();
            connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Name = (string)reader["Name"],
                    Description = (string)reader["Description"],
                    Date = (DateTime)reader["Date"],
                    LoginId = (int)reader["LoginId"],
                    Number = (string)reader["PhoneNumber"],
                    Email = (string)reader["Email"],
                    id=(int)reader["id"]
                });
            }
            return ads;
        }


    }
}

