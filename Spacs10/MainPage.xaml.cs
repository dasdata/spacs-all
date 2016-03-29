using Microsoft.Maker.RemoteWiring;
using Microsoft.VisualBasic;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Text;
using Microsoft.Azure.Devices.Client;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Spacs10
{ 
    public sealed partial class MainPage : Page
    {
        private RemoteDevice arduinoFIX;  // FIXED SPACS
        private Microsoft.Maker.Serial.NetworkSerial netWorkSerialFIX;
        private string deviceFIX_IP = "192.168.0.109";
        private ushort deviceFIX_PORT = 27015;

        private RemoteDevice arduinoCAR;  // CAR SPACS 
        private Microsoft.Maker.Serial.NetworkSerial netWorkSerialCAR;
        private string deviceCAR_IP =  "192.168.0.107";
        private ushort deviceCAR_PORT = 3030;

        private int _srvIoTStream = 0;

        private float _spacsDistance = 0;

        Socket _socket = null;
        // Signaling object used to notify when an asynchronous operation is completed
        static ManualResetEvent _clientDone = new ManualResetEvent(false);
        // Define a timeout in milliseconds for each asynchronous call. If a response is not received within this 
        // timeout period, the call is aborted.
        const int TIMEOUT_MILLISECONDS = 5000;
        // The maximum size of the data buffer to use with the asynchronous socket methods
        const int MAX_BUFFER_SIZE = 2048;

        public MainPage()
        {
            this.InitializeComponent();
            cmdLogMe(" Hello SPACS ");
            try {

                // MAKE CAR CONNECTION DEVICE 107 
                netWorkSerialCAR = new Microsoft.Maker.Serial.NetworkSerial(new Windows.Networking.HostName(deviceCAR_IP), deviceCAR_PORT);
                arduinoCAR = new Microsoft.Maker.RemoteWiring.RemoteDevice(netWorkSerialCAR);
                //Attach event handlers
                netWorkSerialCAR.ConnectionEstablished += NetWorkSerial_ConnectionEstablished;
                netWorkSerialCAR.ConnectionFailed += NetWorkSerial_ConnectionFailed;
                netWorkSerialCAR.begin(57600, Microsoft.Maker.Serial.SerialConfig.SERIAL_8N1);

                //SEND DATA TO THE Azure IoT Hub
               // SendDeviceToCloudMessagesAsync("Testing the void!");
     

            } 
            
             catch (Exception ex)
            {
                cmdLogMe("ERROR..." + ex.Message.ToString());
            }
        }

     
      

        private void NetWorkSerial_ConnectionEstablished()
        {
            try
            {
                cmdLogMe(" SPACS device ["+ deviceCAR_IP + "] connected. YES!");
                arduinoCAR.pinMode("6", Microsoft.Maker.RemoteWiring.PinMode.OUTPUT);
                arduinoCAR.pinMode("12", Microsoft.Maker.RemoteWiring.PinMode.OUTPUT); 
            }
            catch (Exception ex)
            {
                cmdLogMe("ERROR..." + ex.Message.ToString());
            }

        }

        private void NetWorkSerial_ConnectionFailed(string message)
        {
            cmdLogMe(" ISSUES: SPACS Car ["+ deviceCAR_IP + "] Connection Failed: " + message);
        }

   
        private void tglCarStatus_Toggled_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tglCarStatus.IsOn)
                {
                    cmdLogMe(" CAR ON "+ deviceCAR_IP );
                    btnDistancePedestrian.Foreground = new SolidColorBrush(Colors.White);
                    carStart();
                }
                else
                {
                    cmdLogMe(" CAR OFF " + deviceCAR_IP);
                    carStop();
                }
            }
            catch (Exception ex)
            {
                cmdLogMe("ERROR..." + ex.Message.ToString());
            }

        }

        /// <summary>
        /// CAR START - WARING - STOP 
        /// </summary>
        private void carStart() {
            try
            {
                arduinoCAR.digitalWrite(6, Microsoft.Maker.RemoteWiring.PinState.HIGH);
                arduinoCAR.digitalWrite(12, Microsoft.Maker.RemoteWiring.PinState.HIGH);
                btnDistancePedestrian.Foreground = new SolidColorBrush(Colors.Green);
            }
            catch (Exception ex)
            {
                cmdLogMe("ERROR..." + ex.Message.ToString());
            }
        }

        private void carWarn()
        {
            try
            {
                cmdLogMe(" WARNING SPACS ... ");
                btnDistancePedestrian.Foreground = new SolidColorBrush(Colors.Yellow);
            }
            catch (Exception ex)
            {
                cmdLogMe("ERROR..." + ex.Message.ToString());
            }
        }


        private void carStop()
        {
            try
            {
                arduinoCAR.digitalWrite(6, Microsoft.Maker.RemoteWiring.PinState.LOW);
                arduinoCAR.digitalWrite(12, Microsoft.Maker.RemoteWiring.PinState.LOW);
                btnDistancePedestrian.Foreground = new SolidColorBrush(Colors.Red);
            }
            catch (Exception ex)
            {
                cmdLogMe("ERROR..." + ex.Message.ToString());
            }
        }


        /// <summary>
        /// SPACS - Azure IoT HUB 
        /// </summary>


        static async void SendDeviceToCloudMessagesAsync(string _strVal)
        {
            //  HostName=YOUR SERVICE.azure-devices.net;DeviceId=device11;SharedAccessKey=fsfsdfsd433454534rfsfer
            string iotHubUri = ""; // ! put in value !
            string deviceId = ""; // ! put in value !
            string deviceKey = ""; // ! put in value !

            var deviceClient = DeviceClient.Create(iotHubUri,
                    AuthenticationMethodFactory.
                        CreateAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey),
                    TransportType.Http1);

            //  var str = "Hello, Azure Cloud!";
            var message = new Message(Encoding.ASCII.GetBytes(_strVal));

            await deviceClient.SendEventAsync(message);
        }

        private void tglAzureIot_Toggled(object sender, RoutedEventArgs e)
        {
            //IS OK TO STORE TO THE CLOUD! 
            try
            {
                if (tglAzureIot.IsOn)
                {
                    cmdLogMe("Hello Azure IOT from SPACS ");
                    _srvIoTStream = 1;
                }
                else
                {
                    cmdLogMe("Azure IOT disabled from SPACS");
                    _srvIoTStream = 0;
                }
            }
            catch (Exception ex)
            {
                cmdLogMe("ERROR..." + ex.Message.ToString() );
            }
             

        }

        /// <summary>
        /// LOG LOCALLY /  AZURE IoT HUB
        /// </summary>
        /// <param name="message"></param>
        /// 
        private void cmdLogMe(string message)
        {
            try
            {
                txtConsole.Text += System.DateTime.Now +" "+ message + Constants.vbNewLine;
                //STORE TO AZURE IOT
                if (_srvIoTStream == 1)
                {
                    SendDeviceToCloudMessagesAsync(message);
                }

            }
            catch (Exception ex)
            {
                cmdLogMe("ERROR..." + ex.Message.ToString());
            }


           
        }

        private async void WaitForOneSecond()
        {
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2));
            //   TimerStatus.Text = Convert.ToString(System.DateTime.Now) ;
            // do something after 5 seconds! 
            cmdLogMe("Reading from socket!");
            if (_socket != null)
            {
                cmdLogMe(Receive());
            }
                WaitForOneSecond();
        }

        /// <summary>
        /// SOCKET CONNECTION
        /// </summary>
        /// <param name="data"></param>
        /// 

        private void tglSPACSStatus_Toggled(object sender, RoutedEventArgs e)
        {
            if (tglSPACSStatus.IsOn)
            {
                try
                {
                    cmdLogMe(Connect(deviceFIX_IP, Convert.ToInt32(deviceFIX_PORT))); ////"Try to connect to pedestrian shield..."); 
                    cmdLogMe(Send("Hello from Spacs App"));
                    cmdLogMe(Receive());
                    // GET DISTANCE FROM PEDESTRIAN - SPACS  
                    _spacsDistance = 30;
                    if (_spacsDistance < 30)
                    {
                        carStop();
                    }
                    else {
                        carStart();
                    }

                            //SOCKET CONNECTION XXXX
                            if (_socket != null)
                    {
                    
                        //    _socket.Close();
                        //     _socket.OnDataRecived -= socket_OnDataRecived;
                      //  _socket = null;
                    }
 
                    cmdLogMe("Connected to pedestrian shield...");
                }
                catch (Exception ex)
                {
                    cmdLogMe("ERROR..." + ex.Message.ToString());
                }

            }
        }
       
     

        public string Connect(string hostName, int portNumber)
        {
            string result = string.Empty;
            DnsEndPoint hostEntry = new DnsEndPoint(hostName, portNumber);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.RemoteEndPoint = hostEntry;
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object s, SocketAsyncEventArgs e)
            {
                result = e.SocketError.ToString();
                _clientDone.Set();
            });
            _clientDone.Reset();
            _socket.ConnectAsync(socketEventArg);
            _clientDone.WaitOne(TIMEOUT_MILLISECONDS);
            return result;
        }

        /// <summary>
        /// Send the given data to the server using the established connection
        /// </summary>
        /// <param name="data">The data to send to the server</param>
        /// <returns>The result of the Send request</returns>
        public string Send(string data)
        {
            string response = "Operation Timeout";
            if (_socket != null)
            {

                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = _socket.RemoteEndPoint;
                socketEventArg.UserToken = null;
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object s, SocketAsyncEventArgs e)
                {
                    response = e.SocketError.ToString();
                    _clientDone.Set();
                });
                
                byte[] payload = Encoding.UTF8.GetBytes(data);
                socketEventArg.SetBuffer(payload, 0, payload.Length);
                _clientDone.Reset();
                _socket.SendAsync(socketEventArg);
                _clientDone.WaitOne(TIMEOUT_MILLISECONDS);
            }
            else
            {
                response = "Socket is not initialized";
            }
            return response;
        }


        public string Receive()
        {
             string response = "Operation Timeout";
            if (_socket != null)
            {
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = _socket.RemoteEndPoint;
                socketEventArg.SetBuffer(new Byte[MAX_BUFFER_SIZE], 0, MAX_BUFFER_SIZE);
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object s, SocketAsyncEventArgs e)
                {
                    if (e.SocketError == SocketError.Success)
                    {
                        // Retrieve the data from the buffer
                        response = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                        response = response.Trim('\0');
                    }
                    else
                    {
                        response = e.SocketError.ToString();
                    }

                    _clientDone.Set();
                });
                
                _clientDone.Reset();
                _socket.ReceiveAsync(socketEventArg);
                _clientDone.WaitOne(TIMEOUT_MILLISECONDS);
            }
            else
            {
                response = "Socket is not initialized";
            }

            return response;
        }





        /// <summary>
        /// Closes the Socket connection and releases all associated resources
        /// </summary>
        public void Close()
        {
            if (_socket != null)
            {
               _socket.Dispose();
            }
        }

        //END OF VOIDS
    }
}
