using Tesseract;
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

        TesseractEngine engine = new TesseractEngine(@"E:\Downloads\tessdata-3.04.00","vie");

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
        public bool SelectMining()
        {
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();

            // 193 310 
            if (Utils.CompareAt(device, "OpenMiningButton.png", new Rectangle(193, 310, 17, 21),86))
            {
                AdbClient.Instance.ExecuteRemoteCommand("input tap 185 319", device, receiver);
                Thread.Sleep(300);
                return true;
            }
            return false;
        }
        
        public void CreateTeam()
        {
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();
            AdbClient.Instance.ExecuteRemoteCommand("input tap 225 1200", device, receiver);
            Thread.Sleep(400);
            AdbClient.Instance.ExecuteRemoteCommand("input tap 500 1200", device, receiver);
            Thread.Sleep(1000);
        }
        public void Mining(int tried=0)
        {
            if (tried > 5) return;
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();
            //if (Utils.CompareAt(device, "LeaveConfirm.png", new Rectangle(402, 769, 215, 52)))
            //{
            //    AdbClient.Instance.ExecuteRemoteCommand("input keyevent KEYCODE_BACK", device, receiver);
            //    return;

            //}
            // 509 689 
            if (Utils.CompareAt(device, "stealButton.png", new Rectangle(509, 689, 55, 44)))
            {
                if (SelectMining())
                {
                    Mining(++tried);
                }
                
                return;
            }
            
            if (Utils.CompareAt(device, "SecondMining.png", new Rectangle(437, 749, 53, 41)))
            {


                AdbClient.Instance.ExecuteRemoteCommand("input tap 471 783", device, receiver);
                Thread.Sleep(2000);
                CreateTeam();
                Console.WriteLine("Team Created!");
                Thread.Sleep(1000);
            }
            else
            {
                if (SelectMining())
                {
                    Mining(++tried);
                }

                return;
            }
        }
        public void EscapeLeaveWindow()
        {
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();
            AdbClient.Instance.ExecuteRemoteCommand("input keyevent KEYCODE_BACK", device, receiver);
            Thread.Sleep(1000);
            AdbClient.Instance.ExecuteRemoteCommand("input keyevent KEYCODE_BACK", device, receiver);
            Thread.Sleep(1000);
            if (Utils.CompareAt(device, "LeaveConfirm.png",new Rectangle (402, 769,215,52) ))
            {
                AdbClient.Instance.ExecuteRemoteCommand("input keyevent KEYCODE_BACK", device, receiver);
                Thread.Sleep(1000);

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
            Console.WriteLine("Go to maps...");
            GoToMaps();
            Console.WriteLine("Selected Mining.");
            if (SelectMining())
            {

                Mining();
                Console.WriteLine("Mining...");
            }
                

            try
            {
                // 165 85
                Image tmp = Utils.CropAt(device, new Rectangle(165, 85, 100, 30));
                string numberOFLand = "";
                engine.SetVariable("tessedit_char_whitelist", "0123456789|/");


                Page page = engine.Process(new Bitmap(tmp));
                numberOFLand = page.GetText();
                page.Dispose();

                Console.WriteLine("numberOFLand:" + numberOFLand);

                string[] pair = numberOFLand.Split(new Char[] { '\\', '|', '/' });

                int have = Utils.GetNumbers(pair[0]);
                int total = Utils.GetNumbers(pair[1]);

                Console.WriteLine("have:" + have + "/ total:" + total);

                if (total > have)
                {
                    //// 53 1033
                    //AdbClient.Instance.ExecuteRemoteCommand("input tap 53 1033", device, receiver);
                    //Console.ReadKey();
                    //// 30 862
                    //Thread.Sleep(200);
                    //AdbClient.Instance.ExecuteRemoteCommand("input tap 30 862", device, receiver);
                    //Console.ReadKey();

                    //Thread.Sleep(200);
                    //AdbClient.Instance.ExecuteRemoteCommand("input tap 538 1205", device, receiver);
                    //Console.ReadKey();

                    // 180 255 54 28

                    if (Utils.CompareAt(device, "XuatChinhButton.png", new Rectangle(180, 255, 54, 28)))
                    {
                        Console.WriteLine("XuatChinhButton avaiable !!!");
                        AdbClient.Instance.ExecuteRemoteCommand("input tap 180 255", device, receiver);
                        Thread.Sleep(400);
                        AdbClient.Instance.ExecuteRemoteCommand("input tap 412 522", device, receiver);
                        Thread.Sleep(400);
                        AdbClient.Instance.ExecuteRemoteCommand("input tap 538 1205", device, receiver);
                        Thread.Sleep(400);
                        if (Utils.CompareAt(device, "StartAtk.png", new Rectangle(444, 745, 40, 45)))
                        {

                            Console.WriteLine("Can atk !->");
                            AdbClient.Instance.ExecuteRemoteCommand("input tap 460 778", device, receiver);
                            Thread.Sleep(400);
                            this.CreateTeam();
                            Thread.Sleep(400);

                        }
                    }

                        
                   
                }
                else
                {
                    // 644 912 38 28
                    Console.WriteLine("Full land check Have Khau or not ");
                    if (Utils.CompareAt(device, "HaveKhauButton.png", new Rectangle(644, 912, 38, 28)))
                    {
                        Console.WriteLine("Have Khau -> kill");
                        Thread.Sleep(400);
                        AdbClient.Instance.ExecuteRemoteCommand("input tap 660 928", device, receiver);
                        Thread.Sleep(600);
                        AdbClient.Instance.ExecuteRemoteCommand("input tap 589 547", device, receiver);
                        Thread.Sleep(600);
                        AdbClient.Instance.ExecuteRemoteCommand("input tap 371 897", device, receiver);
                        Thread.Sleep(800);
                        CreateTeam();
                        Thread.Sleep(400);




                    }

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception ex:"+ex.Message);
                Console.WriteLine("Exception ex:" + ex.StackTrace);
                
            }



            NextAccount();





        }
        public void NextAccount()
        {
            if (accountIndex == (Config.Tables["account"].Rows.Count - 1))
            {
                accountIndex = 0;
            }
            else
            {
                accountIndex++;

            }
            RestartApp();
            Login();
        }
    }
}
