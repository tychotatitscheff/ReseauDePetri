Imports System.Xml.Serialization
''' <summary>
''' Cette classe définit les arcs orientés reliant place et transition.
''' Elle comporte trois éléments, une place, une transition et un sens ce qui impose la création d'une enumération "sens".
''' Nous avons fait le choix d'implémenter la multiplicité et ainsi de rajouter un attribut multiplicité.
''' </summary>
''' <remarks>Pour plus d'information sur la partie théorique : http://fr.wikipedia.org/wiki/R%C3%A9seau_de_Petri </remarks>
<Serializable()>
Public Class Arc
#Region "Enumération définissant le sens"
    Public Enum E_Sens As Integer
        <XmlEnum("Place vers transition")> PlaceVersTransition = 1
        <XmlEnum("Tarnsition vers place")> TransitionVersPlace = 2
    End Enum
#End Region
#Region "Attributs Privés"
    <XmlElement("Place")> Private _Place As Place
    <XmlElement("Transition")> Private _Transition As Transition
    <XmlElement("Multiplicité")> Private _multiplicite As UInteger
    <XmlElement("Sens")> Private _sens As E_Sens
#End Region
#Region "Constructeurs (1 surcharge)"
    Public Sub New()

    End Sub
    Public Sub New(ByVal p_place As Place, ByVal p_transition As Transition, ByVal p_sens As E_Sens, ByVal p_mult As UInteger)
        _Place = p_place
        _Transition = p_transition
        _sens = p_sens
        _multiplicite = p_mult
    End Sub
    Public Sub New(ByVal p_place As Place, ByVal p_transition As Transition, ByVal p_sens As E_Sens)
        _Place = p_place
        _Transition = p_transition
        _sens = p_sens
        _multiplicite = 1
    End Sub
#End Region
#Region "Properties"
    Public Property place() As Place
        Get
            Return _Place
        End Get
        Set(ByVal p_place As Place)
            _Place = p_place
        End Set
    End Property
    Public Property transition() As Transition
        Get
            Return _Transition
        End Get
        Set(ByVal p_trans As Transition)
            _Transition = p_trans
        End Set
    End Property
    Public Property multiplicite() As UInteger
        Get
            Return _multiplicite
        End Get
        Set(ByVal p_mult As UInteger)
            _multiplicite = p_mult
        End Set
    End Property
    Public Property sens() As E_Sens
        Get
            Return _sens
        End Get
        Set(ByVal p_sens As E_Sens)
            _sens = p_sens
        End Set
    End Property
#End Region
#Region "Surcharge de ToString()"
    Public Overrides Function ToString() As String
        Return "[" & MyBase.GetHashCode & "]"
    End Function
#End Region
End Class