using Npgsql;
using ShoeStore.Services;
using System;
using System.Windows;

namespace ShoeStore.Views
{
    public partial class RegisterWindow : Window
    {
        private DatabaseContext _context;

        public RegisterWindow()
        {
            InitializeComponent();
            _context = new DatabaseContext();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtStatus.Text = "";

                string fullName = txtFullName.Text.Trim();
                string login = txtLogin.Text.Trim();
                string password = txtPassword.Password;
                string confirmPassword = txtConfirmPassword.Password;

                if (string.IsNullOrEmpty(fullName))
                {
                    txtStatus.Text = "Введите ФИО";
                    return;
                }

                if (string.IsNullOrEmpty(login))
                {
                    txtStatus.Text = "Введите логин";
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    txtStatus.Text = "Введите пароль";
                    return;
                }

                if (password.Length < 6)
                {
                    txtStatus.Text = "Пароль должен быть не менее 6 символов";
                    return;
                }

                if (password != confirmPassword)
                {
                    txtStatus.Text = "Пароли не совпадают";
                    return;
                }

                using (var conn = _context.GetConnection())
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM users WHERE login = @login";
                    using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@login", login);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            txtStatus.Text = "Пользователь с таким логином уже существует";
                            return;
                        }
                    }

                    int clientRoleId = 3;

                    string insertQuery = @"
                        INSERT INTO users (full_name, login, password_hash, role_id, created_at)
                        VALUES (@fullName, @login, crypt(@password, gen_salt('bf')), @roleId, @createdAt)
                        RETURNING id";

                    using (var insertCmd = new NpgsqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@fullName", fullName);
                        insertCmd.Parameters.AddWithValue("@login", login);
                        insertCmd.Parameters.AddWithValue("@password", password);
                        insertCmd.Parameters.AddWithValue("@roleId", clientRoleId);
                        insertCmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

                        object result = insertCmd.ExecuteScalar();

                        if (result != null)
                        {
                            int newUserId = Convert.ToInt32(result);

                            if (newUserId > 0)
                            {
                                MessageBox.Show("Регистрация успешна!", "Успех",
                                              MessageBoxButton.OK, MessageBoxImage.Information);

                                LoginWindow loginWindow = new LoginWindow();
                                loginWindow.Show();
                                this.Close();
                            }
                        }
                        else
                        {
                            txtStatus.Text = "Ошибка при регистрации";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}\n\n{ex.StackTrace}", "Детали ошибки");
                txtStatus.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void LoginLink_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}