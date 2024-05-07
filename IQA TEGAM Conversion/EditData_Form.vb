Imports Guna.UI2.Native.WinApi
Public Class EditData_Form
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        SaveChanges()
    End Sub

    Private Sub EditData_Form_Load(sender As Object, e As EventArgs) Handles Me.Load
        LoadTable()
    End Sub

    'Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs)
    '    Cellclick()
    'End Sub

    Private Sub CheckBoxUOM_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxUOM.CheckedChanged
        If CheckBoxUOM.Checked Then
            txtUOM.ReadOnly = False
        Else
            txtUOM.ReadOnly = True
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxRange.CheckedChanged
        If CheckBoxRange.Checked Then
            txtRange.ReadOnly = False
        Else
            txtRange.ReadOnly = True
        End If
    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        Cellclick()
    End Sub
End Class