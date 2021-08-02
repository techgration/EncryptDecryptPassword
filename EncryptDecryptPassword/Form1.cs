using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncryptDecryptPassword
{
    public partial class Form1 : Form
    {

        static string key { get; set; } = "M@ttK3nn3dy";


        public Form1()
        {
            InitializeComponent();
        }


        public static string Encrypt(string text)
        {
            try
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    using (var tdes = new TripleDESCryptoServiceProvider())
                    {
                        tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                        tdes.Mode = CipherMode.ECB;
                        tdes.Padding = PaddingMode.PKCS7;

                        using (var transform = tdes.CreateEncryptor())
                        {
                            byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                            byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                            return Convert.ToBase64String(bytes, 0, bytes.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error : " + ex.ToString();
            }

        }

        public static string Decrypt(string cipher)
        {
            try
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    using (var tdes = new TripleDESCryptoServiceProvider())
                    {
                        tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                        tdes.Mode = CipherMode.ECB;
                        tdes.Padding = PaddingMode.PKCS7;

                        using (var transform = tdes.CreateDecryptor())
                        {
                            byte[] cipherBytes = Convert.FromBase64String(cipher);
                            byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                            return UTF8Encoding.UTF8.GetString(bytes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }




        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Password.Text))
            {
                MessageBox.Show("Password cannot be empty");
            }
            else if (String.IsNullOrEmpty(EnvVariable.Text))
            {
                MessageBox.Show("Environment variable name cannot be empty");
            }
            else
            {
                string encryptedPassword = Encrypt(Password.Text);
                System.Environment.SetEnvironmentVariable(EnvVariable.Text, encryptedPassword, EnvironmentVariableTarget.Machine);
                MessageBox.Show("Password set for environment variable " + EnvVariable.Text);
            }


        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(EnvVariable.Text))
            {
                MessageBox.Show("Environment variable name cannot be empty");
            }
            else
            {
                string envVar = System.Environment.GetEnvironmentVariable(EnvVariable.Text, EnvironmentVariableTarget.Machine);
                string decryptedPassword = Decrypt(envVar);
                MessageBox.Show(EnvVariable.Text + " decrypted password is " + decryptedPassword);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }



    }
}
