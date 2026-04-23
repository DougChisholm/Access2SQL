VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Form_Generator"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Compare Database


Function RandomDate(startDate As Date, endDate As Date) As Date
    Dim numDays As Long
    Dim randomDays As Long
    
    
    Randomize

    numDays = endDate - startDate
    
    randomDays = Int((numDays + 1) * Rnd)
    
    RandomDate = startDate + randomDays
End Function

Function RandomGender() As String
    Dim randomIndex As Integer
    
    Randomize
    
    randomIndex = Int(2 * Rnd) '0 lub 1
    
    If randomIndex = 0 Then
        RandomGender = "M"
    Else
        RandomGender = "K"
    End If
    
End Function

Function RandomName(gender As String) As String
    Dim db As DAO.Database
    Dim rs As DAO.Recordset
    Dim strSQL As String
    Dim selectedName As String
    
    Set db = CurrentDb
    
    ' random name
    If gender = "K" Then
        strSQL = "SELECT TOP 1 imie FROM imiona_damskie ORDER BY Rnd(id)"
    Else
        strSQL = "SELECT TOP 1 imie FROM imiona_meskie ORDER BY Rnd(id)"
    End If
    
    Set rs = db.OpenRecordset(strSQL)
    
    If Not rs.EOF Then
        selectedName = rs.Fields("imie").Value
    End If
    
    rs.Close
    Set rs = Nothing
    db.Close
    Set db = Nothing
    
    ' return the random name
    RandomName = selectedName
End Function

Function RandomSurname(ByVal gender As String) As String
    Dim db As DAO.Database
    Dim rs As DAO.Recordset
    Dim strSQL As String
    Dim selectedSurname As String
    
    Set db = CurrentDb
    
    ' random surname
    If gender = "K" Then
        strSQL = "SELECT TOP 1 nazwisko FROM nazwiska_damskie ORDER BY Rnd(id)"
    Else
        strSQL = "SELECT TOP 1 nazwisko FROM nazwiska_meskie ORDER BY Rnd(id)"
    End If
    
    Set rs = db.OpenRecordset(strSQL)
    
    If Not rs.EOF Then
        selectedSurname = rs.Fields("nazwisko").Value
    End If
    
    rs.Close
    Set rs = Nothing
    db.Close
    Set db = Nothing
    
    ' return the random surname
    RandomSurname = selectedSurname
    
End Function

Function RandomNumber(min As Integer, max As Integer) As Integer
    Randomize
    RandomNumber = Int((max - min + 1) * Rnd) + min
End Function

Function RandomPESEL(bDate As Date, gender As String) As String
    Dim numbers(10) As Integer
    numbers(0) = 1
    numbers(1) = 3
    numbers(2) = 7
    numbers(3) = 9
    numbers(4) = 1
    numbers(5) = 3
    numbers(6) = 7
    numbers(7) = 9
    numbers(8) = 1
    numbers(9) = 3
    
    Dim i As Integer
    Dim controlNumber As Integer
    Dim pesel As String
    pesel = Format(Year(bDate) Mod 100, "00") + Format(Month(bDate), "00") + Format(Day(bDate), "00")
    
    For i = 1 To 3
        pesel = pesel + CStr(RandomNumber(0, 9))
    Next
    
    If gender = "K" Then
        'female
        pesel = pesel + CStr(RandomNumber(0, 4) * 2)
    Else
        'male
        pesel = pesel + CStr(RandomNumber(0, 4) * 2 + 1)
    End If
    
    For i = 0 To 9
        controlNumber = controlNumber + CInt(Mid(pesel, i + 1, 1)) * numbers(i)
    Next
    
    pesel = pesel + CStr((10 - controlNumber Mod 10) Mod 10)
    
    
    
    RandomPESEL = pesel
End Function



Private Sub debug_del_Click()
    DoCmd.RunSQL ("DELETE FROM Produkty_zamowienia")
    DoCmd.RunSQL ("DELETE FROM Zamowienia")
    DoCmd.RunSQL ("DELETE FROM Adresy")
    DoCmd.RunSQL ("DELETE FROM Klienci")
    
End Sub

Private Sub Form_Activate()
    Me.Requery
    Me.Tekst28.Requery
End Sub

Private Sub AddRandomOrders(ByVal data_urodzenia As Date, id_klienta As String, rs_products As DAO.Recordset, rs_orders As DAO.Recordset, rs_productOrders As DAO.Recordset)
    Dim i As Integer
    Dim j As Integer
    Dim products As Integer
    Dim total_price As Double
    Dim count As Integer
    Dim price As Double
    Dim id_zamowienia As String
    Dim maxProductCount As Integer
    
    'count of products
    rs_products.MoveLast
    products = rs_products.RecordCount
    
    maxProductCount = 3
    
    If Year(Now) - Year(data_urodzenia) < 30 Then maxProductCount = 5
    
    If Year(Now) - Year(data_urodzenia) > 48 Then maxProductCount = 2
    
    
    'make random orders
    For i = 1 To RandomNumber(1, 6)
        rs_orders.AddNew
        rs_orders.Fields("id_klienta").Value = id_klienta
        rs_orders.Fields("data").Value = RandomDate(DateAdd("yyyy", 10, data_urodzenia), Now)
        
        id_zamowienia = rs_orders.Fields("id_zamowienia").Value
        
        rs_orders.Update
        
        total_price = 0
        
        'for each order, pick random count of random products
        For j = 1 To RandomNumber(1, maxProductCount)
            rs_products.MoveFirst
            rs_products.Move RandomNumber(0, products - 1)
            
            rs_productOrders.AddNew
            rs_productOrders.Fields("id_zamowienia").Value = id_zamowienia
            rs_productOrders.Fields("id_produktu").Value = rs_products.Fields("id_produktu").Value
            
            count = RandomNumber(1, 2)
            rs_productOrders.Fields("ilosc").Value = count
            
            price = CDbl(rs_products.Fields("cena").Value)
            rs_productOrders.Fields("cena_jednostkowa").Value = price
            total_price = total_price + price
            
            rs_productOrders.Update
            
        Next
        
        rs_orders.FindFirst ("id_zamowienia = " & id_zamowienia)
        
        rs_orders.Edit
        
        rs_orders.Fields("cena").Value = CStr(Round(total_price, 2))
        
        rs_orders.Update
        
    Next
