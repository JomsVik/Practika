using Npgsql;
using ShoeStore.Models;
using System;

namespace ShoeStore.Services
{
    public class AuthService
    {
        private DatabaseContext _context;
        public User CurrentUser { get; private set; }

        public AuthService()
        {
            _context = new DatabaseContext();
        }

        public User Login(string login, string password)
        {
            try
            {
                using (var conn = _context.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT u.id, u.full_name, u.login, u.role_id, u.created_at,
                               r.id, r.name
                        FROM users u
                        JOIN roles r ON u.role_id = r.id
                        WHERE u.login = @login 
                        AND u.password_hash = crypt(@password, u.password_hash)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CurrentUser = new User
                                {
                                    Id = reader.GetInt32(0),
                                    FullName = reader.GetString(1),
                                    Login = reader.GetString(2),
                                    RoleId = reader.GetInt32(3),
                                    CreatedAt = reader.GetDateTime(4),
                                    Role = new Role
                                    {
                                        Id = reader.GetInt32(5),
                                        Name = reader.GetString(6)
                                    }
                                };
                                return CurrentUser;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка входа: {ex.Message}");
                return null;
            }
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public bool IsAdmin => CurrentUser?.Role?.Name == "Администратор";
        public bool IsManager => CurrentUser?.Role?.Name == "Менеджер" || IsAdmin;
        public bool IsClient => CurrentUser?.Role?.Name == "Авторизированный клиент";
        public bool IsGuest => CurrentUser == null;


        public string GetRoleName()
        {
            if (CurrentUser == null) return "Гость";
            return CurrentUser.Role.Name;
        }
    }
}