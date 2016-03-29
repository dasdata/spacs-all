using Microsoft.VisualBasic;
using System;
using Windows.UI.Xaml;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Net;

namespace Spacs_CSharp_Universal
{


    public sealed partial class MainPage 
    {

      


        private Microsoft.Maker.RemoteWiring.RemoteDevice arduino;
        private Microsoft.Maker.Serial.NetworkSerial netWorkSerial;
        //TO DO - Socket connection!
        private string deviceCAR_IP;
        private ushort deviceCAR_PORT;

        private string deviceSPACS_IP;
        private ushort deviceSPACS_PORT;

        public static string data = null;

        ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////        
        ////                              SPACS C# - EARLY VERSION MUST BE UPDATED                ////  
        ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////  ////    
          
        public MainPage()
        {
            this.InitializeComponent();
            //Get IP values from the interface
         //   _SpacsFIXED = "http://" + txtSPACSAddress.Text.ToString;
            deviceCAR_IP = txtCarAddress.Text;
            deviceCAR_PORT = Convert.ToUInt16(txtCar_Port.Text);
            deviceSPACS_IP = txtSPACSAddress.Text;
            deviceSPACS_PORT = Convert.ToUInt16(txtSPACSPort.Text);

            //Init Arduino connections and start car
           this.InitArduinoCaR(deviceCAR_IP, deviceCAR_PORT);

            //using (var socket = new so("127.0.0.1", 1337)) // Connects to 127.0.0.1 on port 1337
            //{
            //    socket.Send("Hello world!"); // Sends some data
            //    var data = socket.Receive(); // Receives some data back (blocks execution)
            //}


            //   DispatcherTimerSetup();
        }

  



        private async void WaitForOneSecond()
        {
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));
            //   TimerStatus.Text = Convert.ToString(System.DateTime.Now) ;
            // do something after 5 seconds!
      
        }


 

    //'============================================================================== 
    public void InitArduinoCaR(string _deviceIP, ushort _devicePORT)
        {
            try
            {
                txtConsole.Text += "Hello SPACS - Smart Pedestrian Anti-Collision System" + Constants.vbNewLine;
                //Establish a network serial connection. change it to the right IP address and port 
                netWorkSerial = new Microsoft.Maker.Serial.NetworkSerial(new Windows.Networking.HostName(_deviceIP), _devicePORT);

                //Create Arduino Device
                arduino = new Microsoft.Maker.RemoteWiring.RemoteDevice(netWorkSerial);

                //Attach event handlers
                //netWorkSerial.ConnectionEstablished += NetWorkSerial_ConnectionEstablished;
                //netWorkSerial.ConnectionFailed += NetWorkSerial_ConnectionFailed;

                //Begin connection
                txtConsole.Text += System.DateTime.Now + " begin connection" + _deviceIP + Constants.vbNewLine;
                netWorkSerial.begin(57600, Microsoft.Maker.Serial.SerialConfig.SERIAL_8N1);

                cmdStartCar();

            }
            catch (Exception ex)
            {
                txtConsole.Text += "Ohh man..." + ex.Message.ToString() + Constants.vbNewLine;
            }

        }

        private void NetWorkSerial_ConnectionEstablished()
        {
            try
            {
                txtConsole.Text += System.DateTime.Now + " break device link. yes!" + Constants.vbNewLine;
                arduino.pinMode("12", Microsoft.Maker.RemoteWiring.PinMode.OUTPUT);
                arduino.pinMode("6", Microsoft.Maker.RemoteWiring.PinMode.OUTPUT);
                //Set the pin to output
                //arduino.digitalWrite(6, Microsoft.Maker.RemoteWiring.PinState.HIGH)
            }
            catch (Exception ex)
            {
                txtConsole.Text += "Ohh man..." + ex.Message.ToString() + Constants.vbNewLine;
            }

        }

        private void NetWorkSerial_ConnectionFailed(string message)
        {
            txtConsole.Text += (System.DateTime.Now + " ISSUES: SPACS Car Connection Failed: " + message);
        }


        private void cmdStartCar()
        {
            //HIGH VALUES ON LED and RELAY 
            deviceCAR_IP = txtCarAddress.Text;
            txtConsole.Text += System.DateTime.Now + " SPACS CAR ON" + Constants.vbNewLine;
            arduino.digitalWrite(12, Microsoft.Maker.RemoteWiring.PinState.HIGH);
            arduino.digitalWrite(6, Microsoft.Maker.RemoteWiring.PinState.HIGH);
        }

        private void cmdStopCar()
        {
            //LOW VALUES ON LED and RELAY 
            try
            {
                arduino.digitalWrite(12, Microsoft.Maker.RemoteWiring.PinState.LOW);
                arduino.digitalWrite(6, Microsoft.Maker.RemoteWiring.PinState.LOW);
                txtConsole.Text += System.DateTime.Now + " SPACS CAR OFF" + Constants.vbNewLine;
            }
            catch (Exception ex)
            {
            }
        }


        

        private void tglAzureIot_Toggled(object sender, RoutedEventArgs e)
        {
            //TO DO - Store data to IoT Azure Hub
        }

        private void tglCarStatus_Toggled_1(object sender, RoutedEventArgs e)
        {    
            //START /STOP CAR
            try
            {
                if (tglCarStatus.IsOn)
                {
                    cmdStartCar();
                }
                else {
                    cmdStopCar();
                }

            }
            catch (Exception ex)
            {
            }

        }

        private void tglSPACSStatus_Toggled_1(object sender, RoutedEventArgs e)
        {
          //  cmdConnectToSpacs();
         
            try
            {
                if (tglSPACSStatus.IsOn)
                {
                 //   Connect();
                }
                else {
                //    Disconnect();
                }

            }
            catch (Exception ex)
            {
            }

        }


 

 



    }
}

        //// TIMER 1 seconds - check connections
        //private DispatcherTimer dispatcherTimer;
        //private int timesTicked = 1;
        //private int timesToTick = 200000;
        //public void DispatcherTimerSetup()
        //{
        //    dispatcherTimer = new DispatcherTimer();
        //    dispatcherTimer.Tick += dispatcherTimer_Tick;
        //    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        //    dispatcherTimer.Start();
        //    TimerStatus.Text = "Setup timer. OK!";
        //}

        //private void dispatcherTimer_Tick(object sender, object e)
        //{
        //    // GetSpacsDistance(_SpacsFIXED);
         
        //    TimerStatus.Text = Convert.ToString(System.DateTime.Now);
              
 
        //    if (timesTicked > timesToTick)
        //    {
        //        dispatcherTimer.Stop();
        //        //STOP THE CHECKING
        //    }
        //    timesTicked += 1;
        //}