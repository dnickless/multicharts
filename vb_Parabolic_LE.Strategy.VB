Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Parabolic_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.AFLimit = 0.2
            Me.AFStep = 0.02
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_ParabolicSAR.Call
            If (Me.m_Oposition.Value = -1) Then
                Me.m_ParLE.Send(Me.m_oparop.Value)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_ParabolicSAR = New [Function].ParabolicSAR(Me)
            Me.m_oparcl = New VariableObject(Of Double)(Me)
            Me.m_oparop = New VariableObject(Of Double)(Me)
            Me.m_Oposition = New VariableObject(Of Integer)(Me)
            Me.m_otransition = New VariableObject(Of Integer)(Me)
            Me.m_ParLE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "ParLE", EOrderAction.Buy))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_ParabolicSAR.afstep = Me.AFStep
            Me.m_ParabolicSAR.aflimit = Me.AFLimit
            Me.m_ParabolicSAR.oparcl = Me.m_oparcl
            Me.m_ParabolicSAR.oparop = Me.m_oparop
            Me.m_ParabolicSAR.oposition = Me.m_Oposition
            Me.m_ParabolicSAR.otransition = Me.m_otransition
        End Sub


        ' Properties
        <Input> _
        Public Property AFLimit As Double

        <Input()> _
        Public Property AFStep As Double


        ' Fields
        Private m_oparcl As VariableObject(Of Double)
        Private m_oparop As VariableObject(Of Double)
        Private m_Oposition As VariableObject(Of Integer)
        Private m_otransition As VariableObject(Of Integer)
        Private m_ParabolicSAR As [Function].ParabolicSAR
        Private m_ParLE As IOrderPriced
    End Class
End Namespace
