#Region "Importations namespaces"
Imports System
Imports System.ComponentModel 'Importation necessaire à l'utilisation des bindingList
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Xml
#End Region
''' <summary>
''' Cette classe réalise à la fois la vue et à la fois le controlleur principale de l'application.
''' </summary>
''' <remarks></remarks>
Public Class Main
#Region "Attributs privés"
    Private WithEvents ReseauDePetri As New Reseau()
#End Region
#Region "Gestion du log"
    Private Sub AffichageRTB(ByVal sender As Object, ByVal e As ChangementReseauEventArgs) Handles ReseauDePetri.ReseauChange
        RTB_Console.SelectionColor = e.Couleur
        RTB_Console.AppendText(e.Text)
    End Sub
#End Region
#Region "Gestion Treeview"
    Private Sub Maj_Treeview() Handles ChB_verbose.CheckedChanged
        TV_Donnée.Nodes.Clear()
        TV_Donnée.Nodes.Add(ReseauDePetri.RetournerEquivalenceTreeNode())
        TV_Donnée.ExpandAll()
    End Sub
#End Region
#Region "Mise à jour"
    Public Delegate Sub MajDataBindingEventHandler(ByVal sender As Object, ByVal e As MajDataBindingEventArgs)
    Public Event MajDataBinding As MajDataBindingEventHandler
    Private Sub MAJ_DataBindingList(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load, B_AddPlace.Click, B_AddTrans.Click, Me.MajDataBinding
        Me.CB_place.DataSource = ReseauDePetri.TableauPlace
        Me.CB_trans.DataSource = ReseauDePetri.TableauTransition
    End Sub
#End Region
#Region "Entrée des paramètres"
    Private Sub B_AddPlace_Click(ByVal sender As Object, ByVal e As EventArgs) Handles B_AddPlace.Click
        ReseauDePetri.TableauPlace.Add(New Place(CUInt(TB_Place_Nombre.Text), TB_Place_Nom.Text))
        ReseauDePetri.EnvoyerReseauChange("La place '" & TB_Place_Nom.Text & "' contenant " & TB_Place_Nombre.Text & " jeton(s) a été ajoutée au réseau." & vbCrLf)
        Maj_Treeview()
    End Sub
    Private Sub B_AddTrans_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles B_AddTrans.Click
        ReseauDePetri.TableauTransition.Add(New Transition(TB_Trans_Nom.Text))
        ReseauDePetri.EnvoyerReseauChange("La transition '" & TB_Trans_Nom.Text & "' a été ajoutée au réseau." & vbCrLf)
        Maj_Treeview()
    End Sub
    Private Sub B_AddArc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles B_AddArc.Click
        Dim sens As Arc.E_Sens
        If CB_sens.SelectedItem.ToString = "P -> T" Then
            sens = Arc.E_Sens.PlaceVersTransition
            ReseauDePetri.EnvoyerReseauChange("L'arc de multiplicité " & TB_mult.Text & " reliant la place '" & CType(CB_place.SelectedItem, Place).nom & "' à la transition '" & CType(CB_trans.SelectedItem, Transition).nom & "' a été ajoutée au réseau." & vbCrLf, Color.Black)
        ElseIf CB_sens.SelectedItem.ToString = "T -> P" Then
            sens = Arc.E_Sens.TransitionVersPlace
            ReseauDePetri.EnvoyerReseauChange("L'arc de multiplicité " & TB_mult.Text & "reliant la transition '" & CType(CB_trans.SelectedItem, Transition).nom & "' à la place '" & CType(CB_place.SelectedItem, Place).nom & "' a été ajoutée au réseau." & vbCrLf, Color.Black)
        End If
        If CUInt(TB_mult.Text) > 1 Then
            ReseauDePetri.TableauArc.Add(New Arc(CType(CB_place.SelectedItem, Place), CType(CB_trans.SelectedItem, Transition), sens, CUInt(TB_mult.Text)))

        Else
            ReseauDePetri.TableauArc.Add(New Arc(CType(CB_place.SelectedItem, Place), CType(CB_trans.SelectedItem, Transition), sens))
        End If
        Maj_Treeview()
    End Sub
#End Region
#Region "Simulation"
    Private Sub B_Go_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles B_Go.Click, LancerLaSimulationToolStripMenuItem.Click
        RTB_Console.AppendText(vbCrLf & "**** Début de la simulation ****" & vbCrLf)
        ReseauDePetri.GenererEtat("Initial")
        RTB_Console.AppendText(vbCrLf)
        RTB_Console.SelectionIndent = 20
        ReseauDePetri.Maj_TransitionValidable()
        Dim index As Integer = 1
        While ReseauDePetri.TableauTransitionValidable.Count > 0 And index < CInt(TB_etapeMax.Text) + 1
            ReseauDePetri.ValiderTransition(ReseauDePetri.ChoixHasard_TransitionValidable())
            ReseauDePetri.Maj_TransitionValidable()
            index += 1
        End While
        RTB_Console.AppendText("La simulation a effectué " & CStr(index - 1) & " étapes" & vbCrLf)
        RTB_Console.SelectionIndent = 0
        RTB_Console.AppendText(vbCrLf)
        ReseauDePetri.GenererEtat("Final")
        RTB_Console.AppendText("**** Fin de Simulation ****" & vbCrLf)
    End Sub
#End Region
#Region "Evenement fenetre"
    Private Sub On_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ReseauDePetri = New Reseau()
        RTB_Console.Clear()
        ReseauDePetri.EnvoyerReseauChange("Le réseau a été remis à zéro." & vbCrLf, Color.LimeGreen)
        Maj_Treeview()
        TV_Donnée.ExpandAll()
    End Sub
#End Region
#Region "Interface menu"
    Private Sub QuitterToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QuitterToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub NouvelleSimulationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NouvelleSimulationToolStripMenuItem.Click
        ReseauDePetri = New Reseau()
        RTB_Console.Clear()
        ReseauDePetri.EnvoyerReseauChange("Le réseau a été remis à zéro." & vbCrLf, Color.LimeGreen)
        Maj_Treeview()

        CB_place.DataSource = Nothing
        CB_place.Items.Clear()
        CB_place.DataSource = ReseauDePetri.TableauPlace
        CB_trans.DataSource = Nothing
        CB_trans.Items.Clear()
        CB_trans.DataSource = ReseauDePetri.TableauTransition
        RaiseEvent MajDataBinding(Me, New MajDataBindingEventArgs())
    End Sub
    Private Sub SauvegarderLaSimulationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SauvegarderLaSimulationToolStripMenuItem.Click
        SFD_enregistrement.ShowDialog()
        Dim Path As String = SFD_enregistrement.InitialDirectory + SFD_enregistrement.FileName
        If Path <> "" Then
            Serialisation(Path)
            ReseauDePetri.EnvoyerReseauChange("Le fichier " & Path & " a été convenablement sauvegardé." & vbCrLf)
        Else
            ReseauDePetri.EnvoyerReseauChange("Sauvegarde annulée." & vbCrLf, Color.Maroon)
        End If
    End Sub

    Private Sub ChargerSimulationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChargerSimulationToolStripMenuItem.Click
        OFD_chargement.ShowDialog()
        Dim Path As String = OFD_chargement.InitialDirectory + OFD_chargement.FileName
        If Path <> "" Then
            DeSerialisation(Path)
            Maj_Treeview()
            ReseauDePetri.EnvoyerReseauChange("Le fichier " & Path & " a été convenablement chargé." & vbCrLf, Color.Maroon)
            ReseauDePetri.GenererEtat()
            ReseauDePetri.EnvoyerReseauChange("**** Fin de l'importation ****" & vbCrLf, Color.Maroon)
            RaiseEvent MajDataBinding(Me, New MajDataBindingEventArgs())
        Else
            ReseauDePetri.EnvoyerReseauChange("Importation annulée." & vbCrLf, Color.Maroon)
        End If
    End Sub
    Private Sub SauvegarderLesDonnéesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SauvegarderLesDonnéesToolStripMenuItem.Click

    End Sub
#End Region
#Region "Serialisation et déserialisation"
    Public Sub Serialisation(ByVal _path As String)
        Dim pref As New XmlWriterSettings()
        pref.Indent = True
        pref.IndentChars = vbTab
        pref.NamespaceHandling = NamespaceHandling.OmitDuplicates
        pref.OmitXmlDeclaration = False
        Dim xmlFichier As XmlWriter = XmlWriter.Create(_path, pref)
        Dim bf_reseau As DataContractSerializer = New DataContractSerializer(GetType(Reseau))
        bf_reseau.WriteObject(xmlFichier, ReseauDePetri)
        xmlFichier.Close()
    End Sub
    Public Sub DeSerialisation(ByVal _path As String)
        Dim bf_reseau As DataContractSerializer = New DataContractSerializer(GetType(Reseau))
        Try
            Dim xmlFichier As FileStream = New FileStream(_path, FileMode.Open, FileAccess.Read)
            xmlFichier.Seek(0, IO.SeekOrigin.Begin)
            ReseauDePetri = CType(bf_reseau.ReadObject(xmlFichier), Reseau)
            xmlFichier.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
#End Region
End Class