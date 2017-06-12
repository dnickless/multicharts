Imports PowerLanguage
Imports System

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Enabled)> _
    Public Class vb_From_Strategy_To_Broker_MP_Synchronizer
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.m_timeoutms = 1000
            Me.m_latencyms = 1000
        End Sub

        Protected Overrides Sub CalcBar()
            If (MyBase.Environment.IsRealTimeCalc AndAlso MyBase.Environment.IsAutoTradingMode) Then
                Dim m_inner_mp As Integer = MyBase.StrategyInfo.MarketPosition
                Dim m_broker_mp As Integer = MyBase.StrategyInfo.MarketPositionAtBroker
                Dim m__rightest As DateTime = MyBase.Environment.RightScreenTime
                Dim m__highest As Double = MyBase.Environment.HighestScaleValue
                If Not Nothing Is Me.m_textid Then
                    Me.m_textid.Location = New ChartPoint(m__rightest, m__highest)
                End If
                If (m_broker_mp <> m_inner_mp) Then
                    Dim m__get_tick_count As Long = DateTime.Now.Ticks
                    If Not (Me.m_mp_diff OrElse Me.m_mp_corrected) Then
                        Me.m_mp_diff = True
                        Me.m_mp_diff_time_start = m__get_tick_count
                        If Not Nothing Is Me.m_textid Then
                            Me.m_textid.Text = Me.m_diff_state
                        End If
                    End If
                    If ((Me.m_mp_diff AndAlso Not Me.m_mp_corrected) AndAlso PublicFunctions.DoubleGreater((m__get_tick_count - Me.m_mp_diff_time_start), Me.TimeOut_ms)) Then
                        Me.m_place_correction_marketorder = True
                        Me.m_mp_corrected = True
                        Me.m_mp_corrected_time_start = m__get_tick_count
                    End If
                    If Me.m_mp_corrected Then
                        If Not Nothing Is Me.m_textid Then
                            Me.m_textid.Text = Me.m_correction_state
                        End If
                        If PublicFunctions.DoubleGreater((m__get_tick_count - Me.m_mp_corrected_time_start), Me.Latency_ms) Then
                            Me.m_mp_corrected_time_start = m__get_tick_count
                            Me.m_mp_diff = False
                            Me.m_mp_corrected = False
                        End If
                    End If
                    If Me.m_place_correction_marketorder Then
                        Dim m_correct_contracts As Integer = Math.Abs(CInt((m_broker_mp - m_inner_mp)))
                        Me.m_place_correction_marketorder = False
                        Dim m_is_buy As Boolean = PublicFunctions.IffLogic(Of Boolean)((m_broker_mp > m_inner_mp), False, True)
                        MyBase.GenerateATMarketOrder(m_is_buy, True, m_correct_contracts)
                    End If
                Else
                    If Not Nothing Is Me.m_textid Then
                        Me.m_textid.Text = Me.m_sync_state
                    End If
                    Me.m_mp_corrected = False
                    Me.m_mp_diff = False
                    End If
            End If
        End Sub

        Protected Overrides Sub StartCalc()
            If MyBase.Environment.ApplicationCode = EApplicationCode.Portfolio Then
                Me.m_textid = Nothing
            Else
                Me.m_textid = MyBase.DrwText.Create(New ChartPoint(MyBase.Bars.Time.Item(0), MyBase.Bars.Close.Item(0)), "CurrentState")
            End If
            Me.m_sync_state = "MP is synchronized!"
            Me.m_diff_state = ("MP syncronization. Wait " & (Me.TimeOut_ms * 0.001) & " seconds.")
            Me.m_correction_state = ("MP correction order sent. Wait " & (Me.Latency_ms * 0.001) & " seconds.")
            Me.m_mp_diff = False
            Me.m_mp_diff_time_start = 0
            Me.m_mp_corrected = False
            Me.m_mp_corrected_time_start = 0
            Me.m_place_correction_marketorder = False
        End Sub


        ' Properties
        <Input()> _
        Public Property Latency_ms As Double
            Get
                Return Me.m_latencyms
            End Get
            Set(ByVal value As Double)
                Me.m_latencyms = value
            End Set
        End Property

        <Input()> _
        Public Property TimeOut_ms As Double
            Get
                Return Me.m_timeoutms
            End Get
            Set(ByVal value As Double)
                Me.m_timeoutms = value
            End Set
        End Property


        ' Fields
        Private m_correction_state As String
        Private m_diff_state As String
        Private m_latencyms As Double
        Private m_mp_corrected As Boolean
        Private m_mp_corrected_time_start As Double
        Private m_mp_diff As Boolean
        Private m_mp_diff_time_start As Double
        Private m_place_correction_marketorder As Boolean
        Private m_sync_state As String
        Private m_textid As ITextObject
        Private m_timeoutms As Double
    End Class
End Namespace