End Sub

Private Sub AddRandomAddress(id_klienta As String, rs_address As DAO.Recordset)

    'pick a random sample address
    Dim strSQL As String
    Dim db As DAO.Database
    Dim rs As DAO.Recordset
    
    Dim kraj As String
    Dim miasto As String
    Dim kod As String
    Dim ulica As String
    Dim nr_domu As String
    
    Set db = CurrentDb
    
    strSQL = "SELECT TOP 1 * FROM przykladowe_adresy ORDER BY Rnd(id_adresu)"
    
    Set rs = db.OpenRecordset(strSQL)
    
    If Not rs.EOF Then
        kraj = rs.Fields("kraj").Value
        miasto = rs.Fields("miasto").Value
        kod = rs.Fields("kod_pocztowy").Value
        ulica = rs.Fields("ulica").Value
        nr_domu = rs.Fields("nr_domu").Value
        
    End If
    
    rs.Close
    Set rs = Nothing
    db.Close
    Set db = Nothing
    
    'add this address to addresses table
    
    rs_address.AddNew
    rs_address("id_klienta") = id_klienta
    rs_address("kraj") = kraj
    rs_address("miasto") = miasto
    rs_address("kod_pocztowy") = kod
    rs_address("ulica") = ulica
    rs_address("nr_domu") = nr_domu
    
    rs_address.Update
End Sub

Private Sub AddRandomCustomer(rs As DAO.Recordset, rs_address As DAO.Recordset, rs_products As DAO.Recordset, rs_orders As DAO.Recordset, rs_productOrders As DAO.Recordset)
    Dim id_klienta As String
    Dim birthDate As Date
    birthDate = RandomDate(DateSerial(1950, 1, 1), DateSerial(2002, 12, 1))
    
    Dim gender As String
    gender = RandomGender
    
    Dim name As String
    name = RandomName(gender)
    
    Dim surname As String
    surname = RandomSurname(gender)
    
    Dim pesel As String
    pesel = "12345678901"
    
    rs.AddNew
    
    rs("imie") = name
    rs("nazwisko") = surname
    rs("plec") = gender
    rs("data_urodzenia") = birthDate
    rs("pesel") = RandomPESEL(birthDate, gender)
    
    id_klienta = rs("id_klienta").Value
    
    
    rs.Update
    
    AddRandomAddress id_klienta, rs_address
    
    AddRandomOrders birthDate, id_klienta, rs_products, rs_orders, rs_productOrders
    
    
    
End Sub



Private Sub Polecenie52_Click()
    ' generate customers with their data
    If Not IsNull(Me.ilosc) Then
        Me.Polecenie52.Enabled = False
        
        Dim count As Long
        count = Me.ilosc.Value
        
        Dim answer As Integer
        answer = vbYes
        
        If count > 10000 Then
            MsgBox ("Too much customers. Maximum is 10000")
            answer = vbNo
        ElseIf count > 2000 Then
            answer = MsgBox("Generating such large count of customers might take some time. Continue?", vbQuestion + vbYesNo + vbDefaultButton2, "Uwaga!")
        End If
        
        
        If count > 0 And answer = vbYes Then
            Me.loading.Caption = "Generating..."
            Me.loading.Visible = True
            
            DoEvents
            
            
            Dim db As DAO.Database
            Dim rs_client As DAO.Recordset
            Dim rs_address As DAO.Recordset
            Dim rs_products As DAO.Recordset
            Dim rs_orders As DAO.Recordset
            Dim rs_productOrders As DAO.Recordset
            
            Set db = CurrentDb
            Set rs_client = db.OpenRecordset("Klienci", dbOpenDynaset)
            Set rs_address = db.OpenRecordset("Adresy", dbOpenDynaset)
            Set rs_products = db.OpenRecordset("Produkty", dbOpenDynaset)
            Set rs_orders = db.OpenRecordset("Zamowienia", dbOpenDynaset)
            Set rs_productOrders = db.OpenRecordset("Produkty_zamowienia", dbOpenDynaset)
            
            
            
            Dim i As Integer
            
            
            For i = 1 To count
            'new customer
                AddRandomCustomer rs_client, rs_address, rs_products, rs_orders, rs_productOrders
                
                If i Mod 100 = 0 Then
                    Me.loading.Caption = "Generowanie... " + CStr(Round(i / count * 100)) + "%"
                    DoEvents
                End If
                
            Next
            
            Me.loading.Visible = False
            
            'close all the database objects
            rs_client.Close
            rs_address.Close
            rs_products.Close
            rs_orders.Close
            rs_productOrders.Close
            db.Close
            
            Set db = Nothing
            Set rs_client = Nothing
            Set rs_address = Nothing
            Set rs_products = Nothing
            Set rs_orders = Nothing
            Set rs_productOrders = Nothing
            
        
        End If
        
        Me.Polecenie52.Enabled = True
    End If
End Sub
