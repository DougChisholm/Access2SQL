VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Form_Klienci"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False


Private Sub Polecenie58_Click()
    ' clear the cart
    Dim id As String
    id = Me.id_klienta.Value
    
    DoCmd.RunSQL ("DELETE FROM Koszyk where id_klienta=" & id)
    
    Me.Produkty_w_koszyku_podformularz.Requery
End Sub
