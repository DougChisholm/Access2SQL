VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Form_frmBingo"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Compare Database
Option Explicit
'---------------------------------------------------------------------
'Conditional formatting has been used to set font and background to grey
    'when a form cell is zero, thus 'removing' the number
    
'This is the form where the entire game is coded and takes place
'A random number is calculated each time the 'Get Number' button is clicked.
'this form is closed, and the Start form opened, when the user clicks the Finish button.
    
'---------------------------------------------------------------------------
'declare variables for the bingo form layout
'numbers run from 1 to 100, with 10 in a row
'tblBingo matches this layout
Const intLower = 1
Const intUpper = 100
'Const intUpper = 5                      'small number for testing
Const intRowSize = 10
Dim intBingoArray(intLower To intUpper) As Integer      'the full range of numbers for the game
                            'elements set to 1 when the corresponding number has been found

'variables for the SQL command
Dim strSQL As String                                     'for SQL used to update tblBingo
Dim strColNo As String                                   'used with SQL for the name of the column to be updated
Dim strColArray(1 To intRowSize) As String      'names of the columns for tblBingo rows for SQL
Dim intColArrayCounter As Integer                   'counter for strColArray

'various counters
Dim intNextNo As Integer                                'the next random number found
Dim intColNo As Integer                                  'the column number corresponding to the random number
Dim intTotalNumbers As Integer                       'counter for the the numbers found so far - used to stop processing
Dim boolNumberFound As Boolean                  'used to indicate when a number is found, to stop loop

'------------------------------------------------------------------------
Private Sub cmdGetNumber_Click()
'user clicks on this button to get each number, one at a time

If intTotalNumbers = intUpper Then          'all numbers have been found - end of game
    MsgBox ("All numbers taken")
Else
    
    'find the next number
    boolNumberFound = False                                                 'not found yet
    While Not boolNumberFound
    
        intNextNo = Int(((intUpper - intLower + 1) * Rnd) + intLower)        'get next number
        If intBingoArray(intNextNo) = 0 Then                                'if it is a new number
        
            intBingoArray(intNextNo) = 1                                      'set element to 1 so don't choose this again
            intTotalNumbers = intTotalNumbers + 1                       'add 1 to numbers found
            lstNumbers.RowSource = intNextNo & ";" & lstNumbers.RowSource  'add to list of numbers
            boolNumberFound = True                                           'have found a number, so can end the loop
            MsgBox ("your next number is: " & intNextNo)             'tell user what the number is
            
            'calculate col no - e.g. number = 16 means col =6, number = 30 means col = 30
            intColNo = intNextNo Mod intRowSize                         'mod function will give the remainder of the division, 0-9
            If intColNo = 0 Then                                                    'change 0 to 10
                intColNo = intRowSize
            End If    'intColNo = 0

            'UPDATE tblBingo SET Colx = 0 WHERE Colx = number
            'this SQL will find the number and set it to zero
            strSQL = "UPDATE tblBingo " _
                           & "SET " _
                           & strColArray(intColNo) _
                           & " = 0  " _
                           & "WHERE " _
                           & strColArray(intColNo) _
                           & " = " _
                           & intNextNo _
                           & ";"
        
                DoCmd.RunSQL strSQL                                           'run the SQL and update the table
            
                'redraw the form - the conditional formatting will blank out the cells for the chosen numbers
                Me.Repaint
        
            End If     'intBingoArray(intNextNo) = 0
        Wend     'Not boolNumberFound
   End If     'intTotalNumbers = intUpper
    
End Sub

'-------------------------------------------------------------------------------
Private Sub Form_Load()
'code to be executed at start of game

    'initialise col name array
    For intColArrayCounter = 1 To intRowSize
        strColArray(intColArrayCounter) = "Col" & intColArrayCounter
    Next intColArrayCounter
    
    intTotalNumbers = 0           'no numbers found yet
    
    Randomize                        'seed the random number function
    
End Sub

'--------------------------------------------------------------------------------------------
Private Sub cmdFinish_Click()
On Error GoTo Err_cmdFinish_Click
'wizard close code, changed

    'DoCmd.Close      'this was the wizard code - replaced by code to reference specific forms
    DoCmd.OpenForm "frmStart"                              'open the start form
    DoCmd.Close acForm, "frmBingo"                      'and close this form

Exit_cmdFinish_Click:
    Exit Sub

Err_cmdFinish_Click:
    MsgBox Err.Description
    Resume Exit_cmdFinish_Click
    
End Sub

'==========END OF CODE================
