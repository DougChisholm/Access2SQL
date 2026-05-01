VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Form_ChooseTest"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Compare Database
Option Explicit

'--------------------------------------
Private Sub cmdEnterResults_Click()
'wizard code to open form
'no filter used as StudentResult form is based on query
'     which picks up value from this form
'   - this way can be extended to allow for other values

On Error GoTo Err_cmdEnterResults_Click

    Dim stDocName As String
    Dim stLinkCriteria As String

    stDocName = "StudentResult"
    
    DoCmd.OpenForm stDocName, , , stLinkCriteria

Exit_cmdEnterResults_Click:
    Exit Sub

Err_cmdEnterResults_Click:
    MsgBox Err.Description
    Resume Exit_cmdEnterResults_Click
    
End Sub

'---------------------------------------
Private Sub lstTest_Click()
'enable button when user has chosen a test

    Me!cmdEnterResults.Enabled = True
End Sub
