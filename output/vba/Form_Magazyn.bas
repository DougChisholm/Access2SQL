VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Form_Magazyn"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Compare Database

Private Sub error_Click()
    If Not IsNull(Me.lista) Then
        Dim id As String
        id = Me.lista.Value
        DoCmd.RunSQL ("INSERT INTO Stan_Magazynu VALUES (" & id & ", 0)")
        Me.Requery
        Me.lista.Requery
        lista_AfterUpdate
    End If
End Sub

Private Sub kwerenda_niedostepn_Click()
     DoCmd.OpenQuery "Zmieñ dostêpnoœæ produktów", acViewNormal
End Sub

Private Sub lista_AfterUpdate()
    Dim id As String
    id = Me.lista.Value
    
    Dim ilosc As String
    
    Dim strSQL As String
    strSQL = "SELECT ilosc FROM Stan_Magazynu WHERE id_produktu = " & id

    Dim rs As DAO.Recordset
    Set rs = CurrentDb.OpenRecordset(strSQL)

    If Not rs.EOF Then
        ilosc = rs("ilosc").Value
    End If
    
    Me.ilosc.Value = ilosc
    
    If ilosc = "" Then
        Me.error.Visible = True
        Me.ustaw.Enabled = False
    Else
        Me.error.Visible = False
        Me.ustaw.Enabled = True
    End If
End Sub

Private Sub ustaw_Click()
    Dim id As String
    Dim ilosc As String
    
    If Not IsNull(Me.ilosc) Then
        id = Me.lista.Value
        
        ilosc = Me.ilosc.Value
        DoCmd.RunSQL ("UPDATE Stan_Magazynu SET ilosc = " & ilosc & " WHERE id_produktu = " & id)
        Me.Requery
        lista_AfterUpdate
    End If
End Sub
