Imports System.Xml.Serialization
''' <summary>
''' La classe transition représente les transitions dans un réseau de pétri.
''' Elle ne comporte qu'un seul attribut privé, son nom.
''' </summary>
''' <remarks>Pour plus d'information sur la partie théorique : http://fr.wikipedia.org/wiki/R%C3%A9seau_de_Petri </remarks>
<Serializable()>
Public Class Transition
#Region "Attribut Privé"
    <XmlAttribute("Nom")> Private _nom As String
#End Region
#Region "Constructeur"
    Public Sub New()

    End Sub
    Public Sub New(ByVal p_nom As String)
        _nom = p_nom
    End Sub
#End Region
#Region "Property"
    Public Property nom() As String
        Get
            Return _nom
        End Get
        Set(ByVal p_nom As String)
            _nom = p_nom
        End Set
    End Property
#End Region
#Region "Surcharge de ToString()"
    Public Overrides Function ToString() As String
        Dim hash As String
        If Main.ChB_verbose.Checked Then
            hash = " [" & MyBase.GetHashCode & "]"
        Else
            hash = ""
        End If
        Return _nom & hash
    End Function
#End Region
End Class