VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Form_Obnizka Ceny"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Compare Database

Private Function CenaProduktu() As Double
    Dim cena As Double
    cena = 0
    
    Dim id As String
    id = Me.Lista35.Value
    
    Dim strSQL As String
    strSQL = "SELECT cena FROM Produkty WHERE id_produktu = " & id

    Dim rs As DAO.Recordset
    Set rs = CurrentDb.OpenRecordset(strSQL)

    If Not rs.EOF Then
        cena = rs("cena").Value
    End If
    
    CenaProduktu = cena
    
End Function

Private Sub resetInput()
    Me.obnizka.Value = ""
    Me.ost_cena.Value = ""
    
    Dim cena As Double
    cena = CenaProduktu()
    
    Me.pierw_cena.Value = cena
End Sub

Private Sub Lista35_AfterUpdate()
    Me.wybierz_produkt.Visible = False
    resetInput
End Sub


Private Sub obnizka_AfterUpdate()
    If Not IsNull(Me.Lista35) Then
        Dim cena As Double
        cena = CenaProduktu()
        
        Dim obnizka As Double
        
        If IsNull(Me.obnizka) Then
            obnizka = 0
        Else
            obnizka = Me.obnizka.Value
        End If
        
        Dim cena2 As Double
        cena2 = Round(cena * (1 - obnizka), 2)
        
        Me.ost_cena.Value = cena2
    End If
    

End Sub

Private Sub zastosuj_Click()
    Dim id As String
    Dim newPrice As String
    If (Not IsNull(Me.Lista35)) And (Me.ost_cena.Value <> "") Then
        newPrice = ost_cena.Value
        newPrice = Replace(newPrice, ",", ".")
        
        id = Me.Lista35.Value
        
        
        DoCmd.RunSQL ("UPDATE Produkty SET cena = " & newPrice & " WHERE id_produktu = " & id)
        
        Me.Requery
        Me.Lista35.Requery
        resetInput
    End If
End Sub
