Imports System.ComponentModel 'Importation necessaire à l'utilisation des bindingList
Imports System.Xml.Serialization
''' <summary>
''' La classe réseau rassemble les différents éléments du réseau. Elle comprend donc les listes de place, de transition et d'arc ainsi que les différentes méthodes permettant de mettre à jour le réseau, de selectionner une transition au hasard et de la valider.
''' Elle comprend également la gestion des évenements pour afficher le processus de simulation.
''' </summary>
''' <remarks>Pour plus d'information sur la partie théorique : http://fr.wikipedia.org/wiki/R%C3%A9seau_de_Petri </remarks>
<Serializable()>
Public Class Reseau
#Region "Attributs privés"
    ''' <remarks>L'utilisation de BindingList se justifie pour syncroniser la liste des places avec le combobox de la fenètre principale</remarks>
    <XmlElement("Tableau des places")> Private T_Place As New BindingList(Of Place)
    ''' <remarks>L'utilisation de BindingList se justifie pour syncroniser la liste des transition avec le combobox de la fenètre principale</remarks>
    <XmlElement("Tableau des transitions")> Private T_Transition As New BindingList(Of Transition)
    <XmlElement("Tableau des arcs")> Private T_Arc As New List(Of Arc)
#Region "Attributs necessitant une fonction de mise à jour"
    <NonSerialized()> Private T_TransitionValidable As New List(Of Transition)
    <NonSerialized()> Private T_ArcRentrant As New List(Of Arc)
    <NonSerialized()> Private T_ArcSortant As New List(Of Arc)
#End Region
#End Region
#Region "Constructeur"
    Public Sub New()

    End Sub
#End Region
#Region "Gestion évenements"
    Public Delegate Sub ReseauChangeEventHandler(ByVal sender As Object, ByVal e As ChangementReseauEventArgs)
    <NonSerialized()> Public Event ReseauChange As ReseauChangeEventHandler
    Public Sub EnvoyerReseauChange(ByVal texte As String, ByVal couleur As Color)
        Dim e As ChangementReseauEventArgs = New ChangementReseauEventArgs(texte, couleur)
        If Not (e Is Nothing) Then
            'L'évenement est envoyé
            RaiseEvent ReseauChange(Me, e)
        End If
    End Sub
    Public Sub EnvoyerReseauChange(ByVal texte As String)
        Dim black As Color = Color.Black
        Dim e As ChangementReseauEventArgs = New ChangementReseauEventArgs(texte, black)
        If Not (e Is Nothing) Then
            'L'évenement est envoyé
            RaiseEvent ReseauChange(Me, e)
        End If
    End Sub
#End Region
#Region "Properties"
    Public Property TableauPlace() As BindingList(Of Place)
        Get
            Return T_Place
        End Get
        Set(ByVal value As BindingList(Of Place))
            T_Place = value
        End Set
    End Property
    Public Property TableauTransition() As BindingList(Of Transition)
        Get
            Return T_Transition
        End Get
        Set(ByVal value As BindingList(Of Transition))
            T_Transition = value
        End Set
    End Property
    Public Property TableauArc() As List(Of Arc)
        Get
            Return T_Arc
        End Get
        Set(ByVal value As List(Of Arc))
            T_Arc = value
        End Set
    End Property
    Public ReadOnly Property TableauTransitionValidable() As List(Of Transition)
        Get
            Return T_TransitionValidable
        End Get
    End Property
    Public ReadOnly Property TableauArcRentrant() As List(Of Arc)
        Get
            Return T_ArcRentrant
        End Get
    End Property
    Public ReadOnly Property TableauArcSortant() As List(Of Arc)
        Get
            Return T_ArcSortant
        End Get
    End Property
#End Region
#Region "Fonctions de mises à jour"
    Private Sub Maj_ArcRentrant(ByVal trans As Transition)
        T_ArcRentrant.Clear()

        'Gestion des évenements
        EnvoyerReseauChange("Mise à jour des arcs rentrants dans la transition d'hashcode " & CStr(trans.GetHashCode) & " et de nom '" & trans.nom & "'." & vbCrLf, Color.DarkMagenta)

        'On parcourt les élements de T_Arc
        For Each _arc As Arc In T_Arc
            'Utilisation de Is pour comparer deux instances d'objet
            If _arc.transition Is trans And _arc.sens = Arc.E_Sens.PlaceVersTransition Then
                T_ArcRentrant.Add(_arc)
            End If
        Next
    End Sub
    Private Sub Maj_ArcSortant(ByVal trans As Transition)
        T_ArcSortant.Clear()

        'Gestion des évenements
        EnvoyerReseauChange("Mise à jour des arcs sortants dans la transition d'hashcode " & CStr(trans.GetHashCode) & " et de nom '" & trans.nom & "'." & vbCrLf, Color.DarkMagenta)

        'On parcourt les élements de T_Arc
        For Each _arc As Arc In T_Arc
            'Utilisation de Is pour comparer deux instances d'objet
            If _arc.transition Is trans And _arc.sens = Arc.E_Sens.TransitionVersPlace Then
                T_ArcSortant.Add(_arc)
            End If
        Next
    End Sub
    Public Sub Maj_TransitionValidable()
        T_TransitionValidable.Clear()
        Dim is_validable As Boolean

        'Gestion des évenements
        EnvoyerReseauChange("Mise à jour des transitions validables dans la transition." & vbCrLf)

        'On parcourt les élements de T_Transition
        For Each _trans As Transition In T_Transition
            is_validable = True
            Me.Maj_ArcRentrant(_trans)
            For Each _arc As Arc In T_ArcRentrant
                If _arc.place.nombreJeton < _arc.multiplicite Or is_validable = False Then
                    is_validable = False
                End If
            Next
            If is_validable = True Then
                T_TransitionValidable.Add(_trans)
            End If
        Next
    End Sub
#End Region
#Region "Fonctions de validation"
    Public Function ChoixHasard_TransitionValidable() As Transition
        Me.Maj_TransitionValidable()
        Dim rand As New Random()
        Dim numTrans As Integer = CInt(rand.Next(0, T_TransitionValidable.Count))
        'Gestion des évenements
        EnvoyerReseauChange("Choix au hasard d'une transition dans les transitions validables." & vbCrLf & "Il y'a " & CStr(T_TransitionValidable.Count) & " transition(s) validable(s) et je choisis la " & CStr(numTrans + 1) & " eme (ière)." & vbCrLf & "Il s'agit de la transition d'hashcode " & CStr(T_TransitionValidable.Item(numTrans).GetHashCode) & " et de nom '" & T_TransitionValidable.Item(numTrans).nom & "'." & vbCrLf, Color.DarkRed)
        Return T_TransitionValidable.Item(numTrans)
    End Function
    Public Sub ValiderTransition(ByVal trans As Transition)
        Me.Maj_ArcRentrant(trans)
        For Each _arcRentrant As Arc In T_ArcRentrant
            _arcRentrant.place.DiminuerDeN(_arcRentrant.multiplicite)
            EnvoyerReseauChange("J'enlève " & CStr(_arcRentrant.multiplicite) & " jeton(s) à la place d'hashcode " & CStr(_arcRentrant.place.GetHashCode) & " et de nom '" & _arcRentrant.place.nom & "'." & vbCrLf, Color.DarkOliveGreen)
        Next

        Me.Maj_ArcSortant(trans)
        For Each _arcSortant As Arc In T_ArcSortant
            _arcSortant.place.AugmenterDeN(_arcSortant.multiplicite)
            EnvoyerReseauChange("Je rajoute " & CStr(_arcSortant.multiplicite) & " jeton(s) à la place d'hashcode " & CStr(_arcSortant.place.GetHashCode) & " et de nom '" & _arcSortant.place.nom & "'." & vbCrLf, Color.DarkOliveGreen)
        Next
    End Sub
#End Region
#Region "Fonction d'affichage et de retour"
    Public Function RetournerEquivalenceTreeNode() As TreeNode
        Dim tree As New TreeNode()
        tree.Text = "Données"
        tree.Nodes.Add("Places", "Places")
        tree.Nodes.Add("Transitions", "Transitions")
        tree.Nodes.Add("Arc", "Arc")
        'Maj des nodes destinés aux places
        For Each _place In T_Place
            If _place IsNot Nothing Then
                tree.Nodes("Places").Nodes.Add(CStr(_place.GetHashCode), "[" & _place.GetHashCode & "] : '" & _place.nom & "' (" & _place.nombreJeton & " jetons)")
            End If
        Next
        'Maj des nodes destinés aux transitions
        For Each _trans In T_Transition
            tree.Nodes("Transitions").Nodes.Add(CStr(_trans.GetHashCode), "[" & _trans.GetHashCode & "] : '" & _trans.nom & "' ")
        Next
        'Maj des nodes destinés aux arcs
        For Each _arc In T_Arc
            If _arc.sens = Arc.E_Sens.PlaceVersTransition Then
                tree.Nodes("Arc").Nodes.Add(CStr(_arc.GetHashCode), "[" & _arc.GetHashCode & "] : " & _arc.place.nom & " -> " & _arc.transition.nom & " [x" & _arc.multiplicite & "]")
            Else
                tree.Nodes("Arc").Nodes.Add(CStr(_arc.GetHashCode), "[" & _arc.GetHashCode & "] : " & _arc.transition.nom & " -> " & _arc.place.nom & " [x" & _arc.multiplicite & "]")
            End If
        Next
        Return tree
    End Function
    Public Sub GenererEtat()
        For Each _place In T_Place
            If _place.nombreJeton = 0 Then
                EnvoyerReseauChange("Il ne reste plus de jeton dans la place " & _place.nom & "." & vbCrLf, Color.DarkOrange)
            ElseIf _place.nombreJeton = 1 Then
                EnvoyerReseauChange("Il y a un unique jeton dans la place " & _place.nom & "." & vbCrLf, Color.DarkOrange)
            Else
                EnvoyerReseauChange("Il y a " & CStr(_place.nombreJeton) & " jetons dans la place " & _place.nom & "." & vbCrLf, Color.DarkOrange)
            End If
        Next
    End Sub
#End Region
End Class