Imports Microsoft.Maker.Serial
Imports Microsoft.Maker.RemoteWiring
Imports System.Net.Sockets
Imports System.Net
Imports System.Text
Imports System.Net.Http
Imports System.IO.Compression


Public NotInheritable Class MainPage
    Inherits Page

    Private arduino As Microsoft.Maker.RemoteWiring.RemoteDevice
    Private netWorkSerial As Microsoft.Maker.Serial.NetworkSerial
    Private _SpacsFIXED As String   'TO DO - Socket connection!
    Private deviceCAR_IP As String
    Public Shared data As String = Nothing

    Public Sub New()
        Try
            Me.InitializeComponent()
            'Get IP values from the interface
            _SpacsFIXED = txtSPACSAddress.Text.ToString
            deviceCAR_IP = txtCarAddress.Text

            'Init Arduino connections and start car
            ''  Me.InitArduinoCaR(deviceCAR_IP)
            InitArduinoFixed(_SpacsFIXED)
            '     cmdStartCar()
            'Init Timer for distance readings

        Catch ex As Exception

        End Try


    End Sub



    ''============================================================================== 
    Public Sub InitArduinoFixed(ByVal _deviceIP As String)
        Try
            txtConsole.Text &= "Hello SPACS - Fixed Anti-Collision System" & vbNewLine
            'Establish a network serial connection. change it to the right IP address and port 
            netWorkSerial = New Microsoft.Maker.Serial.NetworkSerial(New Windows.Networking.HostName(_deviceIP), 3030)

            'Create Arduino Device
            arduino = New Microsoft.Maker.RemoteWiring.RemoteDevice(netWorkSerial)

            'Attach event handlers
            AddHandler netWorkSerial.ConnectionEstablished, AddressOf NetWorkSerial_ConnectionEstablished
            AddHandler netWorkSerial.ConnectionFailed, AddressOf NetWorkSerial_ConnectionFailed

            'Begin connection
            txtConsole.Text &= Date.Now & " begin connection" & _deviceIP & vbNewLine
            netWorkSerial.begin(57600, Microsoft.Maker.Serial.SerialConfig.SERIAL_8N1)

            '  cmdStartCar()

        Catch ex As Exception
            txtConsole.Text &= "Ohh man..." & ex.Message.ToString & vbNewLine
        End Try

    End Sub

    ''============================================================================== 



    Private Sub NetWorkSerial_FixedConnectionEstablished()
        Try
            txtConsole.Text &= Date.Now & " break device link. yes!" & vbNewLine
            '   arduino.pinMode("12", Microsoft.Maker.RemoteWiring.PinMode.OUTPUT)
            '  arduino.pinMode("6", Microsoft.Maker.RemoteWiring.PinMode.OUTPUT)
            'Set the pin to output
            'arduino.digitalWrite(6, Microsoft.Maker.RemoteWiring.PinState.HIGH)
        Catch ex As Exception
            txtConsole.Text &= "Ohh man..." & ex.Message.ToString & vbNewLine
        End Try

    End Sub

    Private Sub NetWorkSerial_FixedConnectionFailed(message As String)
        txtConsole.Text &= (Date.Now & " ISSUES: SPACS Car Connection Failed: " & message)
    End Sub



    Public Sub InitArduinoCaR(ByVal _deviceIP As String)
        Try
            txtConsole.Text &= "Hello SPACS - Smart Pedestrian Anti-Collision System" & vbNewLine
            'Establish a network serial connection. change it to the right IP address and port 
            netWorkSerial = New Microsoft.Maker.Serial.NetworkSerial(New Windows.Networking.HostName(_deviceIP), 3030)

            'Create Arduino Device
            arduino = New Microsoft.Maker.RemoteWiring.RemoteDevice(netWorkSerial)

            'Attach event handlers
            AddHandler netWorkSerial.ConnectionEstablished, AddressOf NetWorkSerial_FixedConnectionEstablished
            AddHandler netWorkSerial.ConnectionFailed, AddressOf NetWorkSerial_FixedConnectionFailed

            'Begin connection
            txtConsole.Text &= Date.Now & " begin connection" & _deviceIP & vbNewLine
            netWorkSerial.begin(9600, Microsoft.Maker.Serial.SerialConfig.SERIAL_8N1)

            cmdStartCar()

        Catch ex As Exception
            txtConsole.Text &= "Ohh man..." & ex.Message.ToString & vbNewLine
        End Try

    End Sub

    Private Sub NetWorkSerial_ConnectionEstablished()
        Try
            txtConsole.Text &= Date.Now & " break device link. yes!" & vbNewLine
            arduino.pinMode("12", Microsoft.Maker.RemoteWiring.PinMode.OUTPUT)
            arduino.pinMode("6", Microsoft.Maker.RemoteWiring.PinMode.OUTPUT)
            'Set the pin to output
            'arduino.digitalWrite(6, Microsoft.Maker.RemoteWiring.PinState.HIGH)
        Catch ex As Exception
            txtConsole.Text &= "Ohh man..." & ex.Message.ToString & vbNewLine
        End Try

    End Sub

    Private Sub NetWorkSerial_ConnectionFailed(message As String)
        txtConsole.Text &= (Date.Now & " ISSUES: SPACS Car Connection Failed: " & message)
    End Sub


    Private Function cmdStartCar()
        'HIGH VALUES ON LED and RELAY 
        deviceCAR_IP = txtCarAddress.Text
        txtConsole.Text &= Date.Now & " SPACS CAR ON" & vbNewLine
        arduino.digitalWrite(12, Microsoft.Maker.RemoteWiring.PinState.HIGH)
        arduino.digitalWrite(6, Microsoft.Maker.RemoteWiring.PinState.HIGH)
    End Function

    Private Function cmdStopCar()
        'LOW VALUES ON LED and RELAY 
        Try
            arduino.digitalWrite(12, Microsoft.Maker.RemoteWiring.PinState.LOW)
            arduino.digitalWrite(6, Microsoft.Maker.RemoteWiring.PinState.LOW)
            txtConsole.Text &= Date.Now & " SPACS CAR OFF" & vbNewLine
        Catch ex As Exception
        End Try
    End Function


    Public Function GetRelativeDateValue(ByVal [date] As DateTime, ByVal comparedTo As DateTime) As String
        Dim diff As TimeSpan = comparedTo.Subtract([date])

        If diff.Days >= 7 Then
            Return String.Concat("on ", [date].ToString("MMMM dd, yyyy"))
        ElseIf diff.Days > 1 Then
            Return String.Concat(diff.Days, " days ago")
        ElseIf diff.Days = 1 Then
            Return "yesterday"
        ElseIf diff.Hours >= 2 Then
            Return String.Concat(diff.Hours, " hours ago")
        ElseIf diff.Minutes >= 60 Then
            Return "more than an hour ago"
        ElseIf diff.Minutes >= 5 Then
            Return String.Concat(diff.Minutes, " minutes ago")
        End If
        If diff.Minutes >= 1 Then
            Return "a few minutes ago"
        Else
            Return "less than a minute ago"
        End If
    End Function

    Private Sub tglCarStatus_Toggled(sender As Object, e As RoutedEventArgs) Handles tglCarStatus.Toggled
        'START /STOP CAR
        Try
            If tglCarStatus.IsOn Then
                cmdStartCar()
            Else
                cmdStopCar()
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub tglSPACSStatus_Toggled(sender As Object, e As RoutedEventArgs) Handles tglSPACSStatus.Toggled
        'ENABLE COLLISION SHIELD
        Try
            _SpacsFIXED = "http://" & txtSPACSAddress.Text.ToString
            btnDistancePedestrian.Content = ""
            btnCoord.Content = ""
        Catch ex As Exception
        End Try
    End Sub

    Private Sub tglAzureIot_Toggled(sender As Object, e As RoutedEventArgs) Handles tglAzureIot.Toggled
        'TO DO - Store data to IoT Azure Hub
    End Sub
End Class
