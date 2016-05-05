Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_TrendLines_Automatic
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.alerttype = "IntraBar"
            Me.uptlcolor = Color.Cyan
            Me.dntlcolor = Color.Red
            Me.history = "Yes"
            Me.barspast = 10
            Me.swlostrength = 4
            Me.swhistrength = 4
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_value1 As Integer
            Dim m_value2 As Integer
            Dim m_barnum As Integer = Me.m_barnumber1.Item(0)
            If (Me.m_swinghighbar1.Item(0) = Me.swhistrength) Then
                m_value1 = 9
                Do While (m_value1 >= 0)
                    Me.m_swhitime.Item(0)((m_value1 + 1)) = Me.m_swhitime.Item(0)(m_value1)
                    Me.m_swhival.Item(0)((m_value1 + 1)) = Me.m_swhival.Item(0)(m_value1)
                    m_value1 -= 1
                Loop
                Me.m_swhitime.Item(0)(0) = MyBase.Bars.Time.Item(Me.swhistrength)
                Me.m_swhival.Item(0)(0) = MyBase.Bars.High.Item(Me.swhistrength)
                m_value2 = 1
                Do While (m_value2 <= 10)
                    If (Me.m_swhival.Item(0)(m_value2) > Me.m_swhival.Item(0)(0)) Then
                        Me.m_index.Value = m_value2
                        m_value2 = 11
                    End If
                    m_value2 += 1
                Loop
                If (m_value2 = 12) Then
                    If ((Not Me.m_dntlref.Item(0) Is Nothing) AndAlso Me.m_dntlref.Item(0).Exist) Then
                        If Not If(Not Me.m_historytf, True, Me.m_dntlbreak.Value) Then
                            Me.m_dntlref.Value.End = New ChartPoint(MyBase.Bars.Time.Item(0), Me.m_dntlref.Value.PriceValue(MyBase.Bars.Time.Item(0)))
                            Me.m_dntlref.Value.ExtRight = False
                        ElseIf Not Me.m_historytf Then
                            Me.m_dntlref.Value.Delete()
                        End If
                    End If
                    Me.m_dntlref.Value = MyBase.DrwTrendLine.Create(New ChartPoint(Me.m_swhitime.Item(0)(Me.m_index.Value), Me.m_swhival.Item(0)(Me.m_index.Value)), New ChartPoint(Me.m_swhitime.Item(0)(0), Me.m_swhival.Item(0)(0)))
                    If Me.m_dntlbreak.Value Then
                        Me.m_dntlbreak.Value = False
                    End If
                    Me.m_dntlendbar.Value = (m_barnum - Me.swhistrength)
                    Me.m_dntlref.Value.ExtLeft = False
                    Me.m_dntlref.Value.ExtRight = True
                    Me.m_dntlref.Value.Color = Me.dntlcolor
                    If (Me.m_alerttypecaps = "ONCLOSE") Then
                        Me.m_dntlref.Value.Alert = EAlertType.BreakoutOnClose
                    ElseIf (Me.m_alerttypecaps = "INTRABAR") Then
                        Me.m_dntlref.Value.Alert = EAlertType.BreakoutIntrabar
                    Else
                        Me.m_dntlref.Value.Alert = EAlertType.Disabled
                    End If
                End If
            End If
            If (Me.m_swinglowbar1.Item(0) = Me.swlostrength) Then
                m_value1 = 9
                Do While (m_value1 >= 0)
                    Me.m_swlotime.Item(0)((m_value1 + 1)) = Me.m_swlotime.Item(0)(m_value1)
                    Me.m_swloval.Item(0)((m_value1 + 1)) = Me.m_swloval.Item(0)(m_value1)
                    m_value1 -= 1
                Loop
                Me.m_swlotime.Item(0)(0) = MyBase.Bars.Time.Item(Me.swlostrength)
                Me.m_swloval.Item(0)(0) = MyBase.Bars.Low.Item(Me.swlostrength)
                m_value2 = 1
                Do While (m_value2 <= 10)
                    If (Me.m_swloval.Item(0)(m_value2) < Me.m_swloval.Item(0)(0)) Then
                        Me.m_index.Value = m_value2
                        m_value2 = 11
                    End If
                    m_value2 += 1
                Loop
                If (m_value2 = 12) Then
                    If ((Not Me.m_uptlref.Value Is Nothing) AndAlso Me.m_uptlref.Value.Exist) Then
                        If Not If(Not Me.m_historytf, True, Me.m_uptlbreak.Value) Then
                            Me.m_uptlref.Value.End = New ChartPoint(MyBase.Bars.Time.Item(0), Me.m_uptlref.Value.PriceValue(MyBase.Bars.Time.Item(0)))
                            Me.m_uptlref.Value.ExtRight = False
                        ElseIf Not Me.m_historytf Then
                            Me.m_uptlref.Value.Delete()
                        End If
                    End If
                    Me.m_uptlref.Value = MyBase.DrwTrendLine.Create(New ChartPoint(Me.m_swlotime.Item(0)(Me.m_index.Value), Me.m_swloval.Item(0)(Me.m_index.Value)), New ChartPoint(Me.m_swlotime.Item(0)(0), Me.m_swloval.Item(0)(0)))
                    If Me.m_uptlbreak.Value Then
                        Me.m_uptlbreak.Value = False
                    End If
                    Me.m_uptlendbar.Value = (m_barnum - Me.swlostrength)
                    Me.m_uptlref.Value.ExtLeft = False
                    Me.m_uptlref.Value.ExtRight = True
                    Me.m_uptlref.Value.Color = Me.uptlcolor
                    If (Me.m_alerttypecaps = "ONCLOSE") Then
                        Me.m_uptlref.Value.Alert = EAlertType.BreakoutOnClose
                    Else
                        Me.m_uptlref.Value.Alert = If((Me.m_alerttypecaps = "INTRABAR"), EAlertType.BreakoutIntrabar, EAlertType.Disabled)
                    End If
                End If
            End If
            If ((((Not Me.m_dntlref.Value Is Nothing) AndAlso Me.m_dntlref.Value.Exist) AndAlso (Not Me.m_dntlbreak.Value AndAlso (m_barnum > ((Me.m_dntlendbar.Value + Me.swhistrength) + Me.barspast)))) AndAlso PublicFunctions.DoubleGreater(MyBase.Bars.Close.Item(Me.barspast), Me.m_dntlref.Value.PriceValue(MyBase.Bars.Time.Item(0)))) Then
                Me.m_dntlref.Value.End = New ChartPoint(MyBase.Bars.Time.Item(0), Me.m_dntlref.Value.PriceValue(MyBase.Bars.Time.Item(0)))
                Me.m_dntlref.Value.ExtRight = False
                Me.m_dntlbreak.Value = True
            End If
            If ((((Not Me.m_uptlref.Value Is Nothing) AndAlso Me.m_uptlref.Value.Exist) AndAlso (Not Me.m_uptlbreak.Value AndAlso (m_barnum > ((Me.m_uptlendbar.Value + Me.swlostrength) + Me.barspast)))) AndAlso PublicFunctions.DoubleLess(MyBase.Bars.Close.Item(Me.barspast), Me.m_uptlref.Value.PriceValue(MyBase.Bars.Time.Item(0)))) Then
                Me.m_uptlref.Value.End = New ChartPoint(MyBase.Bars.Time.Item(0), Me.m_uptlref.Value.PriceValue(MyBase.Bars.Time.Item(0)))
                Me.m_uptlref.Value.ExtRight = False
                Me.m_uptlbreak.Value = True
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_barnumber1 = New [Function].BarNumber(Me)
            Me.m_swinghighbar1 = New [Function].SwingHighBar(Me)
            Me.m_swinglowbar1 = New [Function].SwingLowBar(Me)
            Me.m_dntlref = New VariableSeries(Of ITrendLineObject)(Me)
            Me.m_dntlendbar = New VariableObject(Of Integer)(Me)
            Me.m_dntlbreak = New VariableObject(Of Boolean)(Me)
            Me.m_uptlref = New VariableSeries(Of ITrendLineObject)(Me)
            Me.m_uptlendbar = New VariableObject(Of Integer)(Me)
            Me.m_uptlbreak = New VariableObject(Of Boolean)(Me)
            Me.m_index = New VariableObject(Of Integer)(Me)
            Me.m_swhitime = New ArraySimple(Of DateTime)(Me, 11)
            Me.m_swhival = New ArraySimple(Of Double)(Me, 11)
            Me.m_swlotime = New ArraySimple(Of DateTime)(Me, 11)
            Me.m_swloval = New ArraySimple(Of Double)(Me, 11)
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_swinghighbar1.instance = 1
            Me.m_swinghighbar1.price = MyBase.Bars.High
            Me.m_swinghighbar1.strength = Me.swhistrength
            Me.m_swinghighbar1.length = (Me.swhistrength + 1)
            Me.m_swinglowbar1.instance = 1
            Me.m_swinglowbar1.price = MyBase.Bars.Low
            Me.m_swinglowbar1.strength = Me.swlostrength
            Me.m_swinglowbar1.length = (Me.swlostrength + 1)
            Me.m_dntlendbar.DefaultValue = 0
            Me.m_dntlbreak.DefaultValue = False
            Me.m_uptlendbar.DefaultValue = 0
            Me.m_uptlbreak.DefaultValue = False
            Me.m_index.DefaultValue = 0
            Me.m_historytf = ((Me.history.ToUpper = "YES") OrElse (Me.history.ToUpper = "Y"))
            Me.m_alerttypecaps = Me.alerttype.ToUpper
            Me.m_swhival.DefaultValue = -1000000
            Me.m_swloval.DefaultValue = 1000000
        End Sub


        ' Properties
        <Input> _
        Public Property alerttype As String

        <Input()> _
        Public Property barspast As Integer

        <Input()> _
        Public Property dntlcolor As Color

        <Input()> _
        Public Property history As String

        <Input()> _
        Public Property swhistrength As Integer

        <Input()> _
        Public Property swlostrength As Integer

        <Input()> _
        Public Property uptlcolor As Color


        ' Fields
        Private m_alerttypecaps As String
        Private m_barnumber1 As [Function].BarNumber
        Private m_dntlbreak As VariableObject(Of Boolean)
        Private m_dntlendbar As VariableObject(Of Integer)
        Private m_dntlref As VariableSeries(Of ITrendLineObject)
        Private m_historytf As Boolean
        Private m_index As VariableObject(Of Integer)
        Private m_swhitime As ArraySimple(Of DateTime)
        Private m_swhival As ArraySimple(Of Double)
        Private m_swinghighbar1 As [Function].SwingHighBar
        Private m_swinglowbar1 As [Function].SwingLowBar
        Private m_swlotime As ArraySimple(Of DateTime)
        Private m_swloval As ArraySimple(Of Double)
        Private m_uptlbreak As VariableObject(Of Boolean)
        Private m_uptlendbar As VariableObject(Of Integer)
        Private m_uptlref As VariableSeries(Of ITrendLineObject)
    End Class
End Namespace
