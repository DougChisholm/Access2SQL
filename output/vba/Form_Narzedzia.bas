VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Form_Narzedzia"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Compare Database

Private Sub Form_Activate()
    Me.Requery
    Me.Tekst28.Requery
    
    'check the local number separator
    Dim testValue As Single
    Dim decimalSeparator As String
    
    testValue = 0.1
    decimalSeparator = Mid(CStr(testValue), 2, 1)
    
    If decimalSeparator <> "," Then Me.separator_label.Visible = False
End Sub

Private Sub displayValues(pln As Double, euro As Double)
    Me.pln.Value = pln
    Me.euro.Value = euro
End Sub


Private Sub Polecenie45_Click()
    Dim pln As Double
    Dim euro As Double
    Dim rate As Double
    
    If Not IsNull(Me.pln) Then
        rate = 0.2235
        pln = Me.pln.Value
        euro = Round(pln * rate, 2)
        
        displayValues pln, euro
        
    End If
End Sub

Private Sub Polecenie46_Click()
    Dim pln As Double
    Dim euro As Double
    Dim rate As Double
    
    If Not IsNull(Me.euro) Then
        rate = 4.48
        euro = Me.euro.Value
        pln = Round(euro * rate, 2)
        
        displayValues pln, euro
        
    End If
End Sub
