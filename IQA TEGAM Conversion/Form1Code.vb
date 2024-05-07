Imports System.Configuration
Imports System.Data.OleDb
Imports System.IO.Ports
Imports System.Threading
Imports Microsoft.Office.Interop.Excel

Module Computation_Module
    Sub Ohms_ft()
        Dim Eq1 As String
        Eq1 = CDec(Form1.txtReading.Text) / 3

        Form1.txtCoversion.Text = Math.Round(CDec(Eq1), 5)
    End Sub

    Sub Ohms_M()
        Dim Eq1 As String
        Dim Eq2 As Decimal

        Eq1 = CDec(Form1.txtReading.Text) / 3
        Eq2 = Eq1 * 3.28

        Form1.txtCoversion.Text = Math.Round(Eq2, 5)
    End Sub

    Sub ReaLVal()
        Select Case UOM_Value.ToString

            Case "Ohms/ft"
                Ohms_ft()

            Case "Ohms/M"
                Ohms_M()

        End Select
    End Sub

End Module

Module TegamCommand_Module
    Sub TegamFunction()
        'Form1.SerialPort1.Write("P0" & vbCr) 'set Display (Resistance mode)
        'Form1.SerialPort1.Write("X" & vbCr)
        'Form1.SerialPort1.Write(TegamRange & vbCr) 'set range
        'Form1.SerialPort1.Write("X" & vbCr)
        'Form1.SerialPort1.Write("T3" & vbCr) 'delay one shot on talk
        'Form1.SerialPort1.Write("X" & vbCr)
        'Form1.SerialPort1.Write("D001" & vbCr) 'delay set to 1ms
        'Form1.SerialPort1.Write("X" & vbCr)
        'Thread.Sleep(100)
        Form1.SerialPort1.WriteLine("E" & vbCr) 'Trigger
        Form1.SerialPort1.Write("X" & vbCr)
    End Sub
End Module

Module MenuStrip_Module

    Public ChangeSerialPass As Boolean = False

    Sub AddPartNumber()
        With AddPN_Form
            EditData_Form.Close()
            ChangeSerial_Form.Close()
            .TopLevel = False
            Form1.Panel1.Controls.Add(AddPN_Form)
            .WindowState = FormWindowState.Maximized
            .BringToFront()
            .Show()
        End With
    End Sub

    Sub EditData()
        With EditData_Form
            ChangeSerial_Form.Close()
            AddPN_Form.Close()
            .TopLevel = False
            Form1.Panel1.Controls.Add(EditData_Form)
            .WindowState = FormWindowState.Maximized
            .BringToFront()
            .Show()
        End With
    End Sub

    Sub ChangeSerial()
        'ChangeSerialPass = True
        Password_Form.ShowDialog()
    End Sub
End Module

Module ChangeSerial_Module
    Public config As Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
    Public Serial1 As String
    Public NewSerialName As String

    Sub GetSerialName()
        Dim serial As String = System.Configuration.ConfigurationManager.AppSettings("Serialname")
        Console.WriteLine(serial)

        Serial1 = serial
    End Sub

    Sub GetNewSerial()
        NewSerialName = ChangeSerial_Form.cboSerialList.Text
    End Sub

    Sub SaveChangesInSerial()
        config.AppSettings.Settings("Serialname").Value = NewSerialName ' Rewrite Mettlet Toledo Weighing Scale COM name
        config.Save(ConfigurationSaveMode.Modified) ' save the new value

        ConfigurationManager.RefreshSection("appSettings") 'refresh
    End Sub
    Sub LoadComPort()

        Dim Serials As String() = SerialPort.GetPortNames()
        For Each portname As String In Serials
            ChangeSerial_Form.cboSerialList.Items.Add(portname)
        Next
    End Sub
    Sub ButtonSaveSerial()
        If ChangeSerial_Form.cboSerialList.Text = "" Then
            MsgBox("Please input serial name", MsgBoxStyle.Critical)
        Else
            GetNewSerial()
            SaveChangesInSerial()
        End If

        GetSerialName()
        Form1.SerialPort1.PortName = Serial1
        ChangeSerial_Form.Close()
    End Sub

End Module