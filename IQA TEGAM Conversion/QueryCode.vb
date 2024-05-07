Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls

Module Query_Module
    Public connString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\LF Database\IQA Tegam Data.accdb;Persist Security Info=True;Jet OLEDB:Database Password=lfIQAtegam"
    Public connection As New OleDbConnection(connString)

    Public UOM_Value As String
    Public TegamRange As String
    Sub GetPartNumber()
        Try
            Dim MyData As String
            Dim cmd As New OleDbCommand
            Dim Data As New DataTable
            Dim adap As New OleDbDataAdapter
            connection.Open()

            MyData = "SELECT * From PartNo_tb WHERE Part_Number = '" + Form1.txtPartNumber.Text + "'"
            cmd.Connection = connection
            cmd.CommandText = MyData
            adap.SelectCommand = cmd

            adap.Fill(Data)

            If Data.Rows.Count > 0 Then

                UOM_Value = Data.Rows(0).Item("UOM").ToString
                TegamRange = Data.Rows(0).Item("Range").ToString
                Console.WriteLine(UOM_Value)
                Console.WriteLine(TegamRange)

                Form1.btnGo.Focus()
                Form1.txtPartNumber.ReadOnly = True
                Form1.btnGo.Enabled = True

                If Not Form1.SerialPort1.IsOpen Then
                    Form1.SerialPort1.Open()
                End If

                Form1.SerialPort1.Write("P0" & vbCr) 'set Display (Resistance mode)
                Form1.SerialPort1.Write("X" & vbCr)
                Form1.SerialPort1.Write(TegamRange & vbCr) 'set range
                Form1.SerialPort1.Write("X" & vbCr)
                Form1.SerialPort1.Write("T3" & vbCr) 'delay one shot on talk
                Form1.SerialPort1.Write("X" & vbCr)
                Form1.SerialPort1.Write("D001" & vbCr) 'delay set to 1ms
                Form1.SerialPort1.Write("X" & vbCr)

            Else
                MsgBox("Part number does not exist in the database.", MessageBoxIcon.Error)
                Form1.txtPartNumber.Text = ""
                Form1.txtPartNumber.Focus()
            End If
        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            connection.Close()
        End Try
    End Sub

    Sub LoadTable()

        Dim myconnString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\LF Database\IQA Tegam Data.accdb;Persist Security Info=True;Jet OLEDB:Database Password=lfIQAtegam"
        Dim myconnection As New OleDbConnection(myconnString)
        Dim command As New OleDbCommand("", myconnection)
        Dim table As New DataTable

        myconnection.Open()

        If myconnection.State = ConnectionState.Open Then
            command.Connection = myconnection
            command.CommandText = "Select Part_Number, UOM, Range From PartNo_tb"

            Dim rdr As OleDbDataReader = command.ExecuteReader

            table.Load(rdr)

            EditData_Form.DataGridView1.DataSource = table

            ' Bold the header cells
            For Each column As DataGridViewColumn In EditData_Form.DataGridView1.Columns
                column.HeaderCell.Style.Font = New Font(EditData_Form.DataGridView1.Font, FontStyle.Bold)
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Next

        End If
        myconnection.Close()
    End Sub

    Sub SaveChanges()
        If EditData_Form.txtUOM.Text = "" Then
            MsgBox("Please enter UOM!", MsgBoxStyle.Critical)
        ElseIf EditData_Form.txtRange.Text = "" Then
            MsgBox("Please enter range!", MsgBoxStyle.Critical)
        Else

            Try
                connection.Open()
                Dim PN As String
                Dim Unit As String = EditData_Form.txtUOM.Text
                Dim NewRange As String = EditData_Form.txtRange.Text
                Dim query As String = "UPDATE PartNo_tb SET Part_Number = @partnum, UOM = @UnitOM, Range = @Range WHERE Part_Number = @PNumber"
                PN = EditData_Form.txtPartNo.Text
                'Using connection As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\LF Database\IQA Tegam Data.accdb;Persist Security Info=True;Jet OLEDB:Database Password=lfIQAtegam")
                Using command As New OleDbCommand(query, connection)
                    command.Parameters.AddWithValue("@Partnum", PN)
                    command.Parameters.AddWithValue("@UnitOM", Unit)
                    command.Parameters.AddWithValue("@Range", NewRange)
                    command.Parameters.AddWithValue("@PNumber", PN)
                    command.ExecuteNonQuery()
                    'connection.Close()
                End Using
                'End Using
                'LoadTable()

                Form1.txtPartNumber.Text = "Enter part number"
                Form1.txtPartNumber.ForeColor = Color.Silver

                Form1.txtReading.Text = ""
                Form1.txtCoversion.Text = ""
                Form1.txtPartNumber.Focus()
                Form1.txtPartNumber.ReadOnly = False
                Form1.btnGo.Enabled = False

                EditData_Form.Close()

            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                connection.Close()
            End Try
        End If
    End Sub

    Sub Cellclick()
        Try
            Dim mydata As String
            Dim command As New OleDbCommand
            Dim data As New DataTable
            Dim adap As New OleDbDataAdapter
            Dim val As String

            connection.Open()

            val = EditData_Form.DataGridView1.SelectedCells.Item(0).Value.ToString()

            mydata = "SELECT * From PartNo_tb WHERE Part_Number LIKE '" & val & "'"
            command.Connection = connection
            command.CommandText = mydata
            adap.SelectCommand = command

            adap.Fill(data)

            If data.Rows.Count > 0 Then

                EditData_Form.txtPartNo.Text = data.Rows(0).Item("Part_Number").ToString
                EditData_Form.txtUOM.Text = data.Rows(0).Item("UOM").ToString
                EditData_Form.txtRange.Text = data.Rows(0).Item("Range").ToString

            End If
        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            connection.Close()
        End Try
    End Sub

    Sub AddPN_inDb()
        Dim mycommand As String

        Dim Partno As String = AddPN_Form.txtPartNo.Text
        Dim Unit As String = AddPN_Form.cboUOM.Text
        Dim range As String = AddPN_Form.cboRange.Text

        If AddPN_Form.txtPartNo.Text = "" Then
            MsgBox("Please enter part number!", MessageBoxIcon.Error)
        ElseIf AddPN_Form.cboUOM.Text = "" Then
            MsgBox("Please select UOM!", MessageBoxIcon.Error)
        ElseIf AddPN_Form.cboRange.Text = "" Then
            MsgBox("Please select range!", MessageBoxIcon.Error)
        Else

            Try
                connection.Open()
                mycommand = "INSERT INTO [Partno_tb] ([Part_Number],[UOM],[Range]) VALUES (@Partnum, @UnitM, @Range)"
                Using command As New OleDbCommand(mycommand, connection)
                    command.Parameters.AddWithValue("@Partnum", Partno)
                    command.Parameters.AddWithValue("@UnitM", Unit)
                    command.Parameters.AddWithValue("@Range", range)
                    command.ExecuteNonQuery()
                End Using
                connection.Close()
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)

            Finally
                AddPN_Form.txtPartNo.Clear()
                AddPN_Form.cboUOM = Nothing
                AddPN_Form.cboRange = Nothing

                AddPN_Form.Close()

            End Try
        End If
    End Sub

