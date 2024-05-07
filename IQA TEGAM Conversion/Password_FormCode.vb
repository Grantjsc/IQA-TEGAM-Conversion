Module Password_Module
    Sub Okay()
        If Password_Form.txtPass.Text = "IQAmaster" Then
            Password_Form.Close()

            With ChangeSerial_Form
                .TopLevel = False
                Form1.Panel1.Controls.Add(ChangeSerial_Form)
                .WindowState = FormWindowState.Maximized
                .BringToFront()
                .Show()
            End With
        Else
            MsgBox("Wrong password", MsgBoxStyle.Critical)
            Password_Form.txtPass.Text = ""
        End If
    End Sub
End Module