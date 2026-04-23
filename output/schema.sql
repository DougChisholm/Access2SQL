TABLE: Adresy
  id_adresu
  id_klienta
  kraj
  miasto
  kod_pocztowy
  ulica
  nr_domu

TABLE: imiona_damskie
  id
  imie

TABLE: imiona_meskie
  id
  imie

TABLE: Kategorie
  id_kategorii
  nazwa

TABLE: Klienci
  id_klienta
  imie
  nazwisko
  data_urodzenia
  plec
  pesel

TABLE: Koszyk
  id_klienta
  id_produktu

TABLE: nazwiska_damskie
  id
  nazwisko

TABLE: nazwiska_meskie
  id
  nazwisko

TABLE: Produkty
  id_produktu
  nazwa
  opis
  cena
  gwarancja
  dostepny

TABLE: Produkty_Kategorie
  id_produktu
  id_kategorii

TABLE: Produkty_zamowienia
  id_zamowienia
  id_produktu
  ilosc
  cena_jednostkowa

TABLE: przykladowe_adresy
  id_adresu
  id_klienta
  kraj
  miasto
  kod_pocztowy
  ulica
  nr_domu

TABLE: Stan_Magazynu
  id_produktu
  ilosc

TABLE: USysApplicationLog
  ID
  SourceObject
  Data Macro Instance ID
  Error Number
  Category
  Object Type
  Description
  Context
  Created

TABLE: Zamowienia
  id_zamowienia
  id_klienta
  data
  cena
  kod_promocyjny
  notatki

