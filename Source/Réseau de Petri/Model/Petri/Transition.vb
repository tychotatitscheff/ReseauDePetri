Imports System.Runtime.Serialization
<DataContract(IsReference:=True)>
Public Class Transition
#Region "Attribut Privé"
    Private _nom As String
#End Region
#Region "Constructeur"
    Public Sub New()

    End Sub
    Public Sub New(ByVal p_nom As String)
        _nom = p_nom
    End Sub
#End Region
#Region "Property"
    <DataMember(Name:="Nom_de_la_transition", Order:=1, EmitDefaultValue:=True)>
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