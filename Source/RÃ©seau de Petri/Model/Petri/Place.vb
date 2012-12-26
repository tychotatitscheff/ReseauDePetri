Imports System.Xml.Serialization
''' <summary>
''' La classe Place represente les place dans le réseau de Pétri.
''' Elle a un nom et un nombre de jetons.
''' </summary>
''' <remarks>Pour plus d'information sur la partie théorique : http://fr.wikipedia.org/wiki/R%C3%A9seau_de_Petri </remarks>
<Serializable()>
Public Class Place
#Region "Attributs privés"
    <XmlAttribute("Nom")> Private _nom As String
    <XmlElement("Nombre de jeton")> Private _nombreJeton As UInteger
#End Region
#Region "Constructeur"
    Public Sub New()

    End Sub
    Public Sub New(ByVal p_nombre As UInteger, ByVal p_nom As String)
        _nombreJeton = p_nombre
        _nom = p_nom
    End Sub
#End Region
#Region "Properties"
    Public Property nom() As String
        Get
            Return _nom
        End Get
        Set(ByVal p_nom As String)
            _nom = p_nom
        End Set
    End Property
    Public Property nombreJeton() As UInteger
        Get
            Return _nombreJeton
        End Get
        Set(ByVal p_nombre As UInteger)
            _nombreJeton = p_nombre
        End Set
    End Property
#End Region
#Region "Méthodes d'incrementation et décrementation des jetons"
    Public Sub AugmenterDeN(ByVal n As UInteger)
        _nombreJeton = _nombreJeton + n
    End Sub
    Public Sub DiminuerDeN(ByVal n As UInteger)
        _nombreJeton = _nombreJeton - n
    End Sub
#End Region
#Region "Surcharge de ToString()"
    Public Overrides Function ToString() As String
        Return _nom & " [" & MyBase.GetHashCode & "]"
    End Function
#End Region
End Class