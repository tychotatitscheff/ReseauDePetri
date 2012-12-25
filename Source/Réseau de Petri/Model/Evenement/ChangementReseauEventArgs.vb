''' <summary>
''' Cette classe permet la création d'un nouvel évenement.
''' </summary>
''' <remarks>Il serait préférable d'impléménter une veriatble classe d'erreur.</remarks>
Public Class ChangementReseauEventArgs
    Inherits EventArgs 'Elle hérite donc de la classe System.EventArgs qui est la classe de base des classes contenant des données d'événement.
    Private textEvent As String = Nothing
    Private couleurEvent As Color = Nothing
    Public Sub New(ByVal p_EventText As String, ByVal p_colorEvent As Color)
        If p_EventText Is Nothing Then
            Throw New NullReferenceException()
        End If
        textEvent = p_EventText
        If p_EventText Is Nothing Then
            Throw New NullReferenceException()
        End If
        couleurEvent = p_colorEvent
    End Sub
    Public ReadOnly Property Text As String
        Get
            Return Me.textEvent
        End Get
    End Property
    Public ReadOnly Property Couleur As Color
        Get
            Return Me.couleurEvent
        End Get
    End Property
End Class