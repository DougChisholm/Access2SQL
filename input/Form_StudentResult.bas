VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Form_StudentResult"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Compare Database
Option Explicit

'--------------------------------------
Private Sub Form_Current()
'move the cursor the the mark field for each new rec

    Mark.SetFocus
End Sub

'------------------------------------------------
Private Sub Mark_BeforeUpdate(Cancel As Integer)
'when a mark is entered, copy testid from ChooseTest form
'   to the current record

    TestId = Forms!ChooseTest!LstTest
End Sub
