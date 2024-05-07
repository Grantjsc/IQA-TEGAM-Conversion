Public Class Password_Form
    Private Sub Password_Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtPass.Text = ""
    End Sub

    Private Sub btnOkay_Click(sender As Object, e As EventArgs) Handles btnOkay.Click
        Okay()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub txtPass_KeyUp(sender As Object, e As KeyEventArgs) Handles txtPass.KeyUp
        If e.KeyCode = Keys.Enter Then
            Okay()
        End If
    End Sub
End Class