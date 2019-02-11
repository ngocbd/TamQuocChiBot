using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTamQuocChi
{
    public class Bot
    {
        DataSet Config = new DataSet();
        int accountIndex = 0;
        public Bot()
        {
            Config.ReadXml("Config.xml");
            Login();

        }
        public void RestartApp()
        {
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();

            //force-stop tamquocchi app
            AdbClient.Instance.ExecuteRemoteCommand("am force-stop com.vgm.tqqat", device, receiver);
            Console.WriteLine("The device responded:");
            Console.WriteLine(receiver.ToString());
            System.Threading.Thread.Sleep(1000);
            
            
            AdbClient.Instance.ExecuteRemoteCommand("monkey -p com.vgm.tqqat -c android.intent.category.LAUNCHER 1", device, receiver);

            System.Threading.Thread.Sleep(10000);


        }
        public void GoToMaps()
        {
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();
            AdbClient.Instance.ExecuteRemoteCommand("input tap 100 1200", device, receiver);
            Thread.Sleep(5000);
        }
        public void SelectMining()
        {
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();
            if (Utils.CompareAt(device, "FirstMining.png", new Rectangle(142, 300, 83, 38)))
            {
                AdbClient.Instance.ExecuteRemoteCommand("input tap 185 319", device, receiver);
                Thread.Sleep(3000);
            }
        }
        
        public void CreateTeam()
        {
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();
            AdbClient.Instance.ExecuteRemoteCommand("input tap 225 1200", device, receiver);
            Thread.Sleep(3000);
            AdbClient.Instance.ExecuteRemoteCommand("input tap 500 1200", device, receiver);
            Thread.Sleep(3000);
        }
        public void Mining()
        {
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();
            if (Utils.CompareAt(device, "SecondMining.png", new Rectangle(437, 749, 53, 41)))
            {
                
                
                AdbClient.Instance.ExecuteRemoteCommand("input tap 471 783", device, receiver);
                Thread.Sleep(3000);
                CreateTeam();
                Console.WriteLine("Team Created!");
            }
        }
        public void EscapeLeaveWindow()
        {
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();
            AdbClient.Instance.ExecuteRemoteCommand("input keyevent KEYCODE_BACK", device, receiver);
            Thread.Sleep(5000);
            
            if (!Utils.CompareAt(device, "LeaveConfirm.png",new Rectangle (402, 769,215,52) ))
            {
                AdbClient.Instance.ExecuteRemoteCommand("input keyevent KEYCODE_BACK", device, receiver);
                Thread.Sleep(5000);

            } 
        }
            public void Login()
        {

            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();
            if (Utils.CompareAt(device, "firstLoginButton.png", new Rectangle(268, 1153, 46, 46)))
            {
                Console.WriteLine("In FirstLogin Screen");
                AdbClient.Instance.ExecuteRemoteCommand("input tap 350 1156", device, receiver);
                

            }
            else
            {
                Console.WriteLine("NotIn FirstLogin Screen");
                // 645 120 50 50
                if (Utils.CompareAt(device, "changeAccountButton.png", new Rectangle(645, 120, 50, 50)))
                {
                    Console.WriteLine("In enter game screen");

                    AdbClient.Instance.ExecuteRemoteCommand("input tap 645 120", device, receiver);

                    Console.WriteLine("Click change account");

                }
                else
                {
                    Console.WriteLine("in unknown screen !!!!!");
                    RestartApp();
                    Login();
                    return;

                }

               
            }

            AdbClient.Instance.ExecuteRemoteCommand("input tap 300 500", device, receiver);
            Utils.ClearText();
            var username = Config.Tables["account"].Rows[accountIndex]["username"].ToString().Trim();
            var password = Config.Tables["account"].Rows[accountIndex]["password"].ToString().Trim();

            Console.WriteLine("Username:"+ username);
            AdbClient.Instance.ExecuteRemoteCommand("input text \""+ username + "\"", device, receiver);
          

            AdbClient.Instance.ExecuteRemoteCommand("input tap 300 560", device, receiver);
            Utils.ClearText();
            Thread.Sleep(1000);
            Console.WriteLine("Password:" + password);
            AdbClient.Instance.ExecuteRemoteCommand("input text "+ password, device, receiver);
            AdbClient.Instance.ExecuteRemoteCommand("input tap 356 650", device, receiver);
            // 215 1130 290 96 
            Thread.Sleep(1000);
            if (Utils.CompareAt(device, "enterGameButton.png", new Rectangle(215, 1130, 290, 96)))
            {
                Console.WriteLine("In EnterGame  Screen");
                AdbClient.Instance.ExecuteRemoteCommand("input tap 360 1175", device, receiver);


            }
            Console.WriteLine("Entered game...");
            Thread.Sleep(7000);
            EscapeLeaveWindow();
            Console.WriteLine("Escape Leaving Window.");
            GoToMaps();
            Console.WriteLine("Go to maps...");
            SelectMining();
            Console.WriteLine("Selected Mining.");
            //while (1==1) {
            Mining();
            Console.WriteLine("Mining...");
            //}
        }
    }
}
