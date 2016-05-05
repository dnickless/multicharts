Imports PowerLanguage
Imports System
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Enabled)> _
    Public Class vb_From_Broker_To_Strategy_MP_Synchronizer
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.LatencyMS = 500
            Me.TimeOutMS = &H3E8
        End Sub

        Protected Overrides Sub CalcBar()
            If (MyBase.Environment.IsRealTimeCalc AndAlso MyBase.Environment.IsAutoTradingMode) Then
                Dim m_inner_mp As Integer = MyBase.StrategyInfo.MarketPosition
                Dim m_broker_mp As Integer = MyBase.StrategyInfo.MarketPositionAtBroker
                Me.m_textid.Location = New ChartPoint(MyBase.Environment.RightScreenTime, MyBase.Environment.HighestScaleValue)
                If (m_broker_mp <> m_inner_mp) Then
                    Me.m_mp_diff_detected_time = DateTime.Now
                    If Not (Me.m_mp_diff OrElse Me.m_mp_corrected) Then
                        Me.m_mp_diff = True
                        Me.m_mp_diff_time_start = Me.m_mp_diff_detected_time
                        Me.m_textid.Text = Me.m_diff_state
                    End If
                    Dim _exit_price As Double = 0
                    Dim _place_order As Boolean = False
                    If (Me.m_mp_diff AndAlso Not Me.m_mp_corrected) Then
                        _exit_price = MyBase.Bars.Close.Item(0)
                        If (DirectCast((Me.m_mp_diff_detected_time - Me.m_mp_diff_time_start), TimeSpan).TotalMilliseconds >= Me.TimeOutMS) Then
                            _place_order = True
                            Me.m_mp_corrected = True
                            Me.m_mp_corrected_time_start = Me.m_mp_diff_detected_time
                        End If
                    End If
                    If Me.m_mp_corrected Then
                        If (DirectCast((Me.m_mp_diff_detected_time - Me.m_mp_corrected_time_start), TimeSpan).TotalMilliseconds >= Me.LatencyMS) Then
                            Me.m_mp_corrected_time_start = Me.m_mp_diff_detected_time
                            Me.m_mp_diff = False
                            Me.m_mp_corrected = False
                        End If
                    End If
                    If _place_order Then
                        If (0 <> m_broker_mp) Then
                            _exit_price = MyBase.StrategyInfo.AvgEntryPriceAtBroker
                        End If
                        MyBase.ChangeMarketPosition((m_broker_mp - m_inner_mp), _exit_price, "Sync Order")
                    End If
                Else
                    Me.m_textid.Text = Me.m_sync_state
                    Me.m_mp_corrected = False
                    Me.m_mp_diff = False
                End If
            End If
        End Sub

        Protected Overrides Sub Destroy()
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_textid = MyBase.DrwText.Create(New ChartPoint(MyBase.Bars.Time.Item(0), MyBase.Bars.Close.Item(0)), "CurrentState")
            Me.m_textid.HStyle = ETextStyleH.Left
            Me.m_textid.VStyle = ETextStyleV.Below
            Me.m_sync_state = "MP is synchronized!"
            Me.m_diff_state = String.Format("MP syncronization." & ChrW(10) & "Wait {0} seconds", (Me.TimeOutMS * 0.001))
            Me.m_mp_diff = False
            Me.m_mp_corrected = False
        End Sub


        ' Properties
        <Input()> _
        Public Property LatencyMS As Integer

        <Input()> _
        Public Property TimeOutMS As Integer


        ' Fields
        Private m_diff_state As String
        Private m_mp_corrected As Boolean
        Private m_mp_corrected_time_start As DateTime
        Private m_mp_diff As Boolean
        Private m_mp_diff_detected_time As DateTime
        Private m_mp_diff_time_start As DateTime
        Private m_sync_state As String
        Private m_textid As ITextObject
    End Class
End Namespace
