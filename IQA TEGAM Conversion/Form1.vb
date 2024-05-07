Imports System.Data.OleDb
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Guna.UI2.Native.WinApi
'Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Runtime.InteropServices
Imports Microsoft.Office.Interop

Public Class Form1
    Dim xlApp As Excel.Application
    Dim xlWorkbook As Excel.Workbook
    Dim xlWorksheet As Excel.Worksheet

    'Dim xlApp As Object
    'Dim xlWorkbook As Object
    'Dim xlWorksheet As Object

    Public Tegam As String
    Public TegamRead As String

    'Public Sub InputToExcel()
    '    ' Get the existing instance of Excel
    '    Try
    '        xlApp = System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application")
    '    Catch ex As Exception
    '        MessageBox.Show("Excel is not running.")
    '        Return
    '    End Try

    '    ' Activate the workbook
    '    xlWorkbook = xlApp.ActiveWorkbook
    '    If xlWorkbook Is Nothing Then
    '        MessageBox.Show("No workbook is active in Excel.")
    '        Return
    '    End If

    '    ' Activate the worksheet
    '    xlWorksheet = xlWorkbook.ActiveSheet

    '    '*********** Pasting in Selected Cell **************
    '    Dim ToExcel As String = txtCoversion.Text
    '    xlApp.ActiveCell.Value = ToExcel 'paste to excel
    '    xlApp.ActiveCell.Offset(1, 0).Select() 'Move to next cell


    '    '    '*********** Pasting in Set Start Cell **************
    '    '    Dim startCell As Excel.Range = xlWorksheet.Range("D9")
    '    '    Dim ToExcel As String = TextBox1.Text

    '    '    If startCell.Value Is Nothing Then
    '    '        startCell.Value = ToExcel
    '    '        startCell.Offset(1, 0).Select()
    '    '    Else

    '    '        ' Paste the text into the next cell
    '    '        xlApp.ActiveCell.Value = textToPaste
    '    '        xlApp.ActiveCell.Offset(1, 0).Select() 'Move to next cell
    '    '    End If
    'End Sub

    Private Sub InputToExcel()
        ' Check if Excel is running
        If Not IsExcelRunning() Then
            MessageBox.Show("Excel is not running.")
            Return
        End If

        ' Try to get the existing instance of Excel
        Try
            xlApp = Marshal.GetActiveObject("Excel.Application")
        Catch ex As Exception
            ' If Excel is not running, create a new instance
            Try
                xlApp = CreateObject("Excel.Application")
            Catch exc As Exception
                MessageBox.Show("Error creating Excel instance: " & exc.Message)
                Return
            End Try
        End Try

        ' Check if xlApp is Nothing
        If xlApp Is Nothing Then
            MessageBox.Show("Unable to get the Excel application instance.")
            Return
        End If

        ' Wait for Excel to fully initialize
        Thread.Sleep(500)

        ' Activate the workbook
        Try
            xlWorkbook = xlApp.ActiveWorkbook
            If xlWorkbook Is Nothing Then
                MessageBox.Show("No workbook is active in Excel.")
                Return
            End If
        Catch ex As Exception
            MessageBox.Show("Error accessing workbook: " & ex.Message)
            Return
        End Try

        ' Activate the worksheet
        Try
            xlWorksheet = xlWorkbook.ActiveSheet
        Catch ex As Exception
            MessageBox.Show("Error accessing worksheet: " & ex.Message)
            Return
        End Try

        ' Pasting in selected cell
        Try
            Dim toExcel As String = txtCoversion.Text
            xlApp.ActiveCell.Value = toExcel ' Paste to Excel
            xlApp.ActiveCell.Offset(1, 0).Select() ' Move to next cell
        Catch ex As Exception
            MessageBox.Show("Error pasting into Excel: " & ex.Message)
        End Try
    End Sub

    Private Function IsExcelRunning() As Boolean
        ' Check if Excel is running by attempting to get the active object
        Try
            Dim obj As Object = Marshal.GetActiveObject("Excel.Application")
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function


    Private Sub btnGo_KeyUp(sender As Object, e As KeyEventArgs) Handles btnGo.KeyUp
        If e.KeyCode = Keys.Enter Then
            If txtPartNumber.Text = "Enter part number" Or txtPartNumber.Text = "" Then
                MsgBox("Please enter part number!", MsgBoxStyle.Information)
            Else

                TegamFunction()
                'Convert()
                'Thread.Sleep(100)
                'InputToExcel()
            End If
        End If
    End Sub

    Private Sub txtPartNumber_Enter(sender As Object, e As EventArgs) Handles txtPartNumber.Enter
        If txtPartNumber.Text = "Enter part number" Then
            txtPartNumber.Text = ""
            txtPartNumber.ForeColor = Color.Black
        End If
    End Sub

    Private Sub txtPartNumber_Leave(sender As Object, e As EventArgs) Handles txtPartNumber.Leave
        If txtPartNumber.Text = "" Then
            txtPartNumber.Text = "Enter part number"
            txtPartNumber.ForeColor = Color.Silver
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Dim dialog As DialogResult
        dialog = MessageBox.Show("Do you really want to exit?", "Exit application", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If dialog = DialogResult.No Then
            e.Cancel = True
        Else
            Application.ExitThread()
        End If
    End Sub

    Private Sub txtPartNumber_KeyUp(sender As Object, e As KeyEventArgs) Handles txtPartNumber.KeyUp
        If e.KeyCode = Keys.Enter Then
            GetPartNumber()
        End If
    End Sub

    Private Sub AddPartNumberToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddPartNumberToolStripMenuItem.Click
        AddPartNumber()
    End Sub

    Private Sub EditSetRangeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditSetRangeToolStripMenuItem.Click
        EditData()
    End Sub

    Private Sub ChangeSerialNameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ChangeSerialNameToolStripMenuItem.Click
        ChangeSerial()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GetSerialName()
        SerialPort1.PortName = Serial1

        btnGo.Enabled = False
    End Sub
    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        If txtPartNumber.Text = "Enter part number" Or txtPartNumber.Text = "" Then
            MsgBox("There is no data to clear!", MsgBoxStyle.Information)
        Else
            txtPartNumber.Text = "Enter part number"
            txtPartNumber.ForeColor = Color.Silver

            txtReading.Text = ""
            txtCoversion.Text = ""
            txtPartNumber.Focus()
            txtPartNumber.ReadOnly = False
            btnGo.Enabled = False
        End If
    End Sub

    Private Sub btnGo_Click(sender As Object, e As EventArgs) Handles btnGo.Click
        TegamFunction()
        'Convert()
        'Thread.Sleep(100)
        'InputToExcel()
    End Sub

    Private Sub SerialPort1_DataReceived_1(sender As Object, e As IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        TegamRead = SerialPort1.ReadLine
        TegamRead = TegamRead.Replace(" ", "")

        Dim regex As New Regex("-?\d+(\.\d+)?")
        Dim matches As MatchCollection = regex.Matches(TegamRead)
        For Each match As Match In matches
            Tegam = match.Value
            Console.WriteLine(Tegam)
        Next

        Invoke(Sub()
                   txtReading.Text = Tegam
                   Thread.Sleep(100)
                   ReaLVal()
                   Thread.Sleep(100)
                   InputToExcel()
               End Sub)

        'ReaLVal()
        'Thread.Sleep(100)
        'InputToExcel()
    End Sub
End Class
