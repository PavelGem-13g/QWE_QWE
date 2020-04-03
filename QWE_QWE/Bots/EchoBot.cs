// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Data.SQLite;
using System;
using System.IO;


namespace Microsoft.BotBuilderSamples.Bots
{
    static class Program
    {
        public static string AllOutput(SQLiteConnection connect)
        {
            string s = "NO DATA";
            SQLiteCommand comandSQL;
            comandSQL = new SQLiteCommand("SELECT * FROM BankAccounts", connect);
            SQLiteDataReader reader = comandSQL.ExecuteReader();
            while (reader.Read())
            {
                 s=$@"
ID       :   {reader["id"]}
Login    :   {reader["Login"]}
Password :   {reader["Password"]}
Money    :   {reader["Money"]}";
            }
            reader.Close();
            return s;
        }
        public static void Log(SQLiteConnection connect, ref string Login)
        {
            string Password;
            SQLiteCommand comandSQL;
            SQLiteDataReader reader;
            bool Log = false, Pas = false;
            do
            {
                try
                {
                    if (!Log)
                    {
                        Console.Write("������� Login : ");
                        Login = Console.ReadLine();
                        comandSQL = new SQLiteCommand($"SELECT (Login) FROM \"BankAccounts\"", connect);
                        reader = comandSQL.ExecuteReader();
                        while (reader.Read()) if ((string)reader["Login"] == Login) Log = true;
                        if (!Log) throw new Exception("�� ���������� ������������ � ����� Login");
                    }
                    else Console.WriteLine($"������� Login : {Login}");
                    comandSQL = new SQLiteCommand($"SELECT * FROM \"BankAccounts\" WHERE \"Login\" = \"{Login}\"", connect);
                    reader = comandSQL.ExecuteReader();
                    reader.Read();
                    Console.Write("������� Password : ");
                    Password = Console.ReadLine();
                    if (Password != (string)reader["Password"]) throw new Exception("������������ ������");
                    Pas = false;
                }
                catch (Exception Error)
                {
                    Console.WriteLine($@"������ : {Error.Message}
����������, ��������� ����");
                    Pas = true;
                    Thread.Sleep(500);
                }
            }
            while (Pas || !Log);
            ;
        }
        public static string[] InfInput(SQLiteConnection connect)
        {
            string[] information;
            try
            {
                Console.Clear();
                Console.WriteLine(@"������� �����, ������ � ��������� ���� �������
������ : Deezbec MyBirthday 2000");
                information = Console.ReadLine().Split();
                if (information.Length != 3) throw new Exception("���� ������� �� 3 ��������");
                if (!Int64.TryParse(information[2], out long x)) throw new Exception("���� �� ����� �������� �� ����");
                SQLiteCommand comandSQL = new SQLiteCommand($"SELECT (\"Login\") FROM \"BankAccounts\"", connect);
                SQLiteDataReader reader = comandSQL.ExecuteReader();
                while (reader.Read()) if ((string)reader["Login"] == information[0]) throw new Exception("����� ������������ ��� ����������");
            }
            catch (Exception Error)
            {
                Console.WriteLine($@"������ : {Error.Message}
����������, ��������� ����");
                Thread.Sleep(500);
                information = InfInput(connect);
            }
            return information;
        }
        public static void IfNoBD(string directory, SQLiteConnection connect)
        {
            int n = 0;
            bool action = false;
            //!
            SQLiteConnection.CreateFile($"{directory}");
            string[] information;
            do
            {
                Console.Clear();
                Console.WriteLine($@"� ���������, � ���� {directory} �� ���� ������� ��
So ��� ����� ������� �����");
                Console.WriteLine("������� ���������� �������� �����");
                try { if (!Int32.TryParse(Console.ReadLine(), out n) || n <= 0) throw new Exception("����������� ������� ���������� ��������"); action = false; }
                catch (Exception Error) { Console.WriteLine($"������ : {Error.Message} \n��������� ����"); action = true; }
            }
            while (action);
            connect.Open();
            SQLiteCommand comandSQL = new SQLiteCommand("CREATE TABLE IF NOT EXISTS \"BankAccounts\"" + "(\"id\" INTEGER PRIMARY KEY AUTOINCREMENT, \"Login\" TEXT, \"Password\" TEXT, \"Money\" INTEGER);", connect);
            for (int i = 0; i < n; i++)
            {
                comandSQL.ExecuteNonQuery();
                information = InfInput(connect);
                comandSQL = new SQLiteCommand($"INSERT INTO \"BankAccounts\" (\"Login\", \"Password\", \"Money\") " + $"VALUES (\"{information[0]}\", \"{information[1]}\", {Convert.ToInt64(information[2])})", connect);
                Console.Clear();
            }
            comandSQL.ExecuteNonQuery();
        }
        public static void Actions(SQLiteConnection connect, string Login)
        {
            int action = 0;
            bool act;
            SQLiteCommand comandSQL = new SQLiteCommand($"SELECT (\"Money\") FROM \"BankAccounts\" WHERE \"Login\" = \"{Login}\"", connect);
            SQLiteDataReader reader = comandSQL.ExecuteReader();
            reader.Read();
            long money = (long)reader["Money"];
            do
            {
                Console.Clear();
                Console.WriteLine($@"����� ����������, {Login}
��� ������ : {money}
��� �������� ����� �������� : 
1 - ������� �����");
                try
                {
                    if (!Int32.TryParse(Console.ReadLine(), out action) || (action != 1)) throw new Exception("������������ ���� ��������");
                    act = false;
                }
                catch (Exception Error)
                {
                    Console.WriteLine($@"������ : {Error.Message}
����������, ��������� ����");
                    act = true;
                    Thread.Sleep(500);
                }
            }
            while (act);
            switch (action)
            {
                case 1: Translation(connect, Login, money); break;
            }
        }
        public static void Translation(SQLiteConnection connect, string Login, long money)
        {
            SQLiteCommand comandSQL;
            SQLiteDataReader reader;
            bool act = false;
            long summ = 0, id = 0;
            do
            {
                Console.Clear();
                Console.Write("������� id ������� : ");
                try
                {
                    if (!Int64.TryParse(Console.ReadLine(), out id)) throw new Exception("�������� id");
                    comandSQL = new SQLiteCommand("SELECT (\"id\") FROM \"BankAccounts\"", connect);
                    reader = comandSQL.ExecuteReader();
                    while (reader.Read()) if (id == (long)reader["id"]) { act = false; break; } else act = true;
                    reader.Close();
                    comandSQL.ExecuteNonQuery();
                    comandSQL = new SQLiteCommand($"SELECT (\"id\") FROM \"BankAccounts\" WHERE (\"Login\") = \"{Login}\"", connect);
                    reader = comandSQL.ExecuteReader(); reader.Read();
                    if (id == (long)reader["id"]) { reader.Close(); throw new Exception("�������� id, �� �� ������ ��������� ������ ������ ����"); }
                    reader.Close();
                    if (act) throw new Exception("�������� id");
                }
                catch (Exception Error)
                {
                    Console.WriteLine($@"������ : {Error.Message}
����������, ��������� ����");
                    act = true;
                    Thread.Sleep(500);
                }
            }
            while (act);
            do
            {
                Console.Clear();
                Console.WriteLine($"������� id ������� : {id}");
                Console.Write("������� ����� ��� �������� (�������� 1%) : ");
                try
                {
                    if (!long.TryParse(Console.ReadLine(), out summ) || summ < 0) throw new Exception("����������� �����");
                    comandSQL = new SQLiteCommand($"SELECT (\"Money\") FROM \"BankAccounts\" WHERE \"Login\" = \"{Login}\"", connect);
                    reader = comandSQL.ExecuteReader(); reader.Read();
                    if (summ > (long)reader["Money"]) { reader.Close(); throw new Exception("� ��� ������������ �������"); }
                    act = false;

                }
                catch (Exception Error)
                {
                    Console.WriteLine($@"������ : {Error.Message}
����������, ��������� ����");
                    act = true;
                    Thread.Sleep(500);
                }
            }
            while (act);
            comandSQL = new SQLiteCommand($"UPDATE \"BankAccounts\" set \"Money\" = {money - summ} WHERE \"Login\" = \"{Login}\"", connect);
            comandSQL.ExecuteNonQuery();
            comandSQL = new SQLiteCommand($"SELECT (Money) FROM \"BankAccounts\" WHERE \"id\" = {id}", connect);
            reader = comandSQL.ExecuteReader(); reader.Read(); money = (long)reader["Money"];
            comandSQL = new SQLiteCommand($"UPDATE \"BankAccounts\" set \"Money\" = {Math.Round(summ + money - summ * 0.01F)} WHERE \"id\" = {id}", connect);
            comandSQL.ExecuteNonQuery();
            
        }
       
    }
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = $"Hello:  {turnContext.Activity.Text}";
            await turnContext.SendActivityAsync(replyText);

            //Program program;
            if (turnContext.Activity.Text=="Start")
            {
string directory = Path.Combine(Environment.CurrentDirectory, "Test.db"), Login = ""; //���������� ��� ���������
            //!
            SQLiteConnection connect = new SQLiteConnection($"Data Source = {directory}; Version = 3");
            if (!File.Exists(directory)) Program.IfNoBD(directory, connect);
            else connect.Open();
            //Program.Log(connect, ref Login);

            string Password;
            SQLiteCommand comandSQL;
            SQLiteDataReader reader;
            bool Log = false, Pas = false;
            do
            {
                try
                {
                    if (!Log)
                    {
                        await turnContext.SendActivityAsync("������� Login : ");
                        Login = Console.ReadLine();
                        comandSQL = new SQLiteCommand($"SELECT (Login) FROM \"BankAccounts\"", connect);
                        reader = comandSQL.ExecuteReader();
                        while (reader.Read()) if ((string)reader["Login"] == Login) Log = true;
                        if (!Log) throw new Exception("�� ���������� ������������ � ����� Login");
                    }
                    else Console.WriteLine($"������� Login : {Login}");
                    comandSQL = new SQLiteCommand($"SELECT * FROM \"BankAccounts\" WHERE \"Login\" = \"{Login}\"", connect);
                    reader = comandSQL.ExecuteReader();
                    reader.Read();
                    Console.Write("������� Password : ");
                    Password = Console.ReadLine();
                    if (Password != (string)reader["Password"]) throw new Exception("������������ ������");
                    Pas = false;
                }
                catch (Exception Error)
                {
                    await turnContext.SendActivityAsync($"Error : {Error.Message} Please retype");
                    Pas = true;
                    Thread.Sleep(500);
                }
            }
            while (Pas || !Log);















            while (true)
            {
                //Program.Actions(connect, Login);

                int action = 0;
                bool act;
                comandSQL = new SQLiteCommand($"SELECT (\"Money\") FROM \"BankAccounts\" WHERE \"Login\" = \"{Login}\"", connect);
                reader = comandSQL.ExecuteReader();
                reader.Read();
                long money = (long)reader["Money"];
                do
                {
                    await turnContext.SendActivityAsync($@"����� ����������, {Login}
��� ������ : {money}
��� �������� ����� �������� : 
1 - ������� �����");
                    try
                    {
                        if (!Int32.TryParse(turnContext.Activity.Text, out action) || (action != 1)) throw new Exception("������������ ���� ��������");
                        act = false;
                    }
                    catch (Exception Error)
                    {
                        await turnContext.SendActivityAsync($@"������ : {Error.Message}
����������, ��������� ����");
                        act = true;
                        Thread.Sleep(500);
                    }
                }
                while (act);
                switch (action)
                {
                    case 1:
                        act = false;
                        long summ = 0, id = 0;
                        do
                        {
                            await turnContext.SendActivityAsync("������� id ������� : ");
                            try
                            {
                                if (!Int64.TryParse(turnContext.Activity.Text, out id)) throw new Exception("�������� id");
                                comandSQL = new SQLiteCommand("SELECT (\"id\") FROM \"BankAccounts\"", connect);
                                reader = comandSQL.ExecuteReader();
                                while (reader.Read()) if (id == (long)reader["id"]) { act = false; break; } else act = true;
                                reader.Close();
                                comandSQL.ExecuteNonQuery();
                                comandSQL = new SQLiteCommand($"SELECT (\"id\") FROM \"BankAccounts\" WHERE (\"Login\") = \"{Login}\"", connect);
                                reader = comandSQL.ExecuteReader(); reader.Read();
                                if (id == (long)reader["id"]) { reader.Close(); throw new Exception("�������� id, �� �� ������ ��������� ������ ������ ����"); }
                                reader.Close();
                                if (act) throw new Exception("�������� id");
                            }
                            catch (Exception Error)
                            {
                                await turnContext.SendActivityAsync($@"������ : {Error.Message}
����������, ��������� ����");
                                act = true;
                                Thread.Sleep(500);
                            }
                        }
                        while (act);
                        do
                        {
                            await turnContext.SendActivityAsync($"������� id ������� : {id}");
                            await turnContext.SendActivityAsync("������� ����� ��� �������� (�������� 1%) : ");
                            try
                            {
                                if (!long.TryParse(Console.ReadLine(), out summ) || summ < 0) throw new Exception("����������� �����");
                                comandSQL = new SQLiteCommand($"SELECT (\"Money\") FROM \"BankAccounts\" WHERE \"Login\" = \"{Login}\"", connect);
                                reader = comandSQL.ExecuteReader(); reader.Read();
                                if (summ > (long)reader["Money"]) { reader.Close(); throw new Exception("� ��� ������������ �������"); }
                                act = false;

                            }
                            catch (Exception Error)
                            {
                                await turnContext.SendActivityAsync($@"������ : {Error.Message}
����������, ��������� ����");
                                act = true;
                                Thread.Sleep(500);
                            }
                        }
                        while (act);
                        comandSQL = new SQLiteCommand($"UPDATE \"BankAccounts\" set \"Money\" = {money - summ} WHERE \"Login\" = \"{Login}\"", connect);
                        comandSQL.ExecuteNonQuery();
                        comandSQL = new SQLiteCommand($"SELECT (Money) FROM \"BankAccounts\" WHERE \"id\" = {id}", connect);
                        reader = comandSQL.ExecuteReader(); reader.Read(); money = (long)reader["Money"];
                        comandSQL = new SQLiteCommand($"UPDATE \"BankAccounts\" set \"Money\" = {Math.Round(summ + money - summ * 0.01F)} WHERE \"id\" = {id}", connect);
                        comandSQL.ExecuteNonQuery();
                            ; break;
                }









                //Program.AllOutput(connect);
                //Console.Write("\n��? ");
                await turnContext.SendActivityAsync("Close program?");
                if (turnContext.Activity.Text.ToLower().Replace("l", "�").Replace("f", "�") == "��")
                    break;
            }
            connect.Close();
            }
            

        }
    }
}