End Module


'Module SQLserver_Module
'    Public connString As String = "Data Source=BTGCATAP5Y3JDK3;Initial Catalog=IQA Tegam Data;Integrated Security=True;TrustServerCertificate=True"
'    Public connection As New SqlConnection(connString)

'    Public UOM_Value As String
'    Public TegamRange As String
'    Sub GetPartNumber()
'        Try
'            Dim MyData As String
'            Dim cmd As New SqlCommand
'            Dim Data As New DataTable
'            Dim adap As New SqlDataAdapter
'            connection.Open()

'            MyData = "SELECT * From PartNo_tb WHERE Part_Number = '" + Form1.txtPartNumber.Text + "'"
'            cmd.Connection = connection
'            cmd.CommandText = MyData
'            adap.SelectCommand = cmd

'            adap.Fill(Data)

'            If Data.Rows.Count > 0 Then

'                UOM_Value = Data.Rows(0).Item("UOM").ToString
'                TegamRange = Data.Rows(0).Item("Range").ToString
'                Console.WriteLine(UOM_Value)
'                Console.WriteLine(TegamRange)

'                Form1.btnGo.Focus()
'                Form1.txtPartNumber.ReadOnly = True

'                If Not Form1.SerialPort1.IsOpen Then
'                    Form1.SerialPort1.Open()
'                End If
'            Else
'                MsgBox("Part number does not exist in the database.", MessageBoxIcon.Error)
'                Form1.txtPartNumber.Text = ""
'                Form1.txtPartNumber.Focus()
'            End If
'        Catch ex As Exception
'            MsgBox(ex.Message, vbCritical)
'        Finally
'            connection.Close()
'        End Try
'    End Sub

'    Sub LoadTb()
'        Dim command As New SqlCommand("", connection)
'        Dim table As New DataTable

'        connection.Open()

'        If connection.State = ConnectionState.Open Then
'            command.Connection = connection
'            command.CommandText = "Select * From PartNo_tb"

'            Dim rdr As SqlDataReader = command.ExecuteReader

