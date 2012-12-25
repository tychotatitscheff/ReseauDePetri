#Region "Importations namespaces"
Imports System
Imports System.ComponentModel 'Importation necessaire à l'utilisation des bindingList
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Xml.Serialization
#End Region
''' <summary>
''' Cette classe réalise à la fois la vue et à la fois le controlleur principale de l'apllication.
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
    Private Sub Maj_Treeview()
        TV_Donnée.Nodes.Clear()
        TV_Donnée.Nodes.Add(ReseauDePetri.RetournerEquivalenceTreeNode())
        TV_Donnée.ExpandAll()
    End Sub
#End Region
#Region "Mise à jour"
    Private Sub MAJ_DataBindingList(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load, B_AddPlace.Click, B_AddTrans.Click
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
        If CB_Sens.SelectedItem.ToString = "P -> T" Then
            sens = Arc.E_Sens.PlaceVersTransition
            ReseauDePetri.EnvoyerReseauChange("L'arc de multiplicité " & TB_mult.Text & " reliant la place '" & CType(CB_place.SelectedItem, Place).nom & "' à la transition '" & CType(CB_trans.SelectedItem, Transition).nom & "' a été ajoutée au réseau." & vbCrLf, Color.Black)
        ElseIf CB_Sens.SelectedItem.ToString = "T -> P" Then
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
        ReseauDePetri.GenererEtat()
        RTB_Console.AppendText(vbCrLf)
        RTB_Console.SelectionIndent = 20
        ReseauDePetri.Maj_TransitionValidable()
        RTB_Console.AppendText(vbCrLf)
        While ReseauDePetri.TableauTransitionValidable.Count > 0
            ReseauDePetri.ValiderTransition(ReseauDePetri.ChoixHasard_TransitionValidable())
            RTB_Console.AppendText(vbCrLf)
            ReseauDePetri.Maj_TransitionValidable()
            RTB_Console.AppendText(vbCrLf)
        End While
        RTB_Console.SelectionIndent = 0
        ReseauDePetri.GenererEtat()
        RTB_Console.AppendText(vbCrLf)
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
    End Sub
    Private Sub SauvegarderLaSimulationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SauvegarderLaSimulationToolStripMenuItem.Click
        Dim path As String = "serialisation.srp"
        Serialisation(path)
    End Sub

    Private Sub ChargerSimulationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChargerSimulationToolStripMenuItem.Click
        Dim path As String = "serialisation.srp"
        DeSerialisation(path)
        Maj_Treeview()
        ReseauDePetri.EnvoyerReseauChange("Le fichier " & path & " a été convenablement chargé." & vbCrLf)
        ReseauDePetri.GenererEtat()
    End Sub
#End Region
#Region "Serialisation et déserialisation"
    Public Sub Serialisation(ByVal _path As String)
        Dim xmlFichier As FileStream = File.Create("serialise.xml")
        Dim bf_reseau As XmlSerializer = New XmlSerializer(GetType(Reseau))
        bf_reseau.Serialize(xmlFichier, ReseauDePetri)
        xmlFichier.Close()
    End Sub
    Public Sub DeSerialisation(ByVal _path As String)
        Dim bf_reseau As XmlSerializer = New XmlSerializer(GetType(Reseau))
        Dim xmlFichier As FileStream = File.Open("serialise.xml", FileMode.Truncate, FileAccess.ReadWrite)
        Dim ReseauDePetri As Reseau = CType(bf_reseau.Deserialize(xmlFichier), Reseau)
        xmlFichier.Close()
    End Sub
#End Region
End Class