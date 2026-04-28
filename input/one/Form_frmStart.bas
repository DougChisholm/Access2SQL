VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Form_frmStart"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Compare Database
Option Explicit
'---------------------------------------------------------------------------------
'This is the menu form, with just two buttons:
  'New Game - recreates bingo table from the back-up master and starts the game
  'Finish Playing - to close this form
  '----------------------------------------------------------------------------------

Private Sub cmdNewGame_Click()
'wizard code to run the make-table query
'amended to delete the table, open the game form, and close this form.

On Error GoTo Err_cmdNewGame_Click

    Dim stDocName As String
        
    DoCmd.RunSQL ("DROP TABLE tblBingo")      'this uses SQL to delete the old bingo table
    'DoCmd.DeleteObject acTable, "tblBingo"   'this is an alternative way to delete the old bingo table
    
    'run the query to make a new table from the back up - this is the wizard code
    'at Database window, use Tools-->Options, Edit/Find tab, Confirm section, to suppress action messages
    stDocName = "qryMakeNewBingoTable"
    DoCmd.OpenQuery stDocName, acNormal, acEdit
      
    'open the bingo form, and close this form
    DoCmd.OpenForm "frmBingo"
    DoCmd.Close acForm, "frmStart"

Exit_cmdNewGame_Click:
    Exit Sub

Err_cmdNewGame_Click:
    MsgBox Err.Description
    Resume Exit_cmdNewGame_Click
    
End Sub

'------------------------------------------------------------------------------
Private Sub cmdFinish_Click()
'wizard code to close this form

On Error GoTo Err_cmdFinish_Click


    DoCmd.Close

Exit_cmdFinish_Click:
    Exit Sub

Err_cmdFinish_Click:
    MsgBox Err.Description
    Resume Exit_cmdFinish_Click
    
End Sub

'==========END OF CODE===================