'            table.Load(rdr)

'            EditData_Form.DataGridView1.DataSource = table

'            ' Bold the header cells
'            For Each column As DataGridViewColumn In EditData_Form.DataGridView1.Columns
'                column.HeaderCell.Style.Font = New Font(EditData_Form.DataGridView1.Font, FontStyle.Bold)
'                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
'            Next

'        End If
'        connection.Close()
'    End Sub

'    Sub SaveChanges()
'        If EditData_Form.txtUOM.Text = "" Then
'            MsgBox("Please enter UOM!", MsgBoxStyle.Critical)
'        ElseIf EditData_Form.txtRange.Text = "" Then
'            MsgBox("Please enter range!", MsgBoxStyle.Critical)
'        Else

'            Try
'                'connection.Open()
'                Dim PN As String
'                Dim Unit As String = EditData_Form.txtUOM.Text
'                Dim NewRange As String = EditData_Form.txtRange.Text
'                Dim query As String = "UPDATE PartNo_tb SET Part_Number = @partnum, UOM = @UnitOM, Range = @Range WHERE Part_Number = @PNumber"
'                PN = EditData_Form.txtPartNo.Text
'                'Using connection As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\LF Database\IQA Tegam Data.accdb;Persist Security Info=True;Jet OLEDB:Database Password=lfIQAtegam")
'                Using command As New SqlCommand(query, connection)
'                    command.Parameters.AddWithValue("@Partnum", PN)
'                    command.Parameters.AddWithValue("@UnitOM", Unit)
'                    command.Parameters.AddWithValue("@Range", NewRange)
'                    command.Parameters.AddWithValue("@PNumber", PN)
'                    connection.Open()
'                    command.ExecuteNonQuery()
'                    connection.Close()
'                    'connection.Close()
'                End Using
'                'End Using
'                LoadTb()
'            Catch ex As Exception
'                MsgBox(ex.Message, vbCritical)
'            Finally
'                'connection.Close()
'            End Try
'        End If
'    End Sub

'    Sub Cellclick()
'        Try
'            Dim mydata As String
'            Dim command As New SqlCommand
'            Dim data As New DataTable
'            Dim adap As New SqlDataAdapter
'            Dim val As String

'            connection.Open()

'            val = EditData_Form.DataGridView1.SelectedCells.Item(0).Value.ToString()

'            mydata = "SELECT * From PartNo_tb WHERE Part_Number LIKE '" & val & "'"
'            command.Connection = connection
'            command.CommandText = mydata
'            adap.SelectCommand = command

'            adap.Fill(data)

'            If data.Rows.Count > 0 Then

'                EditData_Form.txtPartNo.Text = data.Rows(0).Item("Part_Number").ToString
'                EditData_Form.txtUOM.Text = data.Rows(0).Item("UOM").ToString
'                EditData_Form.txtRange.Text = data.Rows(0).Item("Range").ToString

'            End If
'        Catch ex As Exception
'            MsgBox(ex.Message, vbCritical)
'        Finally
'            connection.Close()
'        End Try
'    End Sub

'    Sub AddPN_inDb()
'        Dim mycommand As String

'        Dim Partno As String = AddPN_Form.txtPartNo.Text
'        Dim Unit As String = AddPN_Form.cboUOM.Text
'        Dim range As String = AddPN_Form.cboRange.Text

'        If AddPN_Form.txtPartNo.Text = "" Then
'            MsgBox("Please enter part number!", MessageBoxIcon.Error)
'        ElseIf AddPN_Form.cboUOM.Text = "" Then
'            MsgBox("Please select UOM!", MessageBoxIcon.Error)
'        ElseIf AddPN_Form.cboRange.Text = "" Then
'            MsgBox("Please select range!", MessageBoxIcon.Error)
'        Else

'            Try
'                connection.Open()
'                mycommand = "INSERT INTO [Partno_tb] ([Part_Number],[UOM],[Range]) VALUES (@Partnum, @UnitM, @Range)"
'                Using command As New SqlCommand(mycommand, connection)
'                    command.Parameters.AddWithValue("@Partnum", Partno)
'                    command.Parameters.AddWithValue("@UnitM", Unit)
'                    command.Parameters.AddWithValue("@Range", range)
'                    command.ExecuteNonQuery()
'                End Using
'                connection.Close()
'            Catch ex As Exception
'                MsgBox(ex.Message, vbCritical)

'            Finally
'                AddPN_Form.txtPartNo.Clear()
'                AddPN_Form.cboUOM = Nothing
'                AddPN_Form.cboRange = Nothing

'                AddPN_Form.Close()

'            End Try
'        End If
'    End Sub
'End Module
